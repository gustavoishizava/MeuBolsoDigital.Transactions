using System;
using MBD.Transactions.Domain.Events.Common;

namespace MBD.Transactions.Domain.Events
{
    public class TransactionDeletedDomainEvent : DomainEvent
    {
        public Guid Id { get; private init; }
        public TransactionDeletedDomainEvent(Guid id)
        {
            AggregateId = id;
            Id = id;
        }
    }
}