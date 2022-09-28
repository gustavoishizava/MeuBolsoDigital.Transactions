using System;
using System.Diagnostics.CodeAnalysis;
using MBD.Transactions.Domain.Enumerations;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.Transactions.Infrastructure.Context.CustomSerializers
{
    [ExcludeFromCodeCoverage]
    public class TransactionStatusSerializer : SerializerBase<TransactionStatus>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TransactionStatus value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override TransactionStatus Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Enum.Parse<TransactionStatus>(context.Reader.ReadString());
        }
    }
}