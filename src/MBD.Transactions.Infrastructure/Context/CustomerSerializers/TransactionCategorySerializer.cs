using System.Diagnostics.CodeAnalysis;
using MBD.Transactions.Domain.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.Transactions.Infrastructure.Context.CustomerSerializers
{
    [ExcludeFromCodeCoverage]
    public class TransactionCategorySerializer : SerializerBase<Category>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Category value)
        {
            context.Writer.WriteStartDocument();

            context.Writer.WriteName("_id");
            context.Writer.WriteString(value.Id.ToString());

            context.Writer.WriteName("tenant_id");
            context.Writer.WriteString(value.TenantId.ToString());

            context.Writer.WriteName("name");
            context.Writer.WriteString(value.Name);

            context.Writer.WriteName("type");
            context.Writer.WriteString(value.Type.ToString());

            context.Writer.WriteEndDocument();
        }

        public override Category Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return BsonSerializer.Deserialize<Category>(context.Reader);
        }
    }
}