using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MDCMS.Server.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    // designation stored as role-like string (e.g., "Admin" or "Manager")
    [BsonElement("designation")]
    public string Designation { get; set; } = "";
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = false;

    [BsonElement("allowedPages")]
    public List<string> AllowedPages { get; set; } = new List<string>();

    // password hash + salt combined (we'll format as base64: salt:hash)
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;

    [BsonElement("dateModified")]
    public DateTime DateModified { get; set; } = DateTime.UtcNow;
}
