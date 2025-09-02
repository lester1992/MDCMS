namespace MDCMS.Client.Models;

public class User
{
    public string? Id { get; set; }
    public string Username { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Designation { get; set; } = "";
    public bool IsActive { get; set; } = false;
    public List<string> AllowedPages { get; set; } = new List<string>();
    public string PasswordHash { get; set; } = null!;
    public DateTime DateModified { get; set; } = DateTime.UtcNow;
}
