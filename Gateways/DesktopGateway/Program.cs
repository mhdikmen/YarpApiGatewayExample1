using BuildingBlocks.Extentions;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.AddAuthorizationServices();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy<string>("AuthenticatedLimiter", context =>
    {
        var username = context.User.Identity?.Name ?? "anonymous";
        return RateLimitPartition.GetTokenBucketLimiter(username, key => new()
        {
            TokenLimit = 10,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 1,
            AutoReplenishment = true
        });
    });

    options.AddPolicy<string>("AnonymousLimiter", _ =>
    {
        return RateLimitPartition.GetFixedWindowLimiter("anonymous", _ => new()
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });
});

var app = builder.Build();
app.UseAuthorizationServices();

app.UseRateLimiter();
app.MapReverseProxy();


await app.RunAsync();
