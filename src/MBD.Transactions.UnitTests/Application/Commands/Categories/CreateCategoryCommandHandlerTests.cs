using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using MBD.Transactions.Application.AutoMapper;
using MBD.Transactions.Application.Commands.Categories;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Categories
{
    public class CreateCategoryCommandHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly CreateCategoryCommandHandler _handler;

        public CreateCategoryCommandHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainToResponseProfile>());
            config.AssertConfigurationIsValid();
            _autoMocker.Use<IMapper>(config.CreateMapper());

            _handler = _autoMocker.CreateInstance<CreateCategoryCommandHandler>();
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnFail()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                Name = string.Empty,
                Type = TransactionType.Expense
            };

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.NotEmpty(result.Message);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_HasParentCategoryId_ParentCategoryNotFound_ReturnFail()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                ParentCategoryId = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(100),
                Type = TransactionType.Expense
            };

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByIdAsync(command.ParentCategoryId.Value))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.NotEmpty(result.Message);
            Assert.Equal("Categoria pai inválida.", result.Message);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(command.ParentCategoryId.Value), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_HasParentCategoryId_ParentCategoryCantHaveSubcategories_ReturnFail()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                ParentCategoryId = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(100),
                Type = TransactionType.Expense
            };

            var category = new Category(Guid.NewGuid(), "Category", TransactionType.Income);
            var subcategory = category.AddSubCategory("subcategory");

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByIdAsync(command.ParentCategoryId.Value))
                .ReturnsAsync(subcategory);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.NotEmpty(result.Message);
            Assert.Equal("Não é permitido adicionar uma subcategoria à uma categoria filha.", result.Message);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(command.ParentCategoryId.Value), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_HasParentCategoryId_ReturnSuccess()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                ParentCategoryId = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(100),
                Type = TransactionType.Expense
            };

            var category = new Category(Guid.NewGuid(), "Category", TransactionType.Expense);

            _autoMocker.GetMock<ICategoryRepository>()
                .Setup(x => x.GetByIdAsync(command.ParentCategoryId.Value))
                .ReturnsAsync(category);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);

            Assert.NotEqual(Guid.Empty, result.Data.Id);
            Assert.Equal(command.Name, result.Data.Name);
            Assert.Equal(command.Type, result.Data.Type);
            Assert.Equal(category.Id, result.Data.ParentCategoryId);
            Assert.Equal(Status.Active, result.Data.Status);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(command.ParentCategoryId.Value), Times.Once);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.AddAsync(It.Is<Category>(x => x.ParentCategoryId == category.Id
                                                             && x.Name == command.Name
                                                             && x.Type == command.Type)), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_NoHasParentCategoryId_ReturnSuccess()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                ParentCategoryId = null,
                Name = _faker.Random.AlphaNumeric(100),
                Type = TransactionType.Expense
            };

            _autoMocker.GetMock<ILoggedUser>()
                .Setup(x => x.UserId).Returns(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);

            Assert.NotEqual(Guid.Empty, result.Data.Id);
            Assert.Equal(command.Name, result.Data.Name);
            Assert.Equal(command.Type, result.Data.Type);
            Assert.Null(result.Data.ParentCategoryId);
            Assert.Equal(Status.Active, result.Data.Status);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICategoryRepository>()
                .Verify(x => x.AddAsync(It.Is<Category>(x => x.ParentCategoryId == null
                                                             && x.Name == command.Name
                                                             && x.Type == command.Type)), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}