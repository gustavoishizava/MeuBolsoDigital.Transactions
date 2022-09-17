using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Application.Commands.Categories.Create
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResult<CategoryResponse>>
    {
        private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(ICategoryRepository repository, IUnitOfWork unitOfWork, ILoggedUser loggedUser, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _loggedUser = loggedUser;
            _mapper = mapper;
        }

        public async Task<IResult<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result<CategoryResponse>.Fail(validation.ToString());

            Category category = null;
            if (request.ParentCategoryId != null)
            {
                var parentCategory = await _repository.GetByIdAsync(request.ParentCategoryId.Value);
                if (parentCategory == null)
                    return Result<CategoryResponse>.Fail("Categoria pai inválida.");

                if (!parentCategory.CanHaveSubCategories())
                    return Result<CategoryResponse>.Fail("Não é permitido adicionar uma subcategoria à uma categoria filha.");

                category = parentCategory.AddSubCategory(request.Name);
            }
            else
            {
                category = new Category(_loggedUser.UserId, request.Name, request.Type);
            }

            await _repository.AddAsync(category);
            await _unitOfWork.CommitAsync();

            return Result<CategoryResponse>.Success(_mapper.Map<CategoryResponse>(category));
        }
    }
}