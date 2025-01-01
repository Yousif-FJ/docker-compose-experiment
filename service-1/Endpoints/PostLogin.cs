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

    public static async Task<IResult> ChangeStateToLoggedIn([FromServices] IOptions<DbConfig> dbConfig)
    {
        var stateCollection = dbConfig.Value.GetStateCollectionFromDb();

        var currentState = await stateCollection.Find(_ => true).FirstOrDefaultAsync();

        if (currentState is null)
        {
            currentState = new State() { CurrentAppState = AppState.RUNNING };
            await stateCollection.InsertOneAsync(currentState);
        }
        else
        {
            currentState.CurrentAppState = AppState.RUNNING;
            await stateCollection.ReplaceOneAsync(x => x.Id == currentState.Id, currentState);
        }
        return Results.Ok();
    }
}