using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Transactions.Application.Queries.Transactions.GetById
{
    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, IResult<TransactionResponse>>
    {
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _repository;

        public GetTransactionByIdQueryHandler(IMapper mapper, ITransactionRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IResult<TransactionResponse>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _repository.GetByIdAsync(request.Id);
            if (transaction is null)
                return Result<TransactionResponse>.Fail("Transação inválida.");

            return Result<TransactionResponse>.Success(_mapper.Map<TransactionResponse>(transaction));
        }
    }
}