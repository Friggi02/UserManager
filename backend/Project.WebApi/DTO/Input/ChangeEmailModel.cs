namespace Project.WebApi.DTO.Input
{
    public class ChangeEmailModel
    {
        public required string CurrentPassword { get; set; }
        public required string NewEmail { get; set; }
    }
}