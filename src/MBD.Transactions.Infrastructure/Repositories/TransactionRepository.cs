using System;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;

namespace MBD.Transactions.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public Task AddAsync(Transaction entity)
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Transaction entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Transaction entity)
        {
            throw new NotImplementedException();
        }
    }
}