using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using MBD.Transactions.API.Controllers;
using MBD.Transactions.Application.Queries.Transactions.GetAll;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Enumerations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Xunit;
using MBD.Transactions.Application.Queries.Transactions.GetById;
using MeuBolsoDigital.Application.Utils.Responses;
using MBD.Transactions.Application.Commands.Transactions.Create;
using MBD.Transactions.API.Models;
using MBD.Transactions.Application.Commands.Transactions.Update;
using MBD.Transactions.Application.Commands.Transactions.Delete;

namespace MBD.Transactions.UnitTests.API.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly TransactionsController _controller;

        public TransactionsControllerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();
            _controller = _autoMocker.CreateInstance<TransactionsController>();
        }

        [Fact]
        public async Task GetAll_Empty_ReturnNoContent()
        {
            // Arrante && Act
            var response = await _controller.GetAll() as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            _autoMocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<GetAllTransactionsQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnOk()
        {
            // Arrange
            var transaction = new TransactionResponse
            {
                Id = Guid.NewGuid(),
                BankAccountId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                ReferenceDate = DateTime.Now,
                DueDate = DateTime.Now,
                PaymentDate = DateTime.Now,
                Status = TransactionStatus.AwaitingPayment,
                Value = _faker.Random.Decimal(1, 100),
                Description = _faker.Random.AlphaNumeric(100)
            };

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.IsAny<GetAllTransactionsQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new List<TransactionResponse>() { transaction });

            // Act
            var response = await _controller.GetAll() as ObjectResult;
            var value = response.Value as List<TransactionResponse>;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.NotNull(value);
            Assert.Equal(transaction, value.First());
        }

        [Fact]
        public async Task GetById_ReturnNotFound()
        {
            // Arrange
            var query = new GetTransactionByIdQuery(Guid.NewGuid());

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<GetTransactionByIdQuery>(x => x.Id == query.Id), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Result<TransactionResponse>.Fail());

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
            var query = new GetTransactionByIdQuery(Guid.NewGuid());

            var transaction = new TransactionResponse
            {
                Id = query.Id,
                BankAccountId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                ReferenceDate = DateTime.Now,
                DueDate = DateTime.Now,
                PaymentDate = DateTime.Now,
                Status = TransactionStatus.AwaitingPayment,
                Value = _faker.Random.Decimal(1, 100),
                Description = _faker.Random.AlphaNumeric(100)
            };

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<GetTransactionByIdQuery>(x => x.Id == query.Id), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Result<TransactionResponse>.Success(transaction));

            // Act
            var response = await _controller.GetById(query.Id) as ObjectResult;
            var value = response.Value as TransactionResponse;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(transaction, value);
        }

        [Fact]
        public async Task Create_ReturnBadRequest()
        {
            // Arrange
            var request = new CreateTransactionCommand(Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(1, 100),
                                                       _faker.Random.AlphaNumeric(100));

            var errorMessage = Guid.NewGuid().ToString();

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(request, default))
                       .ReturnsAsync(Result<TransactionResponse>.Fail(errorMessage));

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
            var request = new CreateTransactionCommand(Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(1, 100),
                                                       _faker.Random.AlphaNumeric(100));

            var transaction = new TransactionResponse
            {
                Id = Guid.NewGuid(),
                BankAccountId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                ReferenceDate = DateTime.Now,
                DueDate = DateTime.Now,
                PaymentDate = DateTime.Now,
                Status = TransactionStatus.AwaitingPayment,
                Value = _faker.Random.Decimal(1, 100),
                Description = _faker.Random.AlphaNumeric(100)
            };

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(request, default))
                       .ReturnsAsync(Result<TransactionResponse>.Success(transaction));

            // Act
            var response = await _controller.Create(request) as CreatedResult;
            var value = response.Value as TransactionResponse;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal(transaction, value);
            Assert.Equal($"/api/transactions/{transaction.Id}", response.Location);
        }

        [Fact]
        public async Task Update_ReturnBadRequest()
        {
            // Arrange
            var request = new UpdateTransactionCommand(Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(1, 100),
                                                       _faker.Random.AlphaNumeric(100));

            var errorMessage = Guid.NewGuid().ToString();

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(request, default))
                       .ReturnsAsync(Result<TransactionResponse>.Fail(errorMessage));

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
            var request = new UpdateTransactionCommand(Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(1, 100),
                                                       _faker.Random.AlphaNumeric(100));

            var transaction = new TransactionResponse
            {
                Id = Guid.NewGuid(),
                BankAccountId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                ReferenceDate = DateTime.Now,
                DueDate = DateTime.Now,
                PaymentDate = DateTime.Now,
                Status = TransactionStatus.AwaitingPayment,
                Value = _faker.Random.Decimal(1, 100),
                Description = _faker.Random.AlphaNumeric(100)
            };

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(request, default))
                       .ReturnsAsync(Result<TransactionResponse>.Success(transaction));

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
            var request = new DeleteTransactionCommand(Guid.NewGuid());

            var errorMessage = Guid.NewGuid().ToString();

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<DeleteTransactionCommand>(x => x.Id == request.Id), default))
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
            var request = new DeleteTransactionCommand(Guid.NewGuid());

            var errorMessage = Guid.NewGuid().ToString();

            _autoMocker.GetMock<IMediator>().Setup(x => x.Send(It.Is<DeleteTransactionCommand>(x => x.Id == request.Id), default))
                       .ReturnsAsync(Result.Success());

            // Act
            var response = await _controller.Delete(request.Id) as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }
    }
}