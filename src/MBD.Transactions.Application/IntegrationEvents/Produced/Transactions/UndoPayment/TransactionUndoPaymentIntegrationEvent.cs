using System;
using MBD.Transactions.Domain.Enumerations;

namespace MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.UndoPayment
{
    public class TransactionUndoPaymentIntegrationEvent
    {
        public Guid Id { get; private init; }
        public Guid BankAccountId { get; private init; }
        public TransactionType Type { get; private init; }
        public decimal Value { get; private init; }

        public TransactionUndoPaymentIntegrationEvent(Guid id, Guid bankAccountId, TransactionType type, decimal value)
        {
            Id = id;
            BankAccountId = bankAccountId;
            Type = type;
            Value = value;
        }
    }
}