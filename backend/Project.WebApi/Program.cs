using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Project.Dal;
using Project.Dal.Entities;
using Project.Dal.Jwt;
using Project.Dal.Permit;
using Project.Dal.Repositories;
using Project.Dal.Repositories.Interfaces;
using Project.WebApi;

// Create a new instance of the web application builder
var builder = WebApplication.CreateBuilder(args);

// Register the necessary services
string defaultCors = "defaultCors";
builder.Services.AddCors(options =>
    options.AddPolicy(name: defaultCors, policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    })
);
//builder.Services.AddControllers().AddNewtonsoftJson();
ConfigurationManager configuration = builder.Configuration;

var modelBuilder = new ODataConventionModelBuilder();

modelBuilder.EntitySet<User>("User");
var EntityType = modelBuilder.EntityType<User>();
EntityType.Ignore(ui => ui.NormalizedUserName);
EntityType.Ignore(ui => ui.NormalizedEmail);
EntityType.Ignore(ui => ui.EmailConfirmed);
EntityType.Ignore(ui => ui.PasswordHash);
EntityType.Ignore(ui => ui.SecurityStamp);
EntityType.Ignore(ui => ui.ConcurrencyStamp);
EntityType.Ignore(ui => ui.PhoneNumber);
EntityType.Ignore(ui => ui.TwoFactorEnabled);
EntityType.Ignore(ui => ui.PhoneNumberConfirmed);
EntityType.Ignore(ui => ui.LockoutEnabled);
EntityType.Ignore(ui => ui.RefreshToken);

modelBuilder.EnableLowerCamelCase();

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
}).AddNewtonsoftJson().AddOData(
    options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
        "odata",
        modelBuilder.GetEdmModel()));

// Get the connection string from the appsettings.json file
string? connStr = builder.Configuration.GetConnectionString("Default");

// Add a SQL Server database context to the service collection
builder.Services.AddSqlServer<ProjectDbContext>(connStr);

// Add Identity services for authentication and authorization
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; // Optional, enforce unique emails
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Specify allowed characters for usernames
})
.AddEntityFrameworkStores<ProjectDbContext>()
.AddDefaultTokenProviders();

// Adding services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

// Add Swagger and Swagger UI to the service collection
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    // Generate the default UI of Swagger documentation
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ASP.NET 7 Web API",
        Description = "Authentication and Authorization in ASP.NET 7 with JWT and Swagger"
    });

    // Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Build the web application
var app = builder.Build();

// Use Swagger and SwaggerUI in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET 7 Web API v1"));
}

// Apply any pending migrations to the database upon scope creation
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetService<ProjectDbContext>();
    ctx?.Database.Migrate();
}

// Add necessary middleware
app.UseHttpsRedirection();
app.UseCors(defaultCors);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//app.UseRouting();
//app.UseEndpoints(endpoints => endpoints.MapControllers());
app.Run();