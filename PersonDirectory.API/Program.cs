using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PersonDirectory.API.Configurations;
using PersonDirectory.API.Middlewares;
using PersonDirectory.Application.DTOs;
using PersonDirectory.Application.Interfaces;
using PersonDirectory.Application.Services;
using PersonDirectory.Application.Validators;
using PersonDirectory.Infrastructure.Data;
using PersonDirectory.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PersonCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PersonUpdateDtoValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Person Directory API",
        Version = "v1"
    });
});

// Register app-specific services
builder.Services.AddSingleton<AppDbContext>(); // replace with AddScoped if needed
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();

var app = builder.Build();

// Enable Swagger in all environments (you can restrict to Development if you prefer)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Person Directory API v1");
    c.RoutePrefix = "swagger"; // so UI is at /swagger
});

// Exception handling middleware
app.UseGlobalExceptionHandler();
// Enable HTTPS
app.UseHttpsRedirection();
app.UseHttpsRedirection();
// Routing & Controllers
app.UseAuthorization();
app.MapControllers();



app.Run();
