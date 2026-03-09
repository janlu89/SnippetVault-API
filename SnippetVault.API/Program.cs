using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using SnippetVault.API.Extensions;
using SnippetVault.Application.Interfaces;
using SnippetVault.Application.Services;
using SnippetVault.Application.Settings;
using SnippetVault.Infrastructure.Data;
using SnippetVault.Infrastructure.Repositories;
using SnippetVault.Infrastructure.Services;
using System.Text;

// Configure Serilog before the host is built.
// This is important — if the host itself fails to start, you still want logs.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/snippetvault-.log",   // The dash tells Serilog to append the date here
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Replace the default Microsoft logging with Serilog
builder.Host.UseSerilog();

//CORS Fix
builder.Services.AddCorsPolicy(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// Database
// Use SQLite in development for simplicity, PostgreSQL in production.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// JWT Settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ISnippetService, SnippetService>();
builder.Services.AddScoped<ISnippetRepository, SnippetRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

var app = builder.Build();

// ExceptionMiddleware must be FIRST so it wraps the entire pipeline
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply pending EF Core migrations automatically on container startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

// Ensures all buffered log entries are flushed to disk on shutdown
Log.CloseAndFlush();