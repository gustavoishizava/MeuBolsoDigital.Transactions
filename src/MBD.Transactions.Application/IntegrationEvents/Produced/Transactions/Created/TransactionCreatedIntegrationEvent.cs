using System;
using MBD.Transactions.Domain.Enumerations;

namespace MBD.Transactions.Application.IntegrationEvents.Produced.Transactions.Created
{
    public class TransactionCreatedIntegrationEvent
    {
        public Guid Id { get; init; }
        public Guid TenantId { get; init; }
        public Guid BankAccountId { get; init; }
        public Guid CategoryId { get; init; }
        public DateTime ReferenceDate { get; init; }
        public DateTime DueDate { get; init; }
        public DateTime? PaymentDate { get; init; }
        public TransactionStatus Status { get; init; }
        public decimal Value { get; init; }
        public string Description { get; init; }
        public TransactionType Type { get; init; }

        public TransactionCreatedIntegrationEvent(Guid id, Guid tenantId, Guid bankAccountId, Guid categoryId, DateTime referenceDate, DateTime dueDate, DateTime? paymentDate, TransactionStatus status, decimal value, string description, TransactionType type)
        {
            Id = id;
            TenantId = tenantId;
            BankAccountId = bankAccountId;
            CategoryId = categoryId;
            ReferenceDate = referenceDate;
            DueDate = dueDate;
            PaymentDate = paymentDate;
            Status = status;
            Value = value;
            Description = description;
            Type = type;
        }
    }
}