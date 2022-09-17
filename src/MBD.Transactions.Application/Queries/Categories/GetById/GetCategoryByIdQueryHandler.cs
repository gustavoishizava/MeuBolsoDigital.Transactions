using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Transactions.Application.Queries.Categories.GetById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, IResult<CategoryResponse>>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IResult<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetByIdAsync(request.Id);
            if (category == null)
                return Result<CategoryResponse>.Fail("Categoria inválida.");

            return Result<CategoryResponse>.Success(_mapper.Map<CategoryResponse>(category));
        }
    }
}