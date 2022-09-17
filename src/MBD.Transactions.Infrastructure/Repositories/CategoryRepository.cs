using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.MongoDB.Context.Context.Operations;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MBD.Transactions.Infrastructure.Context;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MongoDB.Driver;

namespace MBD.Transactions.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TransactionContext _context;
        private readonly ILoggedUser _loggedUser;

        public CategoryRepository(TransactionContext context, ILoggedUser loggedUser)
        {
            _context = context;
            _loggedUser = loggedUser;
        }

        public async Task AddAsync(Category entity)
        {
            await _context.Categories.AddAsync(entity);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(bool includeSubCategories = true)
        {
            var query = _context.Categories.Collection.Aggregate<Category>()
                                                      .Match(x => x.TenantId == _loggedUser.UserId && x.ParentCategoryId == null);
            if (includeSubCategories)
                query = query.Lookup<Category, Category>("categories", "_id", "parent_category_id", "_subCategories");

            return await query.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            return await _context.Categories.Collection.Aggregate<Category>()
                                                       .Match(x => x.Id == id && x.TenantId == _loggedUser.UserId)
                                                       .Lookup<Category, Category>("categories", "_id", "parent_category_id", "_subCategories")
                                                       .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>> GetByTypeAsync(TransactionType type, bool includeSubCategories = true)
        {
            var query = _context.Categories.Collection.Aggregate<Category>()
                                                      .Match(x => x.TenantId == _loggedUser.UserId && x.ParentCategoryId == null && x.Type == type);
            if (includeSubCategories)
                query = query.Lookup<Category, Category>("categories", "_id", "parent_category_id", "_subCategories");

            return await query.ToListAsync();
        }

        public async Task RemoveAsync(Category entity)
        {
            await _context.Categories.RemoveAsync(Builders<Category>.Filter.Where(x => x.Id == entity.Id), entity);
        }

        public async Task UpdateAsync(Category entity)
        {
            await _context.Categories.UpdateAsync(Builders<Category>.Filter.Where(x => x.Id == entity.Id), entity);
        }

        public async Task UpdateRangeAsync(List<Category> categories)
        {
            var updates = new List<BulkOperationModel<Category>>();
            foreach (var category in categories)
                updates.Add(new(Builders<Category>.Filter.Where(x => x.Id == category.Id), category));

            await _context.Categories.UpdateRangeAsync(updates);
        }
    }
}