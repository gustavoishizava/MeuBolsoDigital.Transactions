using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.Transactions.Domain.Entities.Common;
using MongoDB.Bson.Serialization;

namespace MBD.Transactions.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class BaseEntityWithEventMapping : BsonClassMapConfiguration
    {
        public override BsonClassMap GetConfiguration()
        {
            return new BsonClassMap<BaseEntityWithEvent>();
        }
    }
}