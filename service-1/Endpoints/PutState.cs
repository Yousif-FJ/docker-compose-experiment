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
                return Results.Text("Invalid state parameter", statusCode: 400);
            }
        }
        catch
        {
            return Results.Text("Invalid state parameter", statusCode: 400);
        }

        if (currentStateEntry.CurrentAppState == newState)
        {
            return Results.Text(newState.ToString(), statusCode: 200);
        }

        if (currentStateEntry.CurrentAppState == AppState.INIT)
        {
            return Results.Text("Cannot change state from INIT without Login", statusCode: 400);
        }

        if (newState == AppState.SHUTDOWN)
        {
            return Results.Text("Shutdown is not implemented", statusCode: 501);
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

        return Results.Text(newState.ToString(), statusCode: 200);
    }
}