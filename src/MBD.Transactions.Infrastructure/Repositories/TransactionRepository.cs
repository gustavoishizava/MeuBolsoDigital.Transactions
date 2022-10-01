using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MBD.Transactions.Infrastructure.Context;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MongoDB.Driver;

namespace MBD.Transactions.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionContext _context;
        private readonly ILoggedUser _loggedUser;

        public TransactionRepository(TransactionContext context, ILoggedUser loggedUser)
        {
            _context = context;
            _loggedUser = loggedUser;
        }

        public async Task AddAsync(Transaction entity)
        {
            await _context.Transactions.AddAsync(entity);
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.Collection.Find(Builders<Transaction>.Filter.Where(x => x.TenantId == _loggedUser.UserId)).ToListAsync();
        }

        public async Task<Transaction> GetByIdAsync(Guid id)
        {
            return await _context.Transactions.Collection.Find(Builders<Transaction>.Filter.Where(x => x.Id == id && x.TenantId == _loggedUser.UserId)).FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(Transaction entity)
        {
            await _context.Transactions.RemoveAsync(Builders<Transaction>.Filter.Where(x => x.Id == entity.Id), entity);
        }

        public async Task UpdateAsync(Transaction entity)
        {
            await _context.Transactions.UpdateAsync(Builders<Transaction>.Filter.Where(x => x.Id == entity.Id), entity);
        }

        public async Task UpdateBankAccountDescriptionAsync(Guid bankAccountId, string description)
        {
            var filter = Builders<Transaction>.Filter.Where(x => x.BankAccount.Id == bankAccountId);
            var update = Builders<Transaction>.Update.Set(x => x.BankAccount.Description, description);

            await _context.Transactions.Collection.UpdateManyAsync(_context.ClientSessionHandle, filter, update);
        }

        public async Task UpdateCategoryNameAsync(Guid categoryId, string name)
        {
            var filter = Builders<Transaction>.Filter.Where(x => x.Category.Id == categoryId);
            var update = Builders<Transaction>.Update.Set(x => x.Category.Name, name);

            await _context.Transactions.Collection.UpdateManyAsync(_context.ClientSessionHandle, filter, update);
        }
    }
}