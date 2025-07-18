// -----------------------------------------------------------------------------
// School Management System - ASP.NET Core Web API Entry Point
// This file configures services, authentication, middleware, and endpoint routing
// -----------------------------------------------------------------------------

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Services;
using SchoolApi.Middleware;
using SchoolApi.Hubs;
using System.Text.Json.Serialization;
using Serilog;
using Serilog.Events;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured logging to console and file
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("Logs/school-api-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

// Add controllers and configure JSON options (e.g., ignore cycles)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "School Management System API",
        Version = "v1",
        Description = "A comprehensive REST API for managing school operations including students, teachers, courses, grades, and attendance.",
        Contact = new OpenApiContact
        {
            Name = "School Management Team",
            Email = "support@schoolmanagement.com"
        }
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Custom schema IDs prevent name conflicts in OpenAPI spec
    options.CustomSchemaIds(type =>
    {
        if (type.IsGenericType)
        {
            var genericTypeName = type.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
            var genericArgs = string.Join("And", type.GetGenericArguments().Select(t => t.Name));
            return $"{type.Namespace}.{genericTypeName}Of{genericArgs}";
        }
        return $"{type.Namespace}.{type.Name}";
    });
});

// Configure CORS to allow frontend (React) to access the API securely
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:5173") // React app port (Vite default)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure ASP.NET Core Identity for user management and authentication
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password, lockout, and user settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// -----------------------------------------------------------------------------
// JWT Authentication Setup
// -----------------------------------------------------------------------------
// Configures JWT Bearer authentication and tells ASP.NET Core which claim to use
// for user roles. This is critical for [Authorize(Roles = "...")] to work.
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured"))),
        // IMPORTANT: Map the role claim so [Authorize(Roles = "Admin")] works
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    };
});

// Register application services for dependency injection
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Add SignalR for real-time notifications
builder.Services.AddSignalR();

// Add in-memory caching for performance
builder.Services.AddMemoryCache();

var app = builder.Build();

// -----------------------------------------------------------------------------
// Middleware Pipeline Order (IMPORTANT)
// -----------------------------------------------------------------------------
// 1. UseSwagger/UseSwaggerUI: Enable API docs in development
// 2. UseCors: Allow frontend to access API
// 3. UseMiddleware<GlobalExceptionHandler>: Consistent error responses
// 4. UseHttpsRedirection: Redirect HTTP to HTTPS
// 5. UseAuthentication: Validate JWT tokens and set user context
// 6. Custom logging middleware: Log 401/403 with claims for debugging
// 7. UseAuthorization: Enforce [Authorize] attributes on controllers/actions
// 8. MapControllers: Route API requests to controllers

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "School Management API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at root
    });
}

app.UseCors("AllowFrontend");

// Global exception handler for consistent error responses
app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

// Authenticate JWT tokens and set HttpContext.User
app.UseAuthentication();

// Log 401/403 responses with user claims and request path for easier debugging
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
    {
        var user = context.User;
        var claims = user?.Claims?.Select(c => $"{c.Type}: {c.Value}");
        var logClaims = claims != null ? string.Join(", ", claims) : "No claims";
        var path = context.Request.Path;
        // Use Serilog for logging
        Log.Warning($"[{DateTime.Now}] {context.Response.StatusCode} at {path}. Claims: {logClaims}");
    }
});

// Enforce [Authorize] attributes on controllers/actions
app.UseAuthorization();

// Map attribute-routed controllers (e.g., [Route("api/[controller]")])
app.MapControllers();

// -----------------------------------------------------------------------------
// Controller Authorization Example:
// [Authorize(Roles = "Admin")] on a controller or action restricts access to
// users whose JWT contains the "Admin" role claim. This is enforced by the
// authentication and authorization middleware configured above.
// -----------------------------------------------------------------------------

// Map SignalR Hub
app.MapHub<NotificationHub>("/notificationHub");

// Seed data
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DataSeeder.SeedData(scope.ServiceProvider);
        Log.Information("Data seeding completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding data");
    }
}

app.Run();
