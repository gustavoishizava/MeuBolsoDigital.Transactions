using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MBD.Transactions.API.Controllers;
using MBD.Transactions.API.Models;
using MBD.Transactions.Application.Commands.Categories.Create;
using MBD.Transactions.Application.Commands.Categories.Delete;
using MBD.Transactions.Application.Commands.Categories.Update;
using MBD.Transactions.Application.Queries.Categories.GetAll;
using MBD.Transactions.Application.Queries.Categories.GetAllByType;
using MBD.Transactions.Application.Queries.Categories.GetById;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Enumerations;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.API.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();
            _controller = _autoMocker.CreateInstance<CategoriesController>();
        }

        [Fact]
        public async Task GetAll_Empty_ReturnNoContent()
        {
            // Arrange
            var result = new CategoryByTypeResponse(new(), new());

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(result);

            // Act
            var response = await _controller.GetAll() as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            _autoMocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<GetAllCategoriesQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnOk()
        {
            // Arrange
            var category = new CategoryWithSubCategoriesResponse
            {
                Id = Guid.NewGuid(),
                ParentCategoryId = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(100),
                Type = TransactionType.Expense,
                Status = Status.Active
            };

            var result = new CategoryByTypeResponse(new(), new() { category });

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(result);

            // Act
            var response = await _controller.GetAll() as ObjectResult;
            var value = response.Value as CategoryByTypeResponse;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.NotNull(value);
        }

        [Fact]
        public async Task GetById_ReturnNotFound()
        {
            // Arrange
            var query = new GetCategoryByIdQuery(Guid.NewGuid());

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<GetCategoryByIdQuery>(x => x.Id == query.Id), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Result<CategoryResponse>.Fail());

            // Act
            var response = await _controller.GetById(query.Id) as NotFoundResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnOk()
        {
            // Arrange
            var query = new GetCategoryByIdQuery(Guid.NewGuid());

            var category = new CategoryResponse
            {
                Id = Guid.NewGuid(),
                ParentCategoryId = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(100),
                Type = TransactionType.Expense,
                Status = Status.Active
            };

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<GetCategoryByIdQuery>(x => x.Id == query.Id), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Result<CategoryResponse>.Success(category));

            // Act
            var response = await _controller.GetById(query.Id) as ObjectResult;
            var value = response.Value as CategoryResponse;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(category, value);
        }

        [Fact]
        public async Task GetAllByType_ReturnNoContent()
        {
            // Arrange
            var query = new GetAllCategoriesByTypeQuery(TransactionType.Income);

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<GetAllCategoriesByTypeQuery>(x => x.Type == query.Type), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((List<CategoryWithSubCategoriesResponse>)null);

            // Act
            var response = await _controller.GetAllByType(query.Type) as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetAllByType_ReturnOk()
        {
            // Arrange
            var query = new GetAllCategoriesByTypeQuery(TransactionType.Income);

            var category = new CategoryWithSubCategoriesResponse
            {
                Id = Guid.NewGuid(),
                ParentCategoryId = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(100),
                Type = TransactionType.Income,
                Status = Status.Active
            };

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<GetAllCategoriesByTypeQuery>(x => x.Type == query.Type), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new List<CategoryWithSubCategoriesResponse> { category });

            // Act
            var response = await _controller.GetAllByType(query.Type) as ObjectResult;
            var value = response.Value as List<CategoryWithSubCategoriesResponse>;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnBadRequest()
        {
            // Arrange
            var request = new CreateCategoryCommand
            {
                Name = _faker.Random.AlphaNumeric(50),
                ParentCategoryId = null,
                Type = TransactionType.Income
            };

            var errorMessage = Guid.NewGuid().ToString();

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(request, default))
                       .ReturnsAsync(Result<CategoryResponse>.Fail(errorMessage));

            // Act
            var response = await _controller.Create(request) as ObjectResult;
            var value = response.Value as ErrorModel;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, value.Errors.First());
        }

        [Fact]
        public async Task Create_ReturnCreated()
        {
            // Arrange
            var request = new CreateCategoryCommand
            {
                Name = _faker.Random.AlphaNumeric(50),
                ParentCategoryId = null,
                Type = TransactionType.Income
            };

            var category = new CategoryResponse
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ParentCategoryId = request.ParentCategoryId,
                Status = Status.Active,
                Type = request.Type
            };

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(request, default))
                       .ReturnsAsync(Result<CategoryResponse>.Success(category));

            // Act
            var response = await _controller.Create(request) as CreatedResult;
            var value = response.Value as CategoryResponse;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal(category, value);
            Assert.Equal($"/api/categories/{category.Id}", response.Location);
        }

        [Fact]
        public async Task Update_ReturnBadRequest()
        {
            // Arrange
            var request = new UpdateCategoryCommand
            {
                Id = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(50),
                Status = Status.Active
            };

            var errorMessage = Guid.NewGuid().ToString();

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(request, default))
                       .ReturnsAsync(Result<CategoryResponse>.Fail(errorMessage));

            // Act
            var response = await _controller.Update(request) as ObjectResult;
            var value = response.Value as ErrorModel;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, value.Errors.First());
        }

        [Fact]
        public async Task Update_ReturnNoContent()
        {
            // Arrange
            var request = new UpdateCategoryCommand
            {
                Id = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(50),
                Status = Status.Active
            };

            var category = new CategoryResponse
            {
                Id = request.Id,
                Name = request.Name,
                ParentCategoryId = null,
                Status = request.Status,
                Type = TransactionType.Income
            };

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(request, default))
                       .ReturnsAsync(Result<CategoryResponse>.Success(category));

            // Act
            var response = await _controller.Update(request) as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnBadRequest()
        {
            // Arrange
            var request = new DeleteCategoryCommand(Guid.NewGuid());

            var errorMessage = Guid.NewGuid().ToString();

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<DeleteCategoryCommand>(x => x.Id == request.Id), default))
                       .ReturnsAsync(Result.Fail(errorMessage));

            // Act
            var response = await _controller.Delete(request.Id) as ObjectResult;
            var value = response.Value as ErrorModel;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, value.Errors.First());
        }

        [Fact]
        public async Task Delete_ReturnNoContent()
        {
            // Arrange
            var request = new DeleteCategoryCommand(Guid.NewGuid());

            var errorMessage = Guid.NewGuid().ToString();

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<DeleteCategoryCommand>(x => x.Id == request.Id), default))
                       .ReturnsAsync(Result.Success());

            // Act
            var response = await _controller.Delete(request.Id) as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }
    }
}