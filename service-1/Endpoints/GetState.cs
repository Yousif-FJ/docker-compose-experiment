
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using service_1.Database;

internal static class GetState
{
    public static RouteHandlerBuilder MapGetStateEndpoint(this WebApplication app)
    {
        return app.MapGet("/state", GetStateHandler);
    }

    public static async Task<IResult> GetStateHandler([FromServices] IOptions<DbConfig> dbConfig)
    {
        var mongoClient = new MongoClient(dbConfig.Value.ConnectionString);

        var myDb = mongoClient.GetDatabase(dbConfig.Value.DatabaseName);

        var stateCollection = myDb.GetCollection<State>(dbConfig.Value.StateCollectionName);

        var currentState = await stateCollection.Find(_ => true).FirstOrDefaultAsync()
             ?? new State{ CurrentAppState = AppState.Init };
         
        return Results.Text(content: currentState.CurrentAppState.ToString().ToUpper(),  statusCode: 200);
    }
}