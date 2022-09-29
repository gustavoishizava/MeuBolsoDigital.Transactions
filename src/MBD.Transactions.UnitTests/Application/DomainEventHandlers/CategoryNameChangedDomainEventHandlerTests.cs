using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.DomainEventHandlers.Categories;
using MBD.Transactions.Domain.Events;
using MBD.Transactions.Domain.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.DomainEventHandlers
{
    public class CategoryNameChangedDomainEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;

        public CategoryNameChangedDomainEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var @event = new CategoryNameChangedDomainEvent(Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var handler = _autoMocker.CreateInstance<CategoryNameChangedDomainEventHandler>();

            // Act
            await handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<ITransactionRepository>()
                       .Verify(x => x.UpdateCategoryNameAsync(@event.Id, @event.NewName), Times.Once);
        }
    }
}