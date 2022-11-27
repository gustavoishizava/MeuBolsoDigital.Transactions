using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.Transactions.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.Transactions.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class TransactionMapping : BsonClassMapConfiguration
    {
        public TransactionMapping() : base("transactions")
        {
        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<Transaction>();

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

            return map;
        }
    }
}