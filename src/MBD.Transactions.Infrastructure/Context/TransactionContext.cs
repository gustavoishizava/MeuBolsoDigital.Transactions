using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using DotNet.MongoDB.Context.Context.ModelConfiguration;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Entities.Common;
using MBD.Transactions.Infrastructure.Extensions;
using MediatR;
using MeuBolsoDigital.IntegrationEventLog;

namespace MBD.Transactions.Infrastructure.Context
{
    [ExcludeFromCodeCoverage]
    public class TransactionContext : DbContext
    {
        private readonly IMediator _mediator;

        public TransactionContext(MongoDbContextOptions options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<IntegrationEventLogEntry> IntegrationEventLogEntries { get; set; }

        protected override void OnModelConfiguring(ModelBuilder modelBuilder)
        {
            modelBuilder.AddModelMap<BaseEntity>(map =>
            {
                map.MapIdProperty(x => x.Id);

                map.MapProperty(x => x.CreatedAt)
                    .SetElementName("created_at");

                map.MapProperty(x => x.UpdatedAt)
                    .SetElementName("updated_at");
            });

            modelBuilder.AddModelMap<BaseEntityWithEvent>();

            modelBuilder.AddModelMap<BankAccount>("bank_accounts", map =>
            {
                map.MapIdProperty(x => x.Id);

                map.MapProperty(x => x.TenantId)
                    .SetElementName("tenant_id");

                map.MapProperty(x => x.Description)
                    .SetElementName("description");
            });

            modelBuilder.AddModelMap<Category>("categories", map =>
            {
                map.MapProperty(x => x.TenantId)
                    .SetElementName("tenant_id");

                map.MapProperty(x => x.ParentCategoryId)
                    .SetElementName("parent_category_id");

                map.MapProperty(x => x.Name)
                    .SetElementName("name");

                map.MapProperty(x => x.Type)
                    .SetElementName("type");

                map.MapProperty(x => x.Status)
                    .SetElementName("status");

                map.MapField("_subCategories")
                    .SetShouldSerializeMethod(x => false);
            });

            modelBuilder.AddModelMap<Transaction>("transactions", map =>
            {
                map.MapProperty(x => x.TenantId)
                    .SetElementName("tenant_id");

                map.MapProperty(x => x.BankAccount)
                    .SetElementName("bank_account");

                map.MapProperty(x => x.Category)
                    .SetElementName("category");

                map.MapProperty(x => x.CreditCardBillId)
                    .SetElementName("credit_card_bill_id");

                map.MapProperty(x => x.ReferenceDate)
                    .SetElementName("reference_date");

                map.MapProperty(x => x.DueDate)
                    .SetElementName("due_date");

                map.MapProperty(x => x.PaymentDate)
                    .SetElementName("payment_date");

                map.MapProperty(x => x.Status)
                    .SetElementName("status");

                map.MapProperty(x => x.Value)
                    .SetElementName("value");

                map.MapProperty(x => x.Description)
                    .SetElementName("description");
            });

            modelBuilder.AddModelMap<IntegrationEventLogEntry>("integration_event_log_entries", mapConfig =>
            {
                mapConfig.MapIdProperty(x => x.Id);
                mapConfig.MapProperty(x => x.CreatedAt).SetElementName("created_at");
                mapConfig.MapProperty(x => x.UpdatedAt).SetElementName("updated_at");
                mapConfig.MapProperty(x => x.EventTypeName).SetElementName("entity_type_name");
                mapConfig.MapProperty(x => x.Content).SetElementName("content");
                mapConfig.MapProperty(x => x.State).SetElementName("state");
            });
        }

        public override async Task CommitAsync()
        {
            await _mediator.DispatchDomainEventsAsync(this);
            await base.CommitAsync();
        }
    }
}