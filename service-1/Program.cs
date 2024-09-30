using Microsoft.AspNetCore.Mvc;
using service_1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();


app.MapGet("/", async ([FromServices] IHttpClientFactory httpClientFactory) =>
{
    using var httpClient = httpClientFactory.CreateClient();

    SystemInformation systemInformation = Utils.ExtractSystemInfo();

    httpClient.BaseAddress = new Uri("http://service-2:3000");

    try
    {
        var result = await httpClient.GetAsync("/");
        var message = await result.Content.ReadAsStringAsync();
        return Results.Ok(systemInformation);
    }
    catch
    {
        return Results.Problem("failed to get response from the service-2");
    }
});

app.Run();

