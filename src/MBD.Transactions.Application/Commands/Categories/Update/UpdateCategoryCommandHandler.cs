using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Application.Commands.Categories.Update
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResult>
    {
        private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(ICategoryRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result.Fail(validation.ToString());

            var category = await _repository.GetByIdAsync(request.Id);
            if (category == null)
                return Result.Fail("Categoria inválida.");

            category.ChangeName(request.Name);
            if (request.Status == Status.Active)
                category.Activate();
            else
                category.Deactivate();

            await _repository.UpdateAsync(category);
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }
    }
}