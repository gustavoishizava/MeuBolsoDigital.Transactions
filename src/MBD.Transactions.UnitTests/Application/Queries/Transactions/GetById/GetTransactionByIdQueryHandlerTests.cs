using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MBD.Transactions.Application.AutoMapper;
using MBD.Transactions.Application.Queries.Transactions.GetById;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Queries.Transactions.GetById
{
    public class GetTransactionByIdQueryHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly GetTransactionByIdQueryHandler _handler;

        public GetTransactionByIdQueryHandlerTests()
        {
            _autoMocker = new AutoMocker();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainToResponseProfile>());
            config.AssertConfigurationIsValid();
            _autoMocker.Use<IMapper>(config.CreateMapper());

            _handler = _autoMocker.CreateInstance<GetTransactionByIdQueryHandler>();
        }

        [Fact]
        public async Task Handle_NotFound_ReturnFail()
        {
            // Arrange
            var query = new GetTransactionByIdQuery(Guid.NewGuid());

            _autoMocker.GetMock<ITransactionRepository>()
                .Setup(x => x.GetByIdAsync(query.Id))
                .ReturnsAsync((Transaction)null);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.Equal("Transação inválida.", result.Message);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.GetByIdAsync(query.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var bankAccount = new BankAccount(Guid.NewGuid(), tenantId, "Bank account");
            var category = new Category(tenantId, "Category", TransactionType.Expense);
            var transaction = new Transaction(tenantId, bankAccount, category, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1), 10, "Description", null);
            var query = new GetTransactionByIdQuery(transaction.Id);

            _autoMocker.GetMock<ITransactionRepository>()
                .Setup(x => x.GetByIdAsync(query.Id))
                .ReturnsAsync(transaction);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            Assert.Equal(transaction.Id, result.Data.Id);
            Assert.Equal(transaction.BankAccount.Id, result.Data.BankAccount.Id);
            Assert.Equal(transaction.BankAccount.Description, result.Data.BankAccount.Description);
            Assert.Equal(transaction.Category.Id, result.Data.Category.Id);
            Assert.Equal(transaction.Category.Name, result.Data.Category.Name);
            Assert.Equal(transaction.Category.Type, result.Data.Type);
            Assert.Equal(transaction.ReferenceDate, result.Data.ReferenceDate);
            Assert.Equal(transaction.DueDate, result.Data.DueDate);
            Assert.Equal(transaction.PaymentDate, result.Data.PaymentDate);
            Assert.Equal(transaction.Status, result.Data.Status);
            Assert.Equal(transaction.Value, result.Data.Value);
            Assert.Equal(transaction.Description, result.Data.Description);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.GetByIdAsync(query.Id), Times.Once);
        }
    }
}