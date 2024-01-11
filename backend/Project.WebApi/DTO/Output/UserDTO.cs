using Project.Dal.Entities;

namespace Project.WebApi.DTO.Output
{
    public class UserDTO
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public bool IsDeleted { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? ProfilePic { get; set; }
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
