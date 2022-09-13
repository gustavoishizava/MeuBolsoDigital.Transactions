using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.IntegrationEvents.Events;
using MBD.Transactions.Domain.Events;
using MediatR;

namespace MBD.Transactions.Application.DomainEventHandlers
{
    public class LinkedToCreditCardBillDomainEventHandler : INotificationHandler<LinkedToCreditCardBillDomainEvent>
    {
        public Task Handle(LinkedToCreditCardBillDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}