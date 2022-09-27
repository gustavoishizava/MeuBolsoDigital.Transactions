using System;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;

namespace MBD.Transactions.Application.Response
{
    public class TransactionResponse
    {
        public Guid Id { get; init; }
        public BankAccountResponse BankAccount { get; init; }
        public SimpleCategoryResponse Category { get; init; }
        public DateTime ReferenceDate { get; init; }
        public DateTime DueDate { get; init; }
        public DateTime? PaymentDate { get; init; }
        public TransactionStatus Status { get; init; }
        public decimal Value { get; init; }
        public string Description { get; init; }
        public TransactionType Type { get; init; }

        public TransactionResponse(Transaction transaction)
        {
            Id = transaction.Id;
            BankAccount = new BankAccountResponse(transaction.BankAccount.Id, transaction.BankAccount.Description);
            Category = new SimpleCategoryResponse(transaction.Category.Id, transaction.Category.Name);
            ReferenceDate = transaction.ReferenceDate;
            DueDate = transaction.DueDate;
            PaymentDate = transaction.PaymentDate;
            Value = transaction.Value;
            Description = transaction.Description;
            Status = transaction.Status;
            Type = transaction.Category.Type;
        }

        public TransactionResponse()
        {
        }
    }
}