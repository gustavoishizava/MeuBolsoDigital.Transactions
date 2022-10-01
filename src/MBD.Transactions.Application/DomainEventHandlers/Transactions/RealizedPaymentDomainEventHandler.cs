using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Paid;
using MBD.Transactions.Domain.Events;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog.Services;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class RealizedPaymentDomainEventHandler : INotificationHandler<RealizedPaymentDomainEvent>
    {
        private readonly IIntegrationEventLogService _service;

        public RealizedPaymentDomainEventHandler(IIntegrationEventLogService service)
        {
            _service = service;
        }

        public async Task Handle(RealizedPaymentDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new TransactionPaidIntegrationEvent(notification.Id,
                                                             notification.Value,
                                                             notification.Date,
                                                             notification.BankAccountId,
                                                             notification.Type,
                                                             notification.TimeStamp);

            await _service.CreateEventAsync<TransactionPaidIntegrationEvent>(@event, "transaction.updated.paid");
        }
    }
}