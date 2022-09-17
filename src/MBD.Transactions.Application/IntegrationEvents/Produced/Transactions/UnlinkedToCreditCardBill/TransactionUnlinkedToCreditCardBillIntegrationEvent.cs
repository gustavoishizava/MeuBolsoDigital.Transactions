using System;

namespace MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.UnlinkedToCreditCardBill
{
    public class TransactionUnlinkedToCreditCardBillIntegrationEvent
    {
        public Guid Id { get; private set; }
        public Guid BankAccountId { get; private init; }
        public Guid CreditCardBillId { get; private init; }

        public TransactionUnlinkedToCreditCardBillIntegrationEvent(Guid id, Guid bankAccountId, Guid creditCardBillId)
        {
            Id = id;
            BankAccountId = bankAccountId;
            CreditCardBillId = creditCardBillId;
        }
    }
}