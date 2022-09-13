using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.IntegrationEvents.Events;
using MediatR;

namespace MBD.Transactions.Application.IntegrationEvents.EventHandling
{
    public class BankAccountDescriptionChangedIntegrationEventHandler : INotificationHandler<BankAccountDescriptionChangedIntegrationEvent>
    {
        public Task Handle(BankAccountDescriptionChangedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}