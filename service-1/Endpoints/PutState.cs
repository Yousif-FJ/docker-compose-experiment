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
        var stateCollection = dbConfig.Value.GetStateCollectionFromDb();

        var currentState = await stateCollection.Find(_ => true).FirstOrDefaultAsync();

        string? newState = null;
        try
        {
            using var streamReader = new StreamReader(request.Body, Encoding.UTF8);
            newState = await streamReader.ReadToEndAsync();
        }
        catch
        {
            return Results.BadRequest("Invalid state parameter");
        }

        if (!Enum.TryParse(newState, out AppState newAppState))
        {
            return Results.BadRequest("Invalid state parameter");
        }

        if (currentState is null)
        {
            currentState = new State() { CurrentAppState = newAppState };
            await stateCollection.InsertOneAsync(currentState);
        }
        else
        {
            currentState.CurrentAppState = newAppState;
            await stateCollection.ReplaceOneAsync(x => x.Id == currentState.Id, currentState);
        }
        return Results.Ok();
    }
}