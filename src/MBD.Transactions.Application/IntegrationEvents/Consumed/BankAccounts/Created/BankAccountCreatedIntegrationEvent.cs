using System;
using MediatR;

namespace MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.Created
{
    public class BankAccountCreatedIntegrationEvent : INotification
    {
        public Guid Id { get; init; }
        public Guid TenantId { get; init; }
        public string Description { get; init; }
    }
}