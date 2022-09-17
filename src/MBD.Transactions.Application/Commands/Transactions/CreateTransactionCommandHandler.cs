using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Application.Commands.Transactions
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, IResult<TransactionResponse>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBankAccountRepository _bankAccountRepository;

        public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, ILoggedUser loggedUser, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository, IBankAccountRepository bankAccountRepository)
        {
            _transactionRepository = transactionRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<IResult<TransactionResponse>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result<TransactionResponse>.Fail(validation.Errors.ToString());

            var bankAccount = await _bankAccountRepository.GetByIdAsync(request.BankAccountId);
            if (bankAccount == null)
                return Result<TransactionResponse>.Fail("Conta bancária inválida.");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<TransactionResponse>.Fail("Categoria inválida.");

            var transaction = new Transaction(
                _loggedUser.UserId,
                bankAccount,
                category,
                request.ReferenceDate,
                request.DueDate,
                request.Value,
                request.Description,
                request.PaymentDate
            );

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.CommitAsync();

            return Result<TransactionResponse>.Success(new TransactionResponse(transaction));
        }
    }
}