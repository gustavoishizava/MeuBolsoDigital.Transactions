using MBD.Transactions.Application.Response;
using MediatR;

namespace MBD.Transactions.Application.Queries.Categories.GetAll
{
    public class GetAllCategoriesQuery : IRequest<CategoryByTypeResponse>
    {
    }
}