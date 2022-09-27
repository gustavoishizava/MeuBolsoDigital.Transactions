using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MBD.Transactions.Application.Commands.Transactions.Create;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Transactions.Create
{
    public class CreateTransactionCommandHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly CreateTransactionCommandHandler _handler;

        public CreateTransactionCommandHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();

            _handler = _autoMocker.CreateInstance<CreateTransactionCommandHandler>();
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnFail()
        {
            // Arrange
            var command = new CreateTransactionCommand(Guid.Empty,
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
            Assert.Null(result.Data);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_BankAccountNotFound_ReturnFail()
        {
            // Arrange
            var command = new CreateTransactionCommand(Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(0, 1000),
                                                       _faker.Random.AlphaNumeric(100));

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(command.BankAccountId))
                .ReturnsAsync((BankAccount)null);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Conta bancária inválida.", result.Message);
            Assert.Null(result.Data);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(command.BankAccountId), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_CategoryNotFound_ReturnFail()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var bankAccount = new BankAccount(Guid.NewGuid(), tenantId, "Bank account");

            var command = new CreateTransactionCommand(bankAccount.Id,
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(0, 1000),
                                                       _faker.Random.AlphaNumeric(100));

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
            Assert.Null(result.Data);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(command.BankAccountId), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(command.CategoryId), Times.Once);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Never);

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

            var command = new CreateTransactionCommand(bankAccount.Id,
                                                       category.Id,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(0, 1000),
                                                       _faker.Random.AlphaNumeric(100));

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
            Assert.NotNull(result.Data);

            Assert.NotEqual(Guid.Empty, result.Data.Id);
            Assert.Equal(bankAccount.Id, result.Data.BankAccount.Id);
            Assert.Equal(bankAccount.Description, result.Data.BankAccount.Description);
            Assert.Equal(category.Id, result.Data.Category.Id);
            Assert.Equal(category.Name, result.Data.Category.Name);
            Assert.Equal(category.Type, result.Data.Type);
            Assert.Equal(command.ReferenceDate, result.Data.ReferenceDate);
            Assert.Equal(command.DueDate, result.Data.DueDate);
            Assert.Equal(command.PaymentDate, result.Data.PaymentDate);
            Assert.Equal(command.Value, result.Data.Value);
            Assert.Equal(TransactionStatus.Paid, result.Data.Status);
            Assert.Equal(command.Description, result.Data.Description);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(command.BankAccountId), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(command.CategoryId), Times.Once);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.AddAsync(It.Is<Transaction>(x => x.TenantId == tenantId
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