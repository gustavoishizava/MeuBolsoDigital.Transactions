using System;
using System.Linq;
using Bogus;
using MBD.Transactions.Application.Commands.Transactions.Update;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Transactions.Update
{
    public class UpdateTransactionCommandTests
    {
        private readonly Faker _faker;

        public UpdateTransactionCommandTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void InvalidCommand_ReturnErrors()
        {
            // Arrange
            var command = new UpdateTransactionCommand(Guid.Empty,
                                                       Guid.Empty,
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

            Assert.Equal(8, validation.Errors.Count);
            Assert.Contains(nameof(UpdateTransactionCommand.Id), properties);
            Assert.Contains(nameof(UpdateTransactionCommand.BankAccountId), properties);
            Assert.Contains(nameof(UpdateTransactionCommand.CategoryId), properties);
            Assert.Contains(nameof(UpdateTransactionCommand.ReferenceDate), properties);
            Assert.Contains(nameof(UpdateTransactionCommand.DueDate), properties);
            Assert.Contains(nameof(UpdateTransactionCommand.PaymentDate), properties);
            Assert.Contains(nameof(UpdateTransactionCommand.Value), properties);
            Assert.Contains(nameof(UpdateTransactionCommand.Description), properties);
        }

        [Fact]
        public void ValidCommand_ReturnSuccess()
        {
            // Arrange
            var command = new UpdateTransactionCommand(Guid.NewGuid(),
                                                       Guid.NewGuid(),
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