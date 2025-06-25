using EmailIntelligence.API;
using EmailIntelligence.Application;
using EmailIntelligence.Infrastructure;
using EmailIntelligence.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

// Initialize database with seed data
await app.InitializeDatabaseAsync();

// Configure the HTTP request pipeline.
app.UseApiServices();

app.Run();
