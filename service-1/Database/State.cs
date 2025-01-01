using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace service_1.Database;

public class State{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public AppState CurrentAppState { get; set; }
}

public enum AppState
{
    INIT, 
    PAUSED,
    RUNNING,
    SHUTDOWN
}