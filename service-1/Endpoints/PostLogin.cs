using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using service_1.Database;

namespace service_1.Endpoints;

internal static class LoginEndpoint
{
    public static RouteHandlerBuilder MapLoginEndpoint(this WebApplication app)
    {
        return app.MapPost("/login", ChangeStateToLoggedIn);
    }

    public static async Task<IResult> ChangeStateToLoggedIn(
        [FromServices] IOptions<DbConfig> dbConfig,
        [FromServices] ILogger<Program> logger)
    {
        var mongoClient = new MongoClient(dbConfig.Value.ConnectionString);

        var myDb = mongoClient.GetDatabase(dbConfig.Value.DatabaseName);

        var stateCollection = myDb.GetCollection<State>(dbConfig.Value.StateCollectionName);

        var currentStateEntry = await stateCollection.Find(_ => true).FirstOrDefaultAsync();

        if (currentStateEntry is null)
        {
            currentStateEntry = new State(){ CurrentAppState = AppState.INIT};
            await stateCollection.InsertOneAsync(currentStateEntry);
        }

        if (currentStateEntry.CurrentAppState == AppState.PAUSED)
        {
            return Results.Text("Service is PAUSED",statusCode: 503);
        }

        if (currentStateEntry.CurrentAppState == AppState.RUNNING)
        {
            return Results.Ok();
        }

        var newState = AppState.RUNNING;

        var LogEntry = new LogEntry
        { 
            DateTime = DateTime.UtcNow,
            Description = $"{currentStateEntry.CurrentAppState}->{newState}"
        };

        currentStateEntry.CurrentAppState = newState;
        var result = await stateCollection.ReplaceOneAsync(_ => true, currentStateEntry);
        logger.LogInformation("Updated state, ModifiedCount: {ModifiedCount}", result.ModifiedCount);
        
        var runLogCollection = myDb.GetCollection<LogEntry>(dbConfig.Value.LogEntryCollectionName);
        await runLogCollection.InsertOneAsync(LogEntry);

        return Results.Ok();
    }
}