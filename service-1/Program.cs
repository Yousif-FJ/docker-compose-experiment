using Microsoft.AspNetCore.RateLimiting;
using service_1.Database;
using service_1.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.Configure<DbConfig>(
    builder.Configuration.GetSection("Database"));

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


//My endpoints:
app.MapServerInfoEndpoint()
    .RequireRateLimiting(RateLimiterPolicyName);
app.MapLoginEndpoint();
app.MapGetStateEndpoint();


app.Run();
