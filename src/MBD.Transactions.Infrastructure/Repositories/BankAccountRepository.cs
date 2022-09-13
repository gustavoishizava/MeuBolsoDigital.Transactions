using System;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;

namespace MBD.Transactions.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        public void Add(BankAccount bankAccount)
        {
            throw new NotImplementedException();
        }

        public Task<BankAccount> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(BankAccount bankAccount)
        {
            throw new NotImplementedException();
        }
    }
}