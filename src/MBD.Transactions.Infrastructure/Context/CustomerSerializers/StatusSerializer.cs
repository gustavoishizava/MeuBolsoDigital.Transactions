using System;
using System.Diagnostics.CodeAnalysis;
using MBD.Transactions.Domain.Enumerations;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.Transactions.Infrastructure.Context.CustomerSerializers
{
    [ExcludeFromCodeCoverage]
    public class StatusSerializer : SerializerBase<Status>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Status value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override Status Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Enum.Parse<Status>(context.Reader.ReadString());
        }
    }
}