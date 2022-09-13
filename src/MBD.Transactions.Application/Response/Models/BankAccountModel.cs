using MBD.Transactions.Domain.Entities;

namespace MBD.Transactions.Application.Response.Models
{
    public class BankAccountModel
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public BankAccountModel(BankAccount bankAccount)
        {
            Id = bankAccount.Id.ToString();
            Description = bankAccount.Description;
        }
    }
}