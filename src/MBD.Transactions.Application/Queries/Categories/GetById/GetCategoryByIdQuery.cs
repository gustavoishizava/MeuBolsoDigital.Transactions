using System;
using MBD.Transactions.Application.Response;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Transactions.Application.Queries.Categories.GetById
{
    public class GetCategoryByIdQuery : IRequest<IResult<CategoryResponse>>
    {
        public Guid Id { get; init; }

        public GetCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}