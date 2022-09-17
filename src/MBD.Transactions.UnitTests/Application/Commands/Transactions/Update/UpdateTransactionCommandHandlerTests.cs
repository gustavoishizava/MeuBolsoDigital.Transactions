using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MBD.Transactions.Application.Commands.Transactions.Update;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Transactions.Update
{
    public class UpdateTransactionCommandHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly UpdateTransactionCommandHandler _handler;

        public UpdateTransactionCommandHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();

            _handler = _autoMocker.CreateInstance<UpdateTransactionCommandHandler>();
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnFail()
        {
            // Arrange
            var command = new UpdateTransactionCommand(Guid.Empty,
                                                       Guid.Empty,
                                                       Guid.Empty,
                                                       DateTime.MinValue,
                                                       DateTime.MinValue,
                                                       DateTime.MinValue,
                                                       -1,
                                                       _faker.Random.AlphaNumeric(101));

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.UpdateAsync(It.IsAny<Transaction>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_TransactionNotFound_ReturnFail()
        {
            // Arrange
            var command = new UpdateTransactionCommand(Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(0, 1000),
                                                       _faker.Random.AlphaNumeric(100));

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

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.UpdateAsync(It.IsAny<Transaction>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_BankAccountNotFound_ReturnFail()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var bankAccount = new BankAccount(Guid.NewGuid(), tenantId, "Bank account");
            var category = new Category(tenantId, "Category", TransactionType.Expense);
            var transaction = new Transaction(tenantId, bankAccount, category, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1), _faker.Random.Decimal(1, 50), "Description", null);

            var command = new UpdateTransactionCommand(transaction.Id,
                                                       Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(51, 1000),
                                                       _faker.Random.AlphaNumeric(100));

            _autoMocker.GetMock<ITransactionRepository>()
                .Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(transaction);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(command.BankAccountId))
                .ReturnsAsync((BankAccount)null);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Conta bancária inválida.", result.Message);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.GetByIdAsync(command.Id), Times.Once);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(command.BankAccountId), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.UpdateAsync(It.IsAny<Transaction>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_CategoryNotFound_ReturnFail()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var bankAccount = new BankAccount(Guid.NewGuid(), tenantId, "Bank account");
            var category = new Category(tenantId, "Category", TransactionType.Expense);
            var transaction = new Transaction(tenantId, bankAccount, category, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1), _faker.Random.Decimal(1, 50), "Description", null);

            var command = new UpdateTransactionCommand(transaction.Id,
                                                       bankAccount.Id,
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(0, 1000),
                                                       _faker.Random.AlphaNumeric(100));

            _autoMocker.GetMock<ITransactionRepository>()
                .Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(transaction);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(command.BankAccountId))
                .ReturnsAsync(bankAccount);

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByIdAsync(command.CategoryId))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Categoria inválida.", result.Message);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.GetByIdAsync(command.Id), Times.Once);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(command.BankAccountId), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(command.CategoryId), Times.Once);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.UpdateAsync(It.IsAny<Transaction>()), Times.Never);

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

            var command = new UpdateTransactionCommand(transaction.Id,
                                                       category.Id,
                                                       bankAccount.Id,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(0, 1000),
                                                       _faker.Random.AlphaNumeric(100));

            _autoMocker.GetMock<ITransactionRepository>()
                .Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(transaction);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(command.BankAccountId))
                .ReturnsAsync(bankAccount);

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByIdAsync(command.CategoryId))
                .ReturnsAsync(category);

            _autoMocker.GetMock<ILoggedUser>()
                .Setup(x => x.UserId)
                .Returns(tenantId);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Message);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.GetByIdAsync(command.Id), Times.Once);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(command.BankAccountId), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(command.CategoryId), Times.Once);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.UpdateAsync(It.Is<Transaction>(x => x.Id == command.Id
                                                                && x.TenantId == tenantId
                                                                && x.BankAccount == bankAccount
                                                                && x.Category == category
                                                                && x.ReferenceDate == command.ReferenceDate
                                                                && x.DueDate == command.DueDate
                                                                && x.Value == command.Value
                                                                && x.Description == command.Description
                                                                && x.PaymentDate == command.PaymentDate)), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}