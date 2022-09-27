using System;

namespace MBD.Transactions.Application.Response
{
    public class BankAccountResponse
    {
        public Guid Id { get; init; }
        public string Description { get; init; }

        public BankAccountResponse(Guid id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}