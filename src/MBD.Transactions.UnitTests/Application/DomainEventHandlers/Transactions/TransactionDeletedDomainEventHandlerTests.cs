using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.DomainEventHandlers.Transactions;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Deleted;
using MBD.Transactions.Domain.Events;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.DomainEventHandlers.Transactions
{
    public class TransactionDeletedDomainEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;

        public TransactionDeletedDomainEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var @event = new TransactionDeletedDomainEvent(Guid.NewGuid());
            var handler = _autoMocker.CreateInstance<TransactionDeletedDomainEventHandler>();

            // Act
            await handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IIntegrationEventLogService>()
            .Verify(x => x.CreateEventAsync<TransactionDeletedIntegrationEvent>(It.Is<TransactionDeletedIntegrationEvent>(x => x.Id == @event.Id && x.TimeStamp == @event.TimeStamp), "transaction.deleted"), Times.Once);
        }
    }
}