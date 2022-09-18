using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MBD.Transactions.Application.AutoMapper;
using MBD.Transactions.Application.Queries.Categories.GetById;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Queries.Categories.GetById
{
    public class GetCategoryByIdQueryHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly GetCategoryByIdQueryHandler _handler;

        public GetCategoryByIdQueryHandlerTests()
        {
            _autoMocker = new AutoMocker();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainToResponseProfile>());
            config.AssertConfigurationIsValid();
            _autoMocker.Use<IMapper>(config.CreateMapper());

            _handler = _autoMocker.CreateInstance<GetCategoryByIdQueryHandler>();
        }

        [Fact]
        public async Task Handle_NotFound_ReturnFail()
        {
            // Arrange
            var query = new GetCategoryByIdQuery(Guid.NewGuid());

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByIdAsync(query.Id))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.Equal("Categoria inválida.", result.Message);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(query.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var category = new Category(Guid.NewGuid(), "Category", TransactionType.Expense);
            var query = new GetCategoryByIdQuery(category.Id);

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByIdAsync(query.Id))
                .ReturnsAsync(category);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            Assert.Equal(category.Id, result.Data.Id);
            Assert.Equal(category.Name, result.Data.Name);
            Assert.Equal(category.Type, result.Data.Type);
            Assert.Equal(category.Status, result.Data.Status);
            Assert.Equal(category.ParentCategoryId, result.Data.ParentCategoryId);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(query.Id), Times.Once);
        }
    }
}