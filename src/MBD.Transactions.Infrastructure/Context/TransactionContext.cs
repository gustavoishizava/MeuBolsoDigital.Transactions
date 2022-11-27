using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Infrastructure.Extensions;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog;
using MongoDB.Driver;

namespace MBD.Transactions.Infrastructure.Context
{
    [ExcludeFromCodeCoverage]
    public class TransactionContext : DbContext
    {
        private readonly IMediator _mediator;

        public TransactionContext(IMongoClient mongoClient, IMongoDatabase mongoDatabase, MongoDbContextOptions options, IMediator mediator) : base(mongoClient, mongoDatabase, options)
        {
            _mediator = mediator;
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<IntegrationEventLogEntry> IntegrationEventLogEntries { get; set; }

        public override async Task CommitAsync()
        {
            await _mediator.DispatchDomainEventsAsync(this);
            await base.CommitAsync();
        }
    }
}