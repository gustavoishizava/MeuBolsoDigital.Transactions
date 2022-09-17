using System;

namespace MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.UndoPayment
{
    public class TransactionUndoPaymentIntegrationEvent
    {
        public Guid Id { get; private init; }

        public TransactionUndoPaymentIntegrationEvent(Guid id)
        {
            Id = id;
        }
    }
}