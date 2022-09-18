using System;
using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using MBD.Transactions.Application.AutoMapper;
using MBD.Transactions.Application.Queries.Categories.GetAll;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Queries.Categories.GetAll
{
    public class GetAllCategoriesQueryHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly GetAllCategoriesQueryHandler _handler;

        public GetAllCategoriesQueryHandlerTests()
        {
            _autoMocker = new AutoMocker();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainToResponseProfile>());
            config.AssertConfigurationIsValid();
            _autoMocker.Use<IMapper>(config.CreateMapper());

            _handler = _autoMocker.CreateInstance<GetAllCategoriesQueryHandler>();
        }

        [Fact]
        public async void Handle_ReturnSuccess()
        {
            // Arrange
            var tentantId = Guid.NewGuid();
            var incomeCategory = new Category(tentantId, "Category 1", TransactionType.Income);
            var incomeCategory2 = new Category(tentantId, "Category 2", TransactionType.Income);
            var expenseCategory = new Category(tentantId, "Category 3", TransactionType.Expense);

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetAllAsync(It.IsAny<bool>()))
                .ReturnsAsync(new List<Category>() { incomeCategory, incomeCategory2, expenseCategory });

            // Act
            var result = await _handler.Handle(new GetAllCategoriesQuery(), new CancellationToken());

            // Assert
            Assert.Single(result.Expense);
            Assert.Equal(2, result.Income.Count);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetAllAsync(It.IsAny<bool>()), Times.Once);
        }
    }
}