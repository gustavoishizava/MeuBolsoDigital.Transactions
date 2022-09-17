using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.Created
{
    public class BankAccountCreatedIntegrationEventHandler : INotificationHandler<BankAccountCreatedIntegrationEvent>
    {
        private readonly IBankAccountRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public BankAccountCreatedIntegrationEventHandler(IBankAccountRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(BankAccountCreatedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var exists = await _repository.GetByIdAsync(notification.Id);
            if (exists is not null)
                return;

            var bankAccount = new BankAccount(notification.Id, notification.TenantId, notification.Description);
            await _repository.AddAsync(bankAccount);
            await _unitOfWork.CommitAsync();
        }
    }
}