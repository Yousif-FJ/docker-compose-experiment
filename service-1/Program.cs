using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using service_1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

const string RateLimiterPolicyName = "RateLimiter";

builder.Services.AddRateLimiter(config => 
    config.AddFixedWindowLimiter(RateLimiterPolicyName, option => { 
        option.PermitLimit = 1;
        option.QueueLimit = 100;
        option.Window = TimeSpan.FromSeconds(2);
        option.AutoReplenishment = true;
    }));

var app = builder.Build();

app.UseRateLimiter();

app.MapGet("/", async ([FromServices] IHttpClientFactory httpClientFactory,
                        ILogger<Program> logger) =>
{
    using var httpClient = httpClientFactory.CreateClient();

    SystemInformation systemInfo1 = Utils.ExtractSystemInfo();

    httpClient.BaseAddress = new Uri("http://service-2:3000");

    try
    {
        var response = await httpClient.GetAsync("/");
        var systemInfo2 = await response.Content.ReadFromJsonAsync<SystemInformation>();
        if (systemInfo2 is null)
        {
            var result = await response.Content.ReadAsStringAsync();
            throw new Exception($"Unexpected response from service 2, Response was : {result}");
        }

        SystemInformation[] resultArray = [systemInfo1, systemInfo2];

        return Results.Ok(resultArray);
    }
    catch (Exception e)
    {
        logger.LogError(message: "Error while requesting service_2", exception: e);
        return Results.Problem("failed to get response from the service-2");
    }
})
.RequireRateLimiting(RateLimiterPolicyName);

app.Run();
