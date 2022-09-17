using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using MeuBolsoDigital.CrossCutting.Extensions;

namespace MBD.Transactions.Application.Commands.Categories
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, IResult>
    {
        private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryCommandHandler(ICategoryRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result.Fail(validation.ToString());

            var category = await _repository.GetByIdAsync(request.Id);
            if (category == null)
                return Result.Fail("Categoria inválida.");

            await UpdateSubcategoriesAsync(category);
            await _repository.RemoveAsync(category);

            await _unitOfWork.CommitAsync();

            return Result.Success();
        }

        private async Task UpdateSubcategoriesAsync(Category categoryToDelete)
        {
            if (categoryToDelete.SubCategories.IsNullOrEmpty())
                return;

            var categoriesToUpdate = categoryToDelete.SubCategories.ToList();
            foreach (var category in categoriesToUpdate)
                category.ClearCategoryParentId();

            await _repository.UpdateRangeAsync(categoriesToUpdate);
        }
    }
}