using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<IEnumerable<Category>> GetByTypeAsync(TransactionType type, bool includeSubCategories = true);
        Task<IEnumerable<Category>> GetAllAsync(bool includeSubCategories = true);
        Task UpdateRangeAsync(List<Category> categories);
    }
}