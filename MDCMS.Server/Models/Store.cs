using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MDCMS.Server.Models
{
    public class Store
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }  // MongoDB ObjectId

        [BsonElement("storeId")]
        public int StoreId { get; set; } // Not editable, unique identifier

        [BsonElement("storeName")]
        public string StoreName { get; set; } = string.Empty;

        [BsonElement("storeType")]
        public string StoreType { get; set; } = "Others"; // Dining, Shopping, Services, Others

        [BsonElement("storeLevel")]
        public int StoreLevel { get; set; } // Not editable

        [BsonElement("storeLogo")]
        public string StoreLogo { get; set; } = "default-logo.png";

        [BsonElement("storeTags")]
        public List<string> StoreTags { get; set; } = new List<string>();

        [BsonElement("isVacant")]
        public bool IsVacant { get; set; }

        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; } = DateTime.UtcNow;
    }
}
