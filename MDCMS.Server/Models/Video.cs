using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MDCMS.Server.Models
{
    public class Video
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = null!; // filename of video

        [BsonElement("preview")]
        public string Preview { get; set; } = ""; // preview image or thumbnail URL

        [BsonElement("duration")]
        public int Duration { get; set; } // in seconds

        [BsonElement("dateStart")]
        public DateTime DateStart { get; set; }

        [BsonElement("dateEnd")]
        public DateTime DateEnd { get; set; }

        [BsonElement("timeStart")]
        public TimeSpan TimeStart { get; set; }

        [BsonElement("timeEnd")]
        public TimeSpan TimeEnd { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Active"; // Active / Inactive

        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; } = DateTime.UtcNow;
    }
}
