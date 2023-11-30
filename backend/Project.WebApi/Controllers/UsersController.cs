using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Project.Dal.Entities;
using Project.Dal.Jwt;
using Project.Dal.Permit;
using Project.Dal.Repositories.Interfaces;
using Project.WebApi.DTO.Input;

namespace Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ODataController
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _repo;
        private readonly IJwtProvider _jwtProvider;

        public UsersController(
            UserManager<User> userManager,
            IConfiguration configuration,
            IUnitOfWork repo,
            IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _configuration = configuration;
            _repo = repo;
            _jwtProvider = jwtProvider;
        }

        [HttpGet]
        [HasPermission(Permissions.ManageUsers)]
        public IActionResult Get(ODataQueryOptions<User> options)
        {
            return Ok(_repo.UserRepo.GetAllOData(options));
        }

        [HttpGet]
        [HasPermission(Permissions.ManageUsers)]
        [Route("GetSingleById")]
        public IActionResult GetSingleById(string id)
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // get user and check if exist
            User? user = _repo.UserRepo.GetById(id);
            if (user == null) return NotFound("User not found");

            return Ok(user);
        }

        [HttpGet]
        [HasPermission(Permissions.ManageMyself)]
        [Route("SelfGet")]
        public async Task<IActionResult> SelfGet()
        {
            string audienceId = _jwtProvider.GetIdFromRequest(Request);

            // check if audience exists
            if (!_repo.UserRepo.Exist("Id", audienceId, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // get the user from the http context
            User? user = await _userManager.FindByIdAsync(audienceId);

            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {

            // checks on the loginModel
            if (string.IsNullOrEmpty(model.Email) || model.Email.Length > 40 || model.Email.Contains(" ")) return BadRequest("Email is not valid");
            if (string.IsNullOrEmpty(model.Password) || model.Password.Length > 40) return BadRequest("Password is not valid");

            // get the user form db and check if exists
            User? user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("Access denied");
            if (user.IsDeleted) return Unauthorized("User is deactivated");

            // check the lockout
            if (user.IsLockedout()) return Unauthorized($"Too many tries. You're locked out until {user.LockoutEnd}");

            // getting the accessFailedMax from appsetting.json
            int accessFailedMax;
            try
            {
                accessFailedMax = _configuration.GetValue<int>("SecuritySettings:AccessFailedMax");
            }
            catch (Exception) { return StatusCode(StatusCodes.Status500InternalServerError, "Failed reading AccessFailedMax from appsetting.json"); }

            // getting the lockoutTime from appsetting.json
            TimeSpan lockoutTime;
            try
            {
                lockoutTime = _configuration.GetValue<TimeSpan>("SecuritySettings:LockoutTime");
            }
            catch (Exception) { return StatusCode(StatusCodes.Status500InternalServerError, "Failed reading LockoutTime from appsetting.json"); }

            // check the number of accesses failed and set lockout
            if (user.AccessFailedCount > accessFailedMax)
            {
                user.AccessFailedCount = 0;
                user.LockoutEnd = DateTime.UtcNow + lockoutTime;
                await _userManager.UpdateAsync(user);
                return Unauthorized($"Too many tries. You're locked out until {user.LockoutEnd}");
            }

            // check if the password is correct
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                user.AccessFailedCount++;
                await _userManager.UpdateAsync(user);
                return Unauthorized("Access denied");
            }

            // remove the lockout
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            // build tokens
            string newAccessToken = await _jwtProvider.GenerateAccessToken(user);
            string newRefreshToken = _jwtProvider.GenerateRefreshToken(user);

            // saving the refresh token
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // get the user form db and check if exists
            User? user = await _userManager.FindByIdAsync(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (user == null) return Unauthorized("Access denied");
            if (user.IsDeleted) return Unauthorized("User is deactivated");

            // check the refresh token on the db
            if (user.RefreshToken != refreshToken) return Unauthorized("RefreshToken not valid");

            // check the token's expire date
            if (new JwtSecurityTokenHandler().ReadToken(refreshToken) is not JwtSecurityToken jsonToken) return Unauthorized("RefreshToken not valid");
            if (_jwtProvider.GetExpirationDate(refreshToken) < DateTime.UtcNow)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
                return Unauthorized("RefreshToken expired");
            }

            // build tokens
            string newAccessToken = await _jwtProvider.GenerateAccessToken(user);
            string newRefreshToken = _jwtProvider.GenerateRefreshToken(user);

            // saving the refresh token
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser(RegisterModel model)
        {

            // check the model's property
            if (string.IsNullOrEmpty(model.Password) || model.Password.Length > 40) return BadRequest("Password is not valid");
            if (string.IsNullOrEmpty(model.Email) || model.Email.Length > 40 || !Regex.IsMatch(model.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) return BadRequest("Email is not valid");

            // instantiate the object
            User user = new()
            {
                UserName = Guid.NewGuid().ToString().Replace("-", ""),
                Email = model.Email,
                IsDeleted = false,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            // creating
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, "User creation failed");

            // adding role
            _repo.UserRoleRepo.Create(new UserRole { UserId = user.Id, RoleId = Role.Registered.Id });

            return Created("User created successfully", user);
        }

        [HttpPost]
        [HasPermission(Permissions.ManageUsers)]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterModel model)
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // check the model's property
            if (string.IsNullOrEmpty(model.Password) || model.Password.Length > 40) return BadRequest("Password is not valid");
            if (string.IsNullOrEmpty(model.Email) || model.Email.Length > 40 || !Regex.IsMatch(model.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) return BadRequest("Email is not valid");

            // instantiate the object
            User user = new()
            {
                UserName = Guid.NewGuid().ToString().Replace("-", ""),
                Email = model.Email,
                IsDeleted = false,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            // creating
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, "Admin creation failed");

            // adding role
            _repo.UserRoleRepo.Create(new UserRole { UserId = user.Id, RoleId = Role.Admin.Id });

            return Created("Admin created successfully", user);
        }

        [HttpPut]
        [HasPermission(Permissions.ManageUsers)]
        [Route("UserToAdmin")]
        public IActionResult UserToAdmin(string id)
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // get user and check if exist
            User? user = _repo.UserRepo.GetById(id);
            if (user == null) return NotFound("User not found");
            if (user.IsDeleted) return NotFound("User is deactivated");

            // adding role
            Result result = _repo.UserRoleRepo.Create(new UserRole { UserId = user.Id, RoleId = Role.Admin.Id });

            if (result.Success)
            {
                return Ok("User upgraded successfully to admin");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "User upgrade to admin failed");
            }
        }

        [HttpPut]
        [HasPermission(Permissions.ManageUsers)]
        [Route("AdminToUser")]
        public IActionResult AdminToUser(string id)
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // get user and check if exist
            User? user = _repo.UserRepo.GetById(id);
            if (user == null) return NotFound("User not found");
            if (user.IsDeleted) return NotFound("User is deactivated");

            if (id == User.FindFirstValue(ClaimTypes.NameIdentifier)) return BadRequest("You cannot downgrade yourself");

            Result result = _repo.UserRoleRepo.DeleteByFilter(x => x.UserId == user.Id && x.RoleId == Role.Admin.Id);

            if (result.Success)
            {
                return Ok("Admin downgraded to user successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Admin downgrade to user failed");
            }
        }

        [HttpPut]
        [HasPermission(Permissions.ManageMyself)]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // get the user from the http context
            User? user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // checks on passwords
            if (model.CurrentPassword == model.NewPassword || await _userManager.CheckPasswordAsync(user!, model.CurrentPassword)) return BadRequest("New and old password are equal");

            // update password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user!);
            var result = await _userManager.ResetPasswordAsync(user!, token, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password updated successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Password update failed");
            }
        }

        [HttpPut]
        [HasPermission(Permissions.ManageMyself)]
        [Route("ChangeUsername")]
        public async Task<IActionResult> ChangeUsername(ChangeUserNameModel model)
        {

            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // checks on newUsername
            if (string.IsNullOrEmpty(model.NewUserName) || model.NewUserName.Length > 40 || model.NewUserName.Contains(" ")) return BadRequest("Username is not valid");


            // check the existance with the same username
            var userExists = await _userManager.FindByNameAsync(model.NewUserName);
            if (userExists != null) return BadRequest("Username already exists");

            // get the audience and check if exists
            User? user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (user == null) return NotFound("User not found");
            if (user.IsDeleted) return NotFound("User is deactivated");

            // checks on passwords
            if (await _userManager.CheckPasswordAsync(user!, model.CurrentPassword)) return BadRequest("Password not valid");

            // update Username
            user.UserName = model.NewUserName;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("Username updated successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Username update failed");
            }
        }

        [HttpPut]
        [HasPermission(Permissions.ManageMyself)]
        [Route("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailModel model)
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // checks on newEmail
            if (string.IsNullOrEmpty(model.NewEmail) || model.NewEmail.Length > 40 || !Regex.IsMatch(model.NewEmail, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) return BadRequest("Email is not valid");

            // check the existance with the same email
            var userExists = await _userManager.FindByEmailAsync(model.NewEmail);
            if (userExists != null) return BadRequest("Email already exists");

            // get the user from the http context
            User? user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // checks on passwords
            if (await _userManager.CheckPasswordAsync(user!, model.CurrentPassword)) return BadRequest("Password not valid");

            // update Email
            user!.Email = model.NewEmail;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("Email updated successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Email update failed");
            }
        }

        [HttpDelete]
        [HasPermission(Permissions.ManageMyself)]
        [Route("SelfDelete")]
        public async Task<IActionResult> SelfDelete()
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // get the user from the http context
            User? user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // check the number of admin
            var roles = await _userManager.GetRolesAsync(user!);
            if (roles.Contains("Admin"))
            {
                List<UserRole> userRoles = _repo.UserRoleRepo.GetByFilter(x => x.RoleId == Role.Admin.Id);
                List<User> admins = _repo.UserRepo.GetAllByRole(Role.Admin);
                foreach (User item in admins) if (item.IsDeleted) admins.Remove(item);
                if (admins.Count <= 1) return BadRequest("There is only one admin");
            }

            user!.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("Entity deleted successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Entity deletion failed");
            }
        }

        [HttpDelete]
        [HasPermission(Permissions.ManageUsers)]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // get user and check if exist
            User? user = _repo.UserRepo.GetById(id);
            if (user == null) return NotFound("User not found");
            if (user.IsDeleted) return NotFound("User is deactivated");

            // check if admin
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin")) return BadRequest("You cannot delete an admin.");

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("Entity deleted successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Entity deletion failed");
            }
        }

        [HttpDelete]
        [HasPermission(Permissions.ManageUsers)]
        [Route("Restore")]
        public async Task<IActionResult> Restore(string id)
        {
            // check if audience exists
            if (_repo.UserRepo.Exist("Id", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, user => !user.IsDeleted).Success) return Unauthorized("Audience not found");

            // get user and check if exist
            User? user = _repo.UserRepo.GetById(id);
            if (user == null) return NotFound("User not found");

            user.IsDeleted = false;
            var result = await _userManager.UpdateAsync(user);


            if (result.Succeeded)
            {
                return Ok("Entity restored successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Entity restoration failed");
            }
        }
    }
}