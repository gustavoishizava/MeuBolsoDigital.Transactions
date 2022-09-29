using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MBD.Transactions.Infrastructure.Context;
using MeuBolsoDigital.IntegrationEventLog;
using MeuBolsoDigital.IntegrationEventLog.Repositories;
using MongoDB.Driver;

namespace MBD.Transactions.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class IntegrationEventLogRepository : IIntegrationEventLogRepository
    {
        private readonly TransactionContext _context;

        public IntegrationEventLogRepository(TransactionContext context)
        {
            _context = context;
        }

        public async Task AddAsync(IntegrationEventLogEntry integrationEventLogEntry)
        {
            await _context.IntegrationEventLogEntries.AddAsync(integrationEventLogEntry);
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync()
        {
            var filter = Builders<IntegrationEventLogEntry>.Filter.Where(x => x.State == EventState.NotPublished);
            return await _context.IntegrationEventLogEntries.Collection.Find(filter).ToListAsync();
        }

        public async Task UpdateAsync(IntegrationEventLogEntry integrationEventLogEntry)
        {
            var filter = Builders<IntegrationEventLogEntry>.Filter.Where(x => x.Id == integrationEventLogEntry.Id);
            await _context.IntegrationEventLogEntries.UpdateAsync(filter, integrationEventLogEntry);
        }
    }
}