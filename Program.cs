using System;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using FifaPollApi.Data;
using FifaPollApi.Domain.Repositories;
using FifaPollApi.DTOs.Auth;
using FifaPollApi.DTOs.Team;
using FifaPollApi.Mappings;
using FifaPollApi.Middleware;
using FifaPollApi.Repositories;
using FifaPollApi.Security;
using FifaPollApi.Services;
using FifaPollApi.Validators;

// Setup Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting FIFA World Cup Polling Web API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllers();

    // Database Connection
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=localhost;Database=fifapoll;User=root;Password=RootPassword123!;";
    
    builder.Services.AddDbContext<FifaPollDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    // CORS Configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // JWT Authentication
    var keyStr = builder.Configuration["JwtSettings:Key"] ?? "super_secret_fifa_world_cup_polling_secret_key_2026";
    var key = Encoding.UTF8.GetBytes(keyStr);
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "FifaPollApi",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "FifaPollUi",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    // Register Repositories & Unit Of Work
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ITeamRepository, TeamRepository>();
    builder.Services.AddScoped<IVoteRepository, VoteRepository>();
    builder.Services.AddScoped<ISettingRepository, SettingRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Register Services
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITeamService, TeamService>();
    builder.Services.AddScoped<IVoteService, VoteService>();
    builder.Services.AddScoped<ISettingService, SettingService>();

    // Register Utilities
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

    // Register AutoMapper
    builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

    // Register FluentValidation
    builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
    builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
    builder.Services.AddScoped<IValidator<CreateTeamDto>, CreateTeamDtoValidator>();

    // Configure Swagger with JWT Support
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "FIFA World Cup Polling API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    var app = builder.Build();

    // Database Seeding and Initialization
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<FifaPollDbContext>();
            var passwordHasher = services.GetRequiredService<IPasswordHasher>();
            await DbInitializer.SeedAsync(context, passwordHasher);
            Log.Information("Database successfully initialized and seeded.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred during database seeding and migration.");
        }
    }

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        // In production, also expose Swagger for testing as per instruction
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Global Exception Handling Middleware
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
