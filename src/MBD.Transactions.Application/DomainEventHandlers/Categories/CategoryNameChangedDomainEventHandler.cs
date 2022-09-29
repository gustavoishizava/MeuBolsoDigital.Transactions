using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Events;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;

namespace MBD.Transactions.Application.DomainEventHandlers.Categories
{
    public class CategoryNameChangedDomainEventHandler : INotificationHandler<CategoryNameChangedDomainEvent>
    {
        private readonly ITransactionRepository _repository;

        public CategoryNameChangedDomainEventHandler(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(CategoryNameChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _repository.UpdateCategoryNameAsync(notification.Id, notification.NewName);
        }
    }
}