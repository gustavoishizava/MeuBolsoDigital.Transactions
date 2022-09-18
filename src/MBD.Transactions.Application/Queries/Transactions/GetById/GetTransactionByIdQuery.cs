using System;
using MBD.Transactions.Application.Response;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Transactions.Application.Queries.Transactions.GetById
{
    public class GetTransactionByIdQuery : IRequest<IResult<TransactionResponse>>
    {
        public Guid Id { get; init; }

        public GetTransactionByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}