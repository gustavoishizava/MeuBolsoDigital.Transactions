using System;
using MediatR;

namespace MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.DescriptionChanged
{
    public class BankAccountDescriptionChangedIntegrationEvent : INotification
    {
        public Guid Id { get; init; }
        public string NewDescription { get; init; }
        public string OldDescription { get; init; }
    }
}