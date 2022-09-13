using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.MongoDbSettings;
using MBD.Transactions.Application.Response.Models;
using MBD.Transactions.Domain.Events;
using MediatR;

namespace MBD.Transactions.Application.DomainEventHandlers
{
    public class CategoryNameChangedDomainEventHandler : INotificationHandler<CategoryNameChangedDomainEvent>
    {
        public Task Handle(CategoryNameChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}