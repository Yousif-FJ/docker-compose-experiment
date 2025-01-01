using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using service_1.Database;

namespace service_1.Endpoints;

internal static class GetState
{
    public static RouteHandlerBuilder MapGetStateEndpoint(this WebApplication app)
    {
        return app.MapGet("/state", GetStateHandler);
    }

    public static async Task<IResult> GetStateHandler([FromServices] IOptions<DbConfig> dbConfig)
    {
        var stateCollection = dbConfig.Value.GetStateCollectionFromDb();

        var currentState = await stateCollection.Find(_ => true).FirstOrDefaultAsync()
             ?? new State { CurrentAppState = AppState.INIT };

        return Results.Text(content: currentState.CurrentAppState.ToString(), statusCode: 200);
    }
}