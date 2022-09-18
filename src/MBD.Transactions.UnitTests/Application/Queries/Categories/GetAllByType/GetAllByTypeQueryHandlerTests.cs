using System;
using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using MBD.Transactions.Application.AutoMapper;
using MBD.Transactions.Application.Queries.Categories.GetAllByType;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Queries.Categories.GetAllByType
{
    public class GetAllByTypeQueryHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly GetAllCategoriesByTypeQueryHandler _handler;

        public GetAllByTypeQueryHandlerTests()
        {
            _autoMocker = new AutoMocker();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainToResponseProfile>());
            config.AssertConfigurationIsValid();
            _autoMocker.Use<IMapper>(config.CreateMapper());

            _handler = _autoMocker.CreateInstance<GetAllCategoriesByTypeQueryHandler>();
        }

        [Theory]
        [InlineData(TransactionType.Expense)]
        [InlineData(TransactionType.Income)]
        public async void Handle_ReturnSuccess(TransactionType transactionType)
        {
            // Arrange
            var query = new GetAllCategoriesByTypeQuery(transactionType);

            var tentantId = Guid.NewGuid();
            var category = new Category(tentantId, "Category 1", transactionType);

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByTypeAsync(transactionType, It.IsAny<bool>()))
                .ReturnsAsync(new List<Category>() { category });

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            Assert.Single(result);

            _autoMocker.GetMock<ICategoryRepository>()
               .Verify(x => x.GetByTypeAsync(transactionType, It.IsAny<bool>()), Times.Once);
        }
    }
}