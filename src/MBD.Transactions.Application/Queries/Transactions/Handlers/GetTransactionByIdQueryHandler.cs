using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.MongoDbSettings;
using MBD.Transactions.Application.Queries.Transactions.Queries;
using MBD.Transactions.Application.Response.Models;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Transactions.Application.Queries.Transactions.Handlers
{
    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, IResult<TransactionModel>>
    {
        public Task<IResult<TransactionModel>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}