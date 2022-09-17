using System;
using System.Linq;
using MBD.Transactions.Application.Commands.Transactions;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Transactions.Delete
{
    public class DeleteTransactionCommandTests
    {
        [Fact]
        public void InvalidId_ReturnError()
        {
            // Arrange
            var command = new DeleteTransactionCommand(Guid.Empty);

            // Act
            var validation = command.Validate();

            // Assert
            Assert.False(validation.IsValid);
            Assert.Single(validation.Errors);
            Assert.Equal(nameof(DeleteTransactionCommand.Id), validation.Errors.First().PropertyName);
        }

        [Fact]
        public void ValidId_ReturnSuccess()
        {
            // Arrange
            var command = new DeleteTransactionCommand(Guid.NewGuid());

            // Act
            var validation = command.Validate();

            // Assert
            Assert.True(validation.IsValid);
        }
    }
}