using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Created;
using MBD.Transactions.Domain.Events;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog.Services;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class TransactionCreatedDomainEventHandler : INotificationHandler<TransactionCreatedDomainEvent>
    {
        private readonly IIntegrationEventLogService _service;

        public TransactionCreatedDomainEventHandler(IIntegrationEventLogService service)
        {
            _service = service;
        }

        public async Task Handle(TransactionCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new TransactionCreatedIntegrationEvent(notification.AggregateId,
                                                                notification.TenantId,
                                                                notification.BankAccount.Id,
                                                                notification.Category.Id,
                                                                notification.ReferenceDate,
                                                                notification.DueDate,
                                                                notification.PaymentDate,
                                                                notification.Status,
                                                                notification.Value,
                                                                notification.Description,
                                                                notification.Category.Type);

            await _service.CreateEventAsync<TransactionCreatedIntegrationEvent>(@event, "transaction.created");
        }
    }
}