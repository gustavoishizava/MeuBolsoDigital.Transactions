using System;
using System.Linq;
using Bogus;
using MBD.Transactions.Application.Commands.Transactions.Create;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Transactions.Create
{
    public class CreateTransactionCommandTests
    {
        private readonly Faker _faker;

        public CreateTransactionCommandTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void InvalidCommand_ReturnErrors()
        {
            // Arrange
            var command = new CreateTransactionCommand(Guid.Empty,
                                                       Guid.Empty,
                                                       DateTime.MinValue,
                                                       DateTime.MinValue,
                                                       DateTime.MinValue,
                                                       -1,
                                                       _faker.Random.AlphaNumeric(101));


            // Act
            var validation = command.Validate();

            // Assert
            var properties = validation.Errors.Select(x => x.PropertyName);

            Assert.Equal(7, validation.Errors.Count);
            Assert.Contains(nameof(CreateTransactionCommand.BankAccountId), properties);
            Assert.Contains(nameof(CreateTransactionCommand.CategoryId), properties);
            Assert.Contains(nameof(CreateTransactionCommand.ReferenceDate), properties);
            Assert.Contains(nameof(CreateTransactionCommand.DueDate), properties);
            Assert.Contains(nameof(CreateTransactionCommand.PaymentDate), properties);
            Assert.Contains(nameof(CreateTransactionCommand.Value), properties);
            Assert.Contains(nameof(CreateTransactionCommand.Description), properties);
        }

        [Fact]
        public void ValidCommand_ReturnSuccess()
        {
            // Arrange
            var command = new CreateTransactionCommand(Guid.NewGuid(),
                                                       Guid.NewGuid(),
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       _faker.Random.Decimal(0, 10000),
                                                       _faker.Random.AlphaNumeric(100));


            // Act
            var validation = command.Validate();

            // Assert
            Assert.True(validation.IsValid);
        }
    }
}