using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Events;
using MediatR;

namespace MBD.Transactions.Application.DomainEventHandlers
{
    public class ReversedPaymentDomainEventHandler : INotificationHandler<ReversedPaymentDomainEvent>
    {
        public Task Handle(ReversedPaymentDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}