using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Extensions;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Infrastructure.Context;
using MBD.Transactions.Infrastructure.Context.Mappings;
using MeuBolsoDigital.IntegrationEventLog;
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
                options.AddSerializer(new EnumSerializer<Status>());
                options.AddSerializer(new EnumSerializer<TransactionStatus>());
                options.AddSerializer(new EnumSerializer<TransactionType>());
                options.AddSerializer(new EnumSerializer<EventState>());
                options.AddSerializer(new DecimalSerializer(BsonType.Decimal128));

                options.AddBsonClassMap(new BaseEntityMapping());
                options.AddBsonClassMap(new BaseEntityWithEventMapping());
                options.AddBsonClassMap(new BankAccountMapping());
                options.AddBsonClassMap(new CategoryMapping());
                options.AddBsonClassMap(new TransactionMapping());
                options.AddBsonClassMap(new IntegrationEventLogEntryMapping());
            });

            return services;
        }
    }
}