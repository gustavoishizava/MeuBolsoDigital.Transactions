using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MBD.Transactions.Application.Commands.Transactions;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Transactions.Delete
{
    public class DeleteTransactionCommandHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly DeleteTransactionCommandHandler _handler;

        public DeleteTransactionCommandHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();

            _handler = _autoMocker.CreateInstance<DeleteTransactionCommandHandler>();
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnFail()
        {
            // Arrange
            var command = new DeleteTransactionCommand(Guid.Empty);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.RemoveAsync(It.IsAny<Transaction>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_NotFound_ReturnFail()
        {
            // Arrange
            var command = new DeleteTransactionCommand(Guid.NewGuid());

            _autoMocker.GetMock<ITransactionRepository>()
                .Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync((Transaction)null);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Transação inválida.", result.Message);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.GetByIdAsync(command.Id), Times.Once);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.RemoveAsync(It.IsAny<Transaction>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var bankAccount = new BankAccount(Guid.NewGuid(), tenantId, "Bank account");
            var category = new Category(tenantId, "Category", TransactionType.Expense);
            var transaction = new Transaction(tenantId, bankAccount, category, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1), _faker.Random.Decimal(1, 50), "Description", null);

            var command = new DeleteTransactionCommand(transaction.Id);

            _autoMocker.GetMock<ITransactionRepository>()
                .Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(transaction);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.True(result.Succeeded);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.GetByIdAsync(command.Id), Times.Once);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.RemoveAsync(It.Is<Transaction>(x => x.Id == command.Id)), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}