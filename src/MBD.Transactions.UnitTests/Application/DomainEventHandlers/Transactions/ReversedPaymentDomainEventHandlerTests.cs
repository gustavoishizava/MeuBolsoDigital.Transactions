using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.DomainEventHandlers.Transactions;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.UndoPayment;
using MBD.Transactions.Domain.Events;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.DomainEventHandlers.Transactions
{
    public class ReversedPaymentDomainEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;

        public ReversedPaymentDomainEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var @event = new ReversedPaymentDomainEvent(Guid.NewGuid());
            var handler = _autoMocker.CreateInstance<ReversedPaymentDomainEventHandler>();

            // Act
            await handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IIntegrationEventLogService>()
            .Verify(x => x.CreateEventAsync<TransactionUndoPaymentIntegrationEvent>(It.Is<TransactionUndoPaymentIntegrationEvent>(x => x.Id == @event.Id), "transaction.updated.undo_payment"));
        }
    }
}