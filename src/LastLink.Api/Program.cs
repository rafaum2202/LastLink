using Asp.Versioning.ApiExplorer;
using FluentValidation;
using FluentValidation.AspNetCore;
using LastLink.Api.Extensions;
using LastLink.Api.Middlewares;
using LastLink.Domain.Configurations;
using LastLink.Infra.Extensions;
using System.Text.Json.Serialization;

namespace LastLink.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddApplication()
                .AddInfra()
                .AddSwaggerDocumentation()
                .AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.Configure<RulesConfig>(builder.Configuration.GetSection("RulesConfig"));

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });

            app.UseHttpsRedirection();

            app.UseMiddleware<CorrelationLoggingMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}