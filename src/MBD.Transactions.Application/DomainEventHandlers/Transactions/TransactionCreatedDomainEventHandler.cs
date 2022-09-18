using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Events;
using MediatR;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class TransactionCreatedDomainEventHandler : INotificationHandler<TransactionCreatedDomainEvent>
    {
        public Task Handle(TransactionCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}