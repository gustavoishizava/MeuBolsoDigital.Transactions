using System.Collections.Generic;
using MBD.Transactions.Application.Response;
using MediatR;

namespace MBD.Transactions.Application.Queries.Transactions.GetAll
{
    public class GetAllTransactionsQuery : IRequest<IEnumerable<TransactionResponse>>
    {
    }
}