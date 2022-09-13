using DotNet.MongoDB.Context.Extensions;
using MBD.Transactions.Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.Transactions.API.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddEFContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDbContext<TransactionContext>(options =>
            {
                options.ConfigureConnection(configuration.GetConnectionString("Default"), configuration["DatabaseName"]);
                options.AddSerializer(new GuidSerializer(BsonType.String));
            });

            return services;
        }
    }
}