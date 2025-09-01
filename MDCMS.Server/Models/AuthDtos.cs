namespace MDCMS.Server.Models
{
    public class AuthDtos
    {
        public record RegisterRequest(string Username, string Name, string Email, string Password, string Designation);
        public record LoginRequest(string Username, string Password);
        public record LoginResponse(string Token, DateTime Expires);

        // New DTOs
        public record UpdateUserRequest(string Name, string Email, string Designation, bool IsActive, List<string> AllowedPages);
        public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
    }
}
