using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();


app.MapGet("/", async ([FromServices] IHttpClientFactory httpClientFactory) =>
{
    using var httpClient = httpClientFactory.CreateClient();

    httpClient.BaseAddress = new Uri("http://service-2:3000");

    try
    {
        var result = await httpClient.GetAsync("/");
        var message = await result.Content.ReadAsStringAsync();
        return $"Got response : {message}";
    }
    catch
    {
        return "failed to get from the service";
    }
});

app.Run();
