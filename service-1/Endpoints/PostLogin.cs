namespace service_1.Endpoints;

internal static class LoginEndpoint
{
    public static RouteHandlerBuilder MapLoginEndpoint(this WebApplication app){
        return app.MapPost("/login", ChangeStateToLoggedIn);
    }

    public static async Task<IResult> ChangeStateToLoggedIn(){
        //TODO implement state
        
        return Results.Ok();
    }
}