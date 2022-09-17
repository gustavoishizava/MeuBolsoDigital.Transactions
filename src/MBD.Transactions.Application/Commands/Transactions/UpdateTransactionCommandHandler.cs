using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Application.Commands.Transactions
{
    public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, IResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBankAccountRepository _bankAccountRepository;

        public UpdateTransactionCommandHandler(ITransactionRepository transactionRepository, ILoggedUser loggedUser, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository, IBankAccountRepository bankAccountRepository)
        {
            _transactionRepository = transactionRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<IResult> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result.Fail(validation.Errors.ToString());

            var transaction = await _transactionRepository.GetByIdAsync(request.Id);
            if (transaction == null)
                return Result.Fail("Transação inválida.");

            var bankAccount = await _bankAccountRepository.GetByIdAsync(request.BankAccountId);
            if (bankAccount == null)
                return Result<TransactionResponse>.Fail("Conta bancária inválida.");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<TransactionResponse>.Fail("Categoria inválida.");

            transaction.Update(bankAccount, category, request.ReferenceDate, request.DueDate, request.Value, request.Description, request.PaymentDate);

            await _transactionRepository.UpdateAsync(transaction);
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }
    }
}