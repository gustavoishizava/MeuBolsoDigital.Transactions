using MBD.Transactions.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MBD.Transactions.API.Configuration
{
    public static class HealthCheckConfiguration
    {
        public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<TransactionContext>();

            return services;
        }

        public static IEndpointRouteBuilder MapHealthCheckEndpoint(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapHealthChecks("/health");

            return endpoint;
        }
    }
}