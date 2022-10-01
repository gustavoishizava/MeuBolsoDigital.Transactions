using System;

namespace MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Deleted
{
    public class TransactionDeletedIntegrationEvent
    {
        public Guid Id { get; init; }
        public DateTime TimeStamp { get; init; }

        public TransactionDeletedIntegrationEvent(Guid id, DateTime timeStamp)
        {
            Id = id;
            TimeStamp = timeStamp;
        }
    }
}