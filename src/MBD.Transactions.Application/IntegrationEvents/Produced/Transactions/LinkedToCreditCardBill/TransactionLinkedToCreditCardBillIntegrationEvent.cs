using System;

namespace MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.LinkedToCreditCardBill
{
    public class TransactionLinkedToCreditCardBillIntegrationEvent
    {
        public Guid Id { get; private init; }
        public Guid BankAccountId { get; private init; }
        public Guid CreditCardBillId { get; private init; }
        public DateTime CreatedAt { get; private init; }
        public decimal Value { get; private init; }
        public DateTime TimeStamp { get; private init; }

        public TransactionLinkedToCreditCardBillIntegrationEvent(Guid id, Guid bankAccountId, Guid creditCardBillId, DateTime createdAt, decimal value, DateTime timeStamp)
        {
            Id = id;
            CreatedAt = createdAt;
            Value = value;
            BankAccountId = bankAccountId;
            CreditCardBillId = creditCardBillId;
            TimeStamp = timeStamp;
        }
    }
}