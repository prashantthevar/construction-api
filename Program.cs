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

// Configure MongoDB Settings from environment variables
builder.Services.Configure<MongoDBSettings>(options =>
{
    options.ConnectionString = "mongodb://mongo:wcoejUokOtszAnyAmNLggJkkWAEfbVWz@autorack.proxy.rlwy.net:10353";
    options.DatabaseName = "test";
});

// Register MongoDB Settings as a singleton service
builder.Services.AddSingleton<IMongoDBSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

// Register MongoClient as a singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient("mongodb://mongo:wcoejUokOtszAnyAmNLggJkkWAEfbVWz@autorack.proxy.rlwy.net:10353");
});

// Register IMongoDatabase
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IMongoDBSettings>();
    return mongoClient.GetDatabase("test"); // Get the database from the client
});

// Register Swagger services
builder.Services.AddSwaggerGen(options =>
{
    // Optional: Configure additional Swagger options (e.g., API info, etc.)
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

var app = builder.Build();

// Enable Swagger UI in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Generates Swagger JSON
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Construction Management API V1");
    });
}

// Use HTTPS redirection only in non-production environments
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Seed roles when the app starts
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedRolesAsync(); // This will seed the roles into the MongoDB database
}

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();

// Port binding for Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "3000"; // Default to 3000 if PORT is not set
app.Urls.Add($"http://*:{port}");

app.Run();
