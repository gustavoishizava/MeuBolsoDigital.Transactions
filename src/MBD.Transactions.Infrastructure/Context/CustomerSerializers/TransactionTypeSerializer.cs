using System;
using System.Diagnostics.CodeAnalysis;
using MBD.Transactions.Domain.Enumerations;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.Transactions.Infrastructure.Context.CustomerSerializers
{
    [ExcludeFromCodeCoverage]
    public class TransactionTypeSerializer : SerializerBase<TransactionType>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TransactionType value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override TransactionType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Enum.Parse<TransactionType>(context.Reader.ReadString());
        }
    }
}