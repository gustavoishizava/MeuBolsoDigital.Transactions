using System;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Events.Common;

namespace MBD.Transactions.Domain.Events
{
    public class ReversedPaymentDomainEvent : DomainEvent
    {
        public Guid Id { get; private init; }
        public Guid BankAccountId { get; private init; }
        public TransactionType Type { get; private init; }
        public decimal Value { get; private init; }

        public ReversedPaymentDomainEvent(Guid id, Guid bankAccountId, TransactionType type, decimal value)
        {
            AggregateId = id;
            Id = id;
            BankAccountId = bankAccountId;
            Type = type;
            Value = value;
        }
    }
}