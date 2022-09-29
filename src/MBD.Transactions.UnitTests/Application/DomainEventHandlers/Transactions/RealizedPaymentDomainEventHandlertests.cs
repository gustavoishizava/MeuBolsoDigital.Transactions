using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.DomainEventHandlers.Transactions;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Paid;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Events;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.DomainEventHandlers.Transactions
{
    public class RealizedPaymentDomainEventHandlertests
    {
        private readonly AutoMocker _autoMocker;

        public RealizedPaymentDomainEventHandlertests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var @event = new RealizedPaymentDomainEvent(Guid.NewGuid(), DateTime.Now, 100, Guid.NewGuid(), TransactionType.Expense);
            var handler = _autoMocker.CreateInstance<RealizedPaymentDomainEventHandler>();

            // Act
            await handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IIntegrationEventLogService>()
            .Verify(x => x.CreateEventAsync<TransactionPaidIntegrationEvent>(It.Is<TransactionPaidIntegrationEvent>(x => x.Id == @event.Id
                                                                                                                         && x.Date == @event.Date
                                                                                                                         && x.BankAccountId == @event.BankAccountId
                                                                                                                         && x.Value == @event.Value
                                                                                                                         && x.Type == @event.Type.ToString()), "transaction.updated.paid"));
        }
    }
}