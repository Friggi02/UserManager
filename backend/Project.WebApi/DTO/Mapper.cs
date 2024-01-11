using Project.Dal.Entities;
using Project.WebApi.DTO.Output;

namespace Project.WebApi.DTO
{
    public class Mapper
    {
        public UserDTO MapUserToDTO(User user) => new UserDTO
        {
            Id = user.Id,
            Email = user.Email!,
            Username = user.UserName!,
            IsDeleted = user.IsDeleted,
            Name = user.Name,
            Surname = user.Surname,
            ProfilePic = user.ProfilePic,
            Roles = user.Roles
        };
    }
}
