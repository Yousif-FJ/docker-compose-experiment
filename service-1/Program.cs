using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using service_1;
using service_1.Endpoints;

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

app.MapServerInfoEndpoint()
    .RequireRateLimiting(RateLimiterPolicyName);

app.Run();
