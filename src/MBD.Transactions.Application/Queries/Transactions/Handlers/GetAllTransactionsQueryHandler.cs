using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.Queries.Transactions.Queries;
using MBD.Transactions.Application.Response.Models;
using MediatR;

namespace MBD.Transactions.Application.Queries.Transactions.Handlers
{
    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, IEnumerable<TransactionModel>>
    {
        public Task<IEnumerable<TransactionModel>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}