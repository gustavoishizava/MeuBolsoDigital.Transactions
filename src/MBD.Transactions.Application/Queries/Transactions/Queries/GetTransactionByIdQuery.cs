using System;
using MBD.Transactions.Application.Response.Models;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Transactions.Application.Queries.Transactions.Queries
{
    public class GetTransactionByIdQuery : IRequest<IResult<TransactionModel>>
    {
        public Guid Id { get; init; }

        public GetTransactionByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}