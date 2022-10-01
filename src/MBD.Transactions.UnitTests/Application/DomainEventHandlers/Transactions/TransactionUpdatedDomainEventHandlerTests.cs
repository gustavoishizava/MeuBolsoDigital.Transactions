using System;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.DomainEventHandlers.Transactions;
using MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Updated;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Events;
using MeuBolsoDigital.IntegrationEventLog.Services;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.DomainEventHandlers.Transactions
{
    public class TransactionUpdatedDomainEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;

        public TransactionUpdatedDomainEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var bankAccount = new BankAccount(Guid.NewGuid(), tenantId, "Bank account");
            var category = new Category(tenantId, "Category", TransactionType.Expense);
            var transaction = new Transaction(tenantId, bankAccount, category, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1), 50, "Description", null);
            var @event = new TransactionUpdatedDomainEvent(transaction);
            var handler = _autoMocker.CreateInstance<TransactionUpdatedDomainEventHandler>();

            // Act
            await handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IIntegrationEventLogService>()
            .Verify(x => x.CreateEventAsync<TransactionUpdatedIntegrationEvent>(It.Is<TransactionUpdatedIntegrationEvent>(x => x.Id == @event.AggregateId
                                                                                                                               && x.TenantId == @event.TenantId
                                                                                                                               && x.BankAccountId == @event.BankAccount.Id
                                                                                                                               && x.CategoryId == @event.Category.Id
                                                                                                                               && x.ReferenceDate == @event.ReferenceDate
                                                                                                                               && x.DueDate == @event.DueDate
                                                                                                                               && x.PaymentDate == @event.PaymentDate
                                                                                                                               && x.Status == @event.Status
                                                                                                                               && x.Value == @event.Value
                                                                                                                               && x.Description == @event.Description
                                                                                                                               && x.Type == @event.Category.Type
                                                                                                                               && x.TimeStamp == @event.TimeStamp), "transaction.updated"), Times.Once);
        }
    }
}