using System;
using MeuBolsoDigital.IntegrationEventLog;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.Transactions.Infrastructure.Context.CustomSerializers
{
    public class EventStateSerializer : SerializerBase<EventState>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, EventState value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override EventState Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Enum.Parse<EventState>(context.Reader.ReadString());
        }
    }
}