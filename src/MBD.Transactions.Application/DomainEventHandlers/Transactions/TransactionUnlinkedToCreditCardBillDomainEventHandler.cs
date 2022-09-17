using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Events;
using MediatR;

namespace MBD.Transactions.Application.DomainEventHandlers.Transactions
{
    public class TransactionUnlinkedToCreditCardBillDomainEventHandler : INotificationHandler<UnlinkedToCreditCardBillDomainEvent>
    {
        public Task Handle(UnlinkedToCreditCardBillDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}