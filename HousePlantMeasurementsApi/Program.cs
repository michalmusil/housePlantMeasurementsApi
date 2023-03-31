using HouseDeviceMeasurementsApi.Repositories.Devices;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Repositories.Measurements;
using HousePlantMeasurementsApi.Repositories.PlantNotes;
using HousePlantMeasurementsApi.Repositories.Plants;
using HousePlantMeasurementsApi.Repositories.Users;
using HousePlantMeasurementsApi.Services.AuthService;
using HousePlantMeasurementsApi.Services.FCMService;
using HousePlantMeasurementsApi.Services.ImageService;
using HousePlantMeasurementsApi.Services.ValidationHelperService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;
using System.Text.Json;

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
builder.Services.AddSingleton<IFCMService, FCMService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IMeasurementValidator, MeasurementValidator>();
builder.Services.AddScoped<IUsersRepository, UsersDbRepository>();
builder.Services.AddScoped<IPlantsRepository, PlantsDbRepository>();
builder.Services.AddScoped<IPlantNotesRepository, PlantNotesRepository>();
builder.Services.AddScoped<IDevicesRepository, DevicesDbRepository>();
builder.Services.AddScoped<IMeasurementsRepository, MeasurementsDbRepository>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseExceptionHandler((errorApp) =>
    {
        errorApp.Run(async (context) =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
            var message = JsonSerializer.Serialize(new { message = exceptionHandler.Error.Message.ToString() });
            await context.Response.WriteAsync(message);
        });
    });
}
else
{
    app.UseExceptionHandler((errorApp) =>
    {
        errorApp.Run(async (context) =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
            var message = JsonSerializer.Serialize(new { message = "Something went wrong on the server" });
            await context.Response.WriteAsync(message);
        });
    });
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Adding custom exception handling for when there is an internal error


app.Run();

