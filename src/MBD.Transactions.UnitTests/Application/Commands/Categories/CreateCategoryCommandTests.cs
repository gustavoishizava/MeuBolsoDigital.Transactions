using System;
using System.Linq;
using Bogus;
using MBD.Transactions.Application.Commands.Categories;
using MBD.Transactions.Domain.Enumerations;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.Commands.Categories
{
    public class CreateCategoryCommandTests
    {
        private readonly Faker _faker;

        public CreateCategoryCommandTests()
        {
            _faker = new Faker();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_EmptyName_ReturnError(string name)
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                Name = name,
                Type = TransactionType.Income
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
            var command = new CreateCategoryCommand
            {
                Name = _faker.Random.AlphaNumeric(101),
                Type = TransactionType.Income
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
            var command = new CreateCategoryCommand
            {
                ParentCategoryId = Guid.Empty,
                Name = "Category",
                Type = TransactionType.Income
            };

            // Act
            var validation = command.Validate();

            // Assert
            Assert.Single(validation.Errors);
            Assert.Equal(nameof(command.ParentCategoryId), validation.Errors.First().PropertyName);
        }

        [Fact]
        public void Validate_ReturnSuccess()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                ParentCategoryId = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(100),
                Type = TransactionType.Expense
            };

            // Act
            var validation = command.Validate();

            // Assert
            Assert.True(validation.IsValid);
        }
    }
}