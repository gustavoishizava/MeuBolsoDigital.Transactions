using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Events;
using MediatR;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class TransactionUpdatedDomainEventHandler : INotificationHandler<TransactionUpdatedDomainEvent>
    {
        public Task Handle(TransactionUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}