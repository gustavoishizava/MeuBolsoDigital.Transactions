using System;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;

namespace MBD.Transactions.Application.Response
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime ReferenceDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public TransactionStatus Status { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }

        public TransactionResponse(Transaction transaction)
        {
            Id = transaction.Id;
            BankAccountId = transaction.BankAccount.Id;
            CategoryId = transaction.Category.Id;
            ReferenceDate = transaction.ReferenceDate;
            DueDate = transaction.DueDate;
            PaymentDate = transaction.PaymentDate;
            Value = transaction.Value;
            Description = transaction.Description;
            Status = transaction.Status;
        }

        public TransactionResponse()
        {
        }
    }
}