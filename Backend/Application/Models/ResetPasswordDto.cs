namespace Final_year_Project.Application.Models
{
    public class ResetPasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}