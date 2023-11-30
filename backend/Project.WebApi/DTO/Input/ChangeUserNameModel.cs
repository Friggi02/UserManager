namespace Project.WebApi.DTO.Input
{
    public class ChangeUserNameModel
    {
        public required string CurrentPassword { get; set; }
        public required string NewUserName { get; set; }
    }
}