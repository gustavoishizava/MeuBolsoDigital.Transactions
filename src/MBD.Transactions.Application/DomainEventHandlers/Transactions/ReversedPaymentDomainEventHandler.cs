using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.UndoPayment;
using MBD.Transactions.Domain.Events;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog.Services;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class ReversedPaymentDomainEventHandler : INotificationHandler<ReversedPaymentDomainEvent>
    {
        private readonly IIntegrationEventLogService _service;

        public ReversedPaymentDomainEventHandler(IIntegrationEventLogService service)
        {
            _service = service;
        }

        public async Task Handle(ReversedPaymentDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new TransactionUndoPaymentIntegrationEvent(notification.Id, notification.BankAccountId, notification.Type, notification.Value, notification.TimeStamp);

            await _service.CreateEventAsync<TransactionUndoPaymentIntegrationEvent>(@event, "updated.undo_payment");
        }
    }
}