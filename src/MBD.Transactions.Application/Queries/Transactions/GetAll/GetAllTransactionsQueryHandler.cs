using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;

namespace MBD.Transactions.Application.Queries.Transactions.GetAll
{
    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, IEnumerable<TransactionResponse>>
    {
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _repository;

        public GetAllTransactionsQueryHandler(IMapper mapper, ITransactionRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IEnumerable<TransactionResponse>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<List<TransactionResponse>>(await _repository.GetAllAsync());
        }
    }
}