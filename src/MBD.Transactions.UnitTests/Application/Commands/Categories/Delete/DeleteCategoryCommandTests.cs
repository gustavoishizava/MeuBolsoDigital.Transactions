using System;
using System.Linq;
using MBD.Transactions.Application.Commands.Categories.Delete;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Categories.Delete
{
    public class DeleteCategoryCommandTests
    {
        [Fact]
        public void InvalidId_ReturnError()
        {
            // Arrange
            var command = new DeleteCategoryCommand(Guid.Empty);

            // Act
            var validation = command.Validate();

            // Assert
            Assert.False(validation.IsValid);
            Assert.Single(validation.Errors);
            Assert.Equal(nameof(DeleteCategoryCommand.Id), validation.Errors.First().PropertyName);
        }

        [Fact]
        public void ValidId_ReturnSuccess()
        {
            // Arrange
            var command = new DeleteCategoryCommand(Guid.NewGuid());

            // Act
            var validation = command.Validate();

            // Assert
            Assert.True(validation.IsValid);
        }
    }
}