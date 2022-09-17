using System.Diagnostics.CodeAnalysis;

namespace MBD.Transactions.API.Workers
{
    [ExcludeFromCodeCoverage]
    public static class RabbitMqConstants
    {
        public const string BANK_ACCOUNT_CREATED = "TOPIC/bank_accounts.created";
    }
}