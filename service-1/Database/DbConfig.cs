namespace service_1.Database;

public class DbConfig
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;

    public string StateCollectionName {get; set;} = null!; 
    public string LogEntryCollectionName { get; set; } = null!;
}