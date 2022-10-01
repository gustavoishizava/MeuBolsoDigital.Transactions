using System;
using System.Threading;
using Bogus;
using MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.DescriptionChanged;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.Transactions.UnitTests.Application.IntegrationEvents.Consumed.BankAccounts.DescriptionChanged
{
    public class BankAccountDescriptionChangedIntegrationEventHandlerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly BankAccountDescriptionChangedIntegrationEventHandler _handler;

        public BankAccountDescriptionChangedIntegrationEventHandlerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();
            _handler = _autoMocker.CreateInstance<BankAccountDescriptionChangedIntegrationEventHandler>();
        }

        [Fact]
        public async void Handle_BankAccountNotExists_DoNothing()
        {
            // Arrange
            var @event = new BankAccountDescriptionChangedIntegrationEvent
            {
                Id = Guid.NewGuid(),
                OldDescription = _faker.Random.AlphaNumeric(100),
                NewDescription = _faker.Random.AlphaNumeric(100)
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
                .Verify(x => x.AddAsync(It.IsAny<BankAccount>()), Times.Never);

            _autoMocker.GetMock<ITransactionRepository>()
                .Verify(x => x.UpdateBankAccountDescriptionAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async void Handle_BankAccountExists_Update()
        {
            // Arrange
            var @event = new BankAccountDescriptionChangedIntegrationEvent
            {
                Id = Guid.NewGuid(),
                OldDescription = _faker.Random.AlphaNumeric(100),
                NewDescription = _faker.Random.AlphaNumeric(100)
            };

            var bankAccount = new BankAccount(@event.Id, Guid.NewGuid(), @event.OldDescription);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(@event.Id))
                .ReturnsAsync(bankAccount);

            // Act
            await _handler.Handle(@event, new CancellationToken());

            // Assert
            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(@event.Id), Times.Once);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.UpdateAsync(It.Is<BankAccount>(x => x.Id == @event.Id && x.Description == @event.NewDescription && x.TenantId == bankAccount.TenantId)), Times.Once);

            _autoMocker.GetMock<ITransactionRepository>()
               .Verify(x => x.UpdateBankAccountDescriptionAsync(@event.Id, @event.NewDescription), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}