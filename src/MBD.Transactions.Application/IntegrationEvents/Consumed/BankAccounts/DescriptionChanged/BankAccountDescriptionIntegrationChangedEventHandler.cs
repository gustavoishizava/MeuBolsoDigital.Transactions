using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.DescriptionChanged
{
    public class BankAccountDescriptionChangedIntegrationEventHandler : INotificationHandler<BankAccountDescriptionChangedIntegrationEvent>
    {
        public Task Handle(BankAccountDescriptionChangedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}