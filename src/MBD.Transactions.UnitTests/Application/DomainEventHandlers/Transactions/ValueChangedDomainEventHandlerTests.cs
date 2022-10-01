using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MBD.Transactions.Application.DomainEventHandlers.Transactions;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.ValueChanged;
using MBD.Transactions.Domain.Events;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.DomainEventHandlers.Transactions
{
    public class ValueChangedDomainEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;

        public ValueChangedDomainEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var @event = new ValueChangedDomainEvent(Guid.NewGuid(), _faker.Random.Decimal(1, 100), _faker.Random.Decimal(1, 100));
            var handler = _autoMocker.CreateInstance<ValueChangedDomainEventHandler>();

            // Act
            await handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IIntegrationEventLogService>()
            .Verify(x => x.CreateEventAsync<TransactionValueChangedIntegrationEvent>(It.Is<TransactionValueChangedIntegrationEvent>(x => x.Id == @event.Id
                                                                                                                                         && x.NewValue == @event.NewValue
                                                                                                                                         && x.OldValue == @event.OldValue
                                                                                                                                         && x.TimeStamp == @event.TimeStamp), "transaction.updated.value_changed"), Times.Once);
        }
    }
}