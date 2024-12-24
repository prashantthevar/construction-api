using ConstructionManagementAPI.Repositories;
using ConstructionManagementSaaS;
using ConstructionManagementSaaS.Data;
using ConstructionManagementSaaS.Repositories;
using ConstructionManagementSaaS.Services;
using ConstructionManagementService.Services;
using ConstructionManagementService.Services.Contracts;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Bind MongoDB settings from appsettings.json and allow environment variable overrides
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<IMongoDBSettings>(sp =>
{
    var config = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    config.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? config.ConnectionString;
    config.DatabaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE") ?? config.DatabaseName;
    return config;
});

// Register MongoClient as a singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IMongoDBSettings>();
    return new MongoClient(settings.ConnectionString);
});

// Register IMongoDatabase
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IMongoDBSettings>();
    return mongoClient.GetDatabase(settings.DatabaseName);
});

// Register Swagger services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Construction Management API",
        Version = "v1"
    });
});

// Register other services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddSingleton<RoleSeeder>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Enable Swagger UI in development and production environments
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Construction Management API V1");
    });
}

// Seed roles when the app starts
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedRolesAsync();
}

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Configure to listen on Railway-assigned PORT or default to 3000
var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
app.Run($"http://0.0.0.0:{port}");
