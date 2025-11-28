using LastLink.Domain.Contracts.Repositories;
using LastLink.Infra.Data;
using LastLink.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LastLink.Infra.Extensions
{
    public static class InfraExtensions
    {
        public static IServiceCollection AddInfra(this IServiceCollection services)
        {
            services.AddDbContext<LastLinkDbContext>(opt =>
                opt.UseInMemoryDatabase("LastLinkDb"));

            services.AddScoped<IAnticipationRepository, AnticipationRepository>();

            return services;
        }
    }
}