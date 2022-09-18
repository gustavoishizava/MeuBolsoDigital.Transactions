using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Events;
using MediatR;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class LinkedToCreditCardBillDomainEventHandler : INotificationHandler<LinkedToCreditCardBillDomainEvent>
    {
        public Task Handle(LinkedToCreditCardBillDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}