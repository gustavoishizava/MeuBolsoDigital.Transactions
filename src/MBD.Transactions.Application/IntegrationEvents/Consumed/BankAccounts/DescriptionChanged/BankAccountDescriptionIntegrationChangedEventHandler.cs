using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.DescriptionChanged
{
    public class BankAccountDescriptionChangedIntegrationEventHandler : INotificationHandler<BankAccountDescriptionChangedIntegrationEvent>
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BankAccountDescriptionChangedIntegrationEventHandler(IBankAccountRepository bankAccountRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _bankAccountRepository = bankAccountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(BankAccountDescriptionChangedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(notification.Id);
            if (bankAccount is null)
                return;

            bankAccount.SetDescription(notification.NewDescription);
            await _bankAccountRepository.UpdateAsync(bankAccount);
            await _transactionRepository.UpdateBankAccountDescriptionAsync(notification.Id, notification.NewDescription);

            await _unitOfWork.CommitAsync();
        }
    }
}