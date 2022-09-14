using System;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MBD.Transactions.Infrastructure.Context;
using MongoDB.Driver;

namespace MBD.Transactions.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly TransactionContext _context;

        public BankAccountRepository(TransactionContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BankAccount bankAccount)
        {
            await _context.BankAccounts.AddAsync(bankAccount);
        }

        public async Task<BankAccount> GetByIdAsync(Guid id)
        {
            return await _context.BankAccounts.Collection.Find(Builders<BankAccount>.Filter.Where(x => x.Id == id)).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(BankAccount bankAccount)
        {
            await _context.BankAccounts.UpdateAsync(Builders<BankAccount>.Filter.Where(x => x.Id == bankAccount.Id), bankAccount);
        }
    }
}