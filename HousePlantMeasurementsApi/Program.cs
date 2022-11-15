using HouseDeviceMeasurementsApi.Repositories.Devices;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Repositories.Measurements;
using HousePlantMeasurementsApi.Repositories.Plants;
using HousePlantMeasurementsApi.Repositories.Users;
using HousePlantMeasurementsApi.Services.AuthService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Setting up Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", securityScheme: new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Description = "Enter Bearer {your generated token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
           new string[] {}
        }
    });
});


// Authentication with JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();


// Add EF db context
builder.Services.AddDbContext<PlantMeasurementsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PlantMeasurementsDatabase"));
});

// Add Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



// Dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsersRepository, UsersDbRepository>();
builder.Services.AddScoped<IPlantsRepository, PlantsDbRepository>();
builder.Services.AddScoped<IDevicesRepository, DevicesDbRepository>();
builder.Services.AddScoped<IMeasurementsRepository, MeasurementsDbRepository>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

