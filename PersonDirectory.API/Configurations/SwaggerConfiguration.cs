using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.API.Configurations;
public static class SwaggerConfiguration
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Person Directory API", Version = "v1" });
        });
    }

    public static void Swagger(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}