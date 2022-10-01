using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Updated;
using MBD.Transactions.Domain.Events;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog.Services;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class TransactionUpdatedDomainEventHandler : INotificationHandler<TransactionUpdatedDomainEvent>
    {
        private readonly IIntegrationEventLogService _service;

        public TransactionUpdatedDomainEventHandler(IIntegrationEventLogService service)
        {
            _service = service;
        }

        public async Task Handle(TransactionUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new TransactionUpdatedIntegrationEvent(notification.AggregateId,
                                                                notification.TenantId,
                                                                notification.BankAccount.Id,
                                                                notification.Category.Id,
                                                                notification.ReferenceDate,
                                                                notification.DueDate,
                                                                notification.PaymentDate,
                                                                notification.Status,
                                                                notification.Value,
                                                                notification.Description,
                                                                notification.Category.Type,
                                                                notification.TimeStamp);

            await _service.CreateEventAsync<TransactionUpdatedIntegrationEvent>(@event, "transaction.updated");
        }
    }
}