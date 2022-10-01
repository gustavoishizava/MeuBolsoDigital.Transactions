using System;
using System.Threading;
using Bogus;
using MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.Created;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.IntegrationEvents.Consumed.BankAccounts.Created
{
    public class BankAccountCreatedIntegrationEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly BankAccountCreatedIntegrationEventHandler _handler;

        public BankAccountCreatedIntegrationEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();
            _handler = _autoMocker.CreateInstance<BankAccountCreatedIntegrationEventHandler>();
        }

        [Fact]
        public async void Handle_BankAccountExists_DoNothing()
        {
            // Arrange
            var @event = new BankAccountCreatedIntegrationEvent
            {
                Id = Guid.NewGuid(),
                Description = _faker.Random.AlphaNumeric(100),
                TenantId = Guid.NewGuid()
            };

            var bankAccount = new BankAccount(@event.Id, @event.TenantId, @event.Description);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(@event.Id))
                .ReturnsAsync(bankAccount);

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(@event.Id), Times.Once);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.AddAsync(It.IsAny<BankAccount>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async void Handle_BankAccountNotExists_Save()
        {
            // Arrange
            var @event = new BankAccountCreatedIntegrationEvent
            {
                Id = Guid.NewGuid(),
                Description = _faker.Random.AlphaNumeric(100),
                TenantId = Guid.NewGuid()
            };

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(@event.Id))
                .ReturnsAsync((BankAccount)null);

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(@event.Id), Times.Once);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.AddAsync(It.Is<BankAccount>(x => x.Id == @event.Id && x.Description == @event.Description && x.TenantId == @event.TenantId)), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}