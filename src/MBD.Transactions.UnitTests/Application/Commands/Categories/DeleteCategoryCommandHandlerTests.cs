using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MBD.Transactions.Application.Commands.Categories;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Categories
{
    public class DeleteCategoryCommandHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly DeleteCategoryCommandHandler _handler;

        public DeleteCategoryCommandHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();

            _handler = _autoMocker.CreateInstance<DeleteCategoryCommandHandler>();
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnFail()
        {
            // Arrange
            var command = new DeleteCategoryCommand(Guid.Empty);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.RemoveAsync(It.IsAny<Category>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_NotFound_ReturnFail()
        {
            // Arrange
            var command = new DeleteCategoryCommand(Guid.NewGuid());

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Categoria inválida.", result.Message);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(command.Id), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.RemoveAsync(It.IsAny<Category>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnSuccess()
        {
            // Arrange
            var category = new Category(Guid.NewGuid(), "Category", TransactionType.Expense);
            var command = new DeleteCategoryCommand(category.Id);

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(category);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Message);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(command.Id), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.RemoveAsync(It.Is<Category>(x => x.Id == command.Id)), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}