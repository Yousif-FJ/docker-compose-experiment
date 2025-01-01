using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using service_1.Database;

namespace service_1.Endpoints;

internal static class PutState
{
    public static RouteHandlerBuilder MapPutState(this WebApplication app)
    {
        return app.MapPut("/state", PutStateHandler);
    }

    public static async Task<IResult> PutStateHandler(
        HttpRequest request,
        [FromServices] IOptions<DbConfig> dbConfig)
    {
        var mongoClient = new MongoClient(dbConfig.Value.ConnectionString);

        var myDb = mongoClient.GetDatabase(dbConfig.Value.DatabaseName);

        var stateCollection = myDb.GetCollection<State>(dbConfig.Value.StateCollectionName);

        var currentStateEntry = await stateCollection.Find(_ => true).FirstOrDefaultAsync() ??
            new State(){ CurrentAppState = AppState.INIT};

        AppState newState;
        try
        {
            using var streamReader = new StreamReader(request.Body, Encoding.UTF8);
            var newStateString = await streamReader.ReadToEndAsync();
            if (!Enum.TryParse(newStateString, out newState))
            {
                return Results.BadRequest("Invalid state parameter");
            }
        }
        catch
        {
            return Results.BadRequest("Invalid state parameter");
        }


        if (newState == AppState.SHUTDOWN)
        {
            return Results.Problem("Shutdown is not implemented", statusCode: 501);
        }

        var LogEntry = new LogEntry
        { 
            DateTime = DateTime.UtcNow,
            Description = $"{currentStateEntry.CurrentAppState}->{newState}"
        };

        currentStateEntry.CurrentAppState = newState;
        await stateCollection.ReplaceOneAsync(_ => true, currentStateEntry);
        
        var runLogCollection = myDb.GetCollection<LogEntry>(dbConfig.Value.LogEntryCollectionName);
        await runLogCollection.InsertOneAsync(LogEntry);

        return Results.Ok();
    }
}