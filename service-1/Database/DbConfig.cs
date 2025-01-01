using MongoDB.Driver;

namespace service_1.Database;

public class DbConfig
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;

    public string StateCollectionName {get; set;} = null!; 
    public string LogEntryCollectionName { get; set; } = null!;
}

public static class DbUtil {
    public static IMongoCollection<State> GetStateCollectionFromDb(this DbConfig dbConfig){
        
        var mongoClient = new MongoClient(dbConfig.ConnectionString);

        var myDb = mongoClient.GetDatabase(dbConfig.DatabaseName);

        var stateCollection = myDb.GetCollection<State>(dbConfig.StateCollectionName);

        return stateCollection;
    }
}