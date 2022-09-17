using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MBD.Transactions.Application.Commands.Categories.Update;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Categories.Update
{
    public class UpdateCategoryCommandHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly UpdateCategoryCommandHandler _handler;

        public UpdateCategoryCommandHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();

            _handler = _autoMocker.CreateInstance<UpdateCategoryCommandHandler>();
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnFail()
        {
            // Arrange
            var command = new UpdateCategoryCommand
            {
                Id = Guid.Empty,
                Name = string.Empty,
                Status = Status.Active
            };

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Message);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_NotFound_ReturnFail()
        {
            // Arrange
            var command = new UpdateCategoryCommand
            {
                Id = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(100),
                Status = Status.Active
            };

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
                .Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Theory]
        [InlineData(Status.Active)]
        [InlineData(Status.Inactive)]
        public async Task Handle_ReturnSuccess(Status status)
        {
            // Arrange
            var category = new Category(Guid.NewGuid(), "Category", TransactionType.Income);

            var command = new UpdateCategoryCommand
            {
                Id = category.Id,
                Name = _faker.Random.AlphaNumeric(100),
                Status = status
            };

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
                .Verify(x => x.UpdateAsync(It.Is<Category>(x => x.Id == command.Id && x.Name == command.Name && x.Status == command.Status)), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}