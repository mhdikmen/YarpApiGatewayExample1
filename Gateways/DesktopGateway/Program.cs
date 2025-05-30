using BuildingBlocks.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.AddAuthorizationServices();

var app = builder.Build();
app.UseAuthorizationServices();

app.MapReverseProxy();
await app.RunAsync();
