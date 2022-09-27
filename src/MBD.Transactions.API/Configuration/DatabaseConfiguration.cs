using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Extensions;
using MBD.Transactions.Infrastructure.Context;
using MBD.Transactions.Infrastructure.Context.CustomerSerializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.Transactions.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDbContext<TransactionContext>(options =>
            {
                options.ConfigureConnection(configuration.GetConnectionString("Default"), configuration["DatabaseName"]);
                options.AddSerializer(new GuidSerializer(BsonType.String));
                options.AddSerializer(new StatusSerializer());
                options.AddSerializer(new TransactionStatusSerializer());
                options.AddSerializer(new TransactionTypeSerializer());
                options.AddSerializer(new Decimal128Serializer(BsonType.Decimal128));
            });

            return services;
        }
    }
}