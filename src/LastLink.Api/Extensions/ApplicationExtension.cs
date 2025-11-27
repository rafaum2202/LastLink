using LastLink.Application.Services;
using LastLink.Domain.Contracts.Services;

namespace LastLink.Api.Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAnticipationService, AnticipationService>();

            return services;
        }
    }
}
