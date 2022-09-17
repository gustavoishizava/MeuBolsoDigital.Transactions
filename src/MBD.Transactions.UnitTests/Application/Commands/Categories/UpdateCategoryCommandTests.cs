using System;
using System.Linq;
using Bogus;
using MBD.Transactions.Application.Commands.Categories;
using MBD.Transactions.Domain.Enumerations;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Categories
{
    public class UpdateCategoryCommandTests
    {
        private readonly Faker _faker;

        public UpdateCategoryCommandTests()
        {
            _faker = new Faker();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_EmptyName_ReturnError(string name)
        {
            // Arrange
            var command = new UpdateCategoryCommand
            {
                Id = Guid.NewGuid(),
                Name = name,
                Status = Status.Active
            };

            // Act
            var validation = command.Validate();

            // Assert
            Assert.Single(validation.Errors);
            Assert.Equal(nameof(command.Name), validation.Errors.First().PropertyName);
        }

        [Fact]
        public void Validate_InvalidLengthName_ReturnError()
        {
            // Arrange
            var command = new UpdateCategoryCommand
            {
                Id = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(101),
                Status = Status.Active
            };

            // Act
            var validation = command.Validate();

            // Assert
            Assert.Single(validation.Errors);
            Assert.Equal(nameof(command.Name), validation.Errors.First().PropertyName);
        }

        [Fact]
        public void Validate_GuidEmpty_ReturnError()
        {
            // Arrange
            var command = new UpdateCategoryCommand
            {
                Id = Guid.Empty,
                Name = "Category",
                Status = Status.Active
            };

            // Act
            var validation = command.Validate();

            // Assert
            Assert.Single(validation.Errors);
            Assert.Equal(nameof(command.Id), validation.Errors.First().PropertyName);
        }

        [Fact]
        public void Validate_ReturnSuccess()
        {
            // Arrange
            var command = new UpdateCategoryCommand
            {
                Id = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(100),
                Status = Status.Active
            };

            // Act
            var validation = command.Validate();

            // Assert
            Assert.True(validation.IsValid);
        }
    }
}