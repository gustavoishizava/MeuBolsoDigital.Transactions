using System;

namespace MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.ValueChanged
{
    public class TransactionValueChangedIntegrationEvent
    {
        public Guid Id { get; init; }
        public decimal NewValue { get; init; }
        public decimal OldValue { get; init; }
        public DateTime TimeStamp { get; private init; }

        public TransactionValueChangedIntegrationEvent(Guid id, decimal newValue, decimal oldValue, DateTime timeStamp)
        {
            Id = id;
            NewValue = newValue;
            OldValue = oldValue;
            TimeStamp = timeStamp;
        }
    }
}