using System;

namespace MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Deleted
{
    public class TransactionDeletedIntegrationEvent
    {
        public Guid Id { get; init; }

        public TransactionDeletedIntegrationEvent(Guid id)
        {
            Id = id;
        }
    }
}