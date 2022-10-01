using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Deleted;
using MBD.Transactions.Domain.Events;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog.Services;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class TransactionDeletedDomainEventHandler : INotificationHandler<TransactionDeletedDomainEvent>
    {
        private readonly IIntegrationEventLogService _service;

        public TransactionDeletedDomainEventHandler(IIntegrationEventLogService service)
        {
            _service = service;
        }

        public async Task Handle(TransactionDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new TransactionDeletedIntegrationEvent(notification.Id, notification.TimeStamp);

            await _service.CreateEventAsync<TransactionDeletedIntegrationEvent>(@event, "transaction.deleted");
        }
    }
}