using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Roulette.Application;
using Roulette.Domain.Contracts.Security;
using Roulette.Infrastructure.Data;
using Roulette.Infrastructure.Data.Core;
using Roulette.Infrastructure.Redis;
using Roulette.Infrastructure.Redis.Settings;
using Roulette.Infrastructure.Security.Services;
using Roulette.Infrastructure.Security.Settings;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Habilitar colores en consola
#region Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Roulette.API")
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Logging.ClearProviders();
#endregion

// Security Service
#region Security
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!))
    };
});
#endregion

#region Data Protection
var keysPath = Path.Combine("/app", "DataProtection-Keys");
Directory.CreateDirectory(keysPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("Roulette.API");
#endregion

// Redis Configuration
#region Redis
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<RedisSettings>>().Value);

// Register Redis Service
builder.Services.AddRedisServices();
#endregion

// Database Context
SQLitePCL.Batteries_V2.Init();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var encryptionPassword = builder.Configuration["Database:EncryptionPassword"];

builder.Services.AddDbContext<RouletteDbContext>(option => {
    option.UseSqlite(connectionString);

    if (!string.IsNullOrEmpty(encryptionPassword))
    {
        option.AddInterceptors(new SqliteCipherConnectionInterceptor(encryptionPassword));
    }
});


// Register Data Services
builder.Services.AddDataServices();

// Register Services
builder.Services.AddApplicationServices();

#region Swagger
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Roulette API", Version = "v1" });

    options.SwaggerDoc("Auth", new OpenApiInfo { Title = "Authentication API", Version = "v1" });
    options.SwaggerDoc("Roulette", new OpenApiInfo { Title = "Roulette API", Version = "v1" });
    options.SwaggerDoc("Bet", new OpenApiInfo { Title = "Bet API", Version = "v1" });
    options.SwaggerDoc("Gambler", new OpenApiInfo { Title = "Gambler API", Version = "v1" });

    options.TagActionsBy(api => [api.GroupName ?? "v1"]);

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var _))
            return false;

        var groupName = apiDesc.GroupName ?? "v1";
        return string.Equals(groupName, docName, StringComparison.OrdinalIgnoreCase);
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
#endregion

var app = builder.Build();

#region Serilog
app.Use(async (context, next) =>
{
    var userName = context.User?.Identity?.IsAuthenticated == true
        ? context.User.Identity.Name ?? context.User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value
        : "Anonymous";

    LogContext.PushProperty("User", userName);
    await next();
});

// Middleware Serilog para requests HTTP
app.UseSerilogRequestLogging(options =>
{
    // Nivel de log según código HTTP
    options.GetLevel = (httpContext, _, ex) =>
    {
        if (ex != null || httpContext.Response.StatusCode >= 500)
            return LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 400)
            return LogEventLevel.Warning;
        return LogEventLevel.Information;
    };

    // Mensaje de salida personalizado
    options.MessageTemplate =
        "Handled {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms (User: {User})";
});
#endregion

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RouletteDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/Auth/swagger.json", "Authentication API");
        options.SwaggerEndpoint("/swagger/Roulette/swagger.json", "Roulette API");
        options.SwaggerEndpoint("/swagger/Bet/swagger.json", "Bet API");
        options.SwaggerEndpoint("/swagger/Gambler/swagger.json", "Gambler API");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting Roulette.API...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly!");
}
finally
{
    Log.Information("Stopping Roulette.API...");
    await Log.CloseAndFlushAsync();
}

public partial class Program
{
    protected Program() { }
}
