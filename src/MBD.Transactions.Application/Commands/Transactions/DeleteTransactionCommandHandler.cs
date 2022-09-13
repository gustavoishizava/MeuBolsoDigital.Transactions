using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Events;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Application.Commands.Transactions
{
    public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, IResult>
    {
        private readonly ITransactionRepository _repository;
        private readonly IUnitOfWork _unitOfwork;

        public DeleteTransactionCommandHandler(ITransactionRepository repository, IUnitOfWork unitOfwork)
        {
            _repository = repository;
            _unitOfwork = unitOfwork;
        }

        public async Task<IResult> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result.Fail(validation.ToString());

            var transaction = await _repository.GetByIdAsync(request.Id);
            if (transaction is null)
                return Result.Fail("Transação inválida.");

            transaction.AddDomainEvent(new TransactionDeletedDomainEvent(transaction.Id));
            await _repository.RemoveAsync(transaction);
            await _unitOfwork.CommitAsync();

            return Result.Success();
        }
    }
}