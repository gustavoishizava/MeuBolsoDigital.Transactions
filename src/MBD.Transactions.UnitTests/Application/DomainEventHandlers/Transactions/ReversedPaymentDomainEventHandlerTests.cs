using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.DomainEventHandlers.Transactions;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.UndoPayment;
using MBD.Transactions.Domain.Enumerations;
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
            var @event = new ReversedPaymentDomainEvent(Guid.NewGuid(), Guid.NewGuid(), TransactionType.Expense, 10);
            var handler = _autoMocker.CreateInstance<ReversedPaymentDomainEventHandler>();

            // Act
            await handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IIntegrationEventLogService>()
            .Verify(x => x.CreateEventAsync<TransactionUndoPaymentIntegrationEvent>(It.Is<TransactionUndoPaymentIntegrationEvent>(x => x.Id == @event.Id
                                                                                                                                       && x.BankAccountId == @event.BankAccountId
                                                                                                                                       && x.Type == @event.Type
                                                                                                                                       && x.Value == @event.Value
                                                                                                                                       && x.TimeStamp == @event.TimeStamp), "updated.undo_payment"), Times.Once);
        }
    }
}