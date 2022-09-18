using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Entities;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Domain.Interfaces.Repositories
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        Task<List<Transaction>> GetAllAsync();
    }
}