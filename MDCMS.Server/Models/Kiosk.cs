using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MDCMS.Server.Models;

public class Kiosk
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("kioskCode")]
    public string KioskCode { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("type")]
    public string Type { get; set; } = null!;

    [BsonElement("logo")]
    public string Logo { get; set; } = "";

    [BsonElement("level")]
    public string Level { get; set; } = null!;
    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new List<string>();

    [BsonElement("dateModified")]
    public DateTime DateModified { get; set; } = DateTime.UtcNow;
}
