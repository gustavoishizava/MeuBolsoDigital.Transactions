using MBD.Transactions.Domain.Entities;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Domain.Interfaces.Repositories
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
    }
}