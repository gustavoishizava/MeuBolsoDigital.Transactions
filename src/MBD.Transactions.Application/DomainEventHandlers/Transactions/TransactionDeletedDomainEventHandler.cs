using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Events;
using MediatR;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class TransactionDeletedDomainEventHandler : INotificationHandler<TransactionDeletedDomainEvent>
    {
        public Task Handle(TransactionDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}