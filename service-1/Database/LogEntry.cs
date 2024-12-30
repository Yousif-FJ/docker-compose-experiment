using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace service_1.Database;

public class LogEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string? Description { get; set; }
}