using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using service_1.Database;

namespace service_1.Endpoints;

internal static class GetRunLog
{
    public static RouteHandlerBuilder MapGetRunLog(this WebApplication app)
    {
        return app.MapGet("/run-log", GetRunLogHandler);
    }

    public static async Task<IResult> GetRunLogHandler([FromServices] IOptions<DbConfig> dbConfig)
    {
        var mongoClient = new MongoClient(dbConfig.Value.ConnectionString);
        var myDb = mongoClient.GetDatabase(dbConfig.Value.DatabaseName);
        var logCollection = myDb.GetCollection<LogEntry>(dbConfig.Value.LogEntryCollectionName);

        var runLogs = await logCollection.Find(_ => true).ToListAsync();

        var runLogsText = string.Join("\n", runLogs.Select(x => x.ToString()));

        return Results.Text(runLogsText);
    }
}