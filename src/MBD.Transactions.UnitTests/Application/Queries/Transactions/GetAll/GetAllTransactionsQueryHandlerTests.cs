using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MBD.Transactions.Application.AutoMapper;
using MBD.Transactions.Application.Queries.Transactions.GetAll;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Queries.Transactions.GetAll
{
    public class GetAllTransactionsQueryHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly GetAllTransactionsQueryHandler _handler;

        public GetAllTransactionsQueryHandlerTests()
        {
            _autoMocker = new AutoMocker();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainToResponseProfile>());
            config.AssertConfigurationIsValid();
            _autoMocker.Use<IMapper>(config.CreateMapper());

            _handler = _autoMocker.CreateInstance<GetAllTransactionsQueryHandler>();
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var bankAccount = new BankAccount(Guid.NewGuid(), tenantId, "Bank account");
            var category = new Category(tenantId, "Category", TransactionType.Expense);
            var transaction = new Transaction(tenantId, bankAccount, category, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1), 10, "Description", null);

            _autoMocker.GetMock<ITransactionRepository>()
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Transaction>() { transaction });

            // Act
            var result = await _handler.Handle(new GetAllTransactionsQuery(), new CancellationToken());

            // Assert
            Assert.Single(result);
        }
    }
}