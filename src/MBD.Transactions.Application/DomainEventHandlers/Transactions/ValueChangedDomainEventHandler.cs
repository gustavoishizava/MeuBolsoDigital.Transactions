using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.ValueChanged;
using MBD.Transactions.Domain.Events;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog.Services;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class ValueChangedDomainEventHandler : INotificationHandler<ValueChangedDomainEvent>
    {
        private readonly IIntegrationEventLogService _service;

        public ValueChangedDomainEventHandler(IIntegrationEventLogService service)
        {
            _service = service;
        }

        public async Task Handle(ValueChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new TransactionValueChangedIntegrationEvent(notification.Id, notification.NewValue, notification.OldValue, notification.TimeStamp);

            await _service.CreateEventAsync<TransactionValueChangedIntegrationEvent>(@event, "transaction.updated.value_changed");
        }
    }
}