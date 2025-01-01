using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using service_1.Database;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace service_1.Endpoints;

internal static class ServerInfoEndpoint
{
    public static RouteHandlerBuilder MapServerInfoEndpoint(this WebApplication app)
    {
        return app.MapGet("/request", GetServerInfoHandler);
    }

    public static async Task<IResult> GetServerInfoHandler(
        [FromServices] IOptions<DbConfig> dbConfig,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] ILogger<Program> logger)
    {
        var stateCollection = dbConfig.Value.GetStateCollectionFromDb();
        
        var currentState = await stateCollection.Find(_ => true).FirstOrDefaultAsync();
        
        if (currentState.CurrentAppState == AppState.PAUSED)
        {
            return Results.Text("Service is PAUSED",statusCode: 503);
        }

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

            var resultTest =  
            $"""
            System Info 1: 
            {systemInfo1}

            System Info 2:
            {systemInfo2}
            """;

            return Results.Text(resultTest);
        }
        catch (Exception e)
        {
            logger.LogError(message: "Error while requesting service_2", exception: e);
            return Results.Text("failed to get response from the service-2", statusCode: 500);
        }
    }
}

public static class Utils
{
    public static SystemInformation ExtractSystemInfo()
    {
        var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .Select(ip => ip.ToString()).ToList();

        var processes = Process.GetProcesses().Select(p => p.ProcessName).ToList();
        long freeBytes;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            freeBytes = new DriveInfo("/").AvailableFreeSpace;
        }
        else
        {
            throw new NotImplementedException("None Linux OS cases were not implemented");
        }
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);

        var systemInformation = new SystemInformation(ip, processes, uptime.ToString(),
                                                        freeBytes.ToString());
        return systemInformation;
    }
}

public record SystemInformation(List<string> Ips, List<string> Processes, string Uptime, string FreeSpace)
{
    public override string ToString()
    {
        return 
        $"""
        IP: {string.Join(", ", Ips)}
        Processes: {string.Join(", ", Processes)}
        Uptime: {Uptime}
        FreeSpace: {FreeSpace}
        """;
    }
};
