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
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000"; // Default to 5000 if not set

builder.WebHost.UseUrls($"http://*:{port}");


// Configure MongoDB Settings from environment variables
builder.Services.Configure<MongoDBSettings>(options =>
{
    options.ConnectionString = "mongodb://mongo:wPgUiQEJbOSeHwsYRiCpVnFWnUDHMQIg@autorack.proxy.rlwy.net:58537";
    options.DatabaseName = "construction-dev";
});

// Register MongoDB Settings as a singleton service
builder.Services.AddSingleton<IMongoDBSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

// Register MongoClient as a singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient("mongodb://mongo:wPgUiQEJbOSeHwsYRiCpVnFWnUDHMQIg@autorack.proxy.rlwy.net:58537");
});

// Register IMongoDatabase
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IMongoDBSettings>();
    return mongoClient.GetDatabase("construction-dev"); // Get the database from the client
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

// Seed roles when the app starts
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedRolesAsync(); // This will seed the roles into the MongoDB database
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
