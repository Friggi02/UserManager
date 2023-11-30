namespace Project.WebApi.DTO.Input
{
    public class ChangePasswordModel
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}