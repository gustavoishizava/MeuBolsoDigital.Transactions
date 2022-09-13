using System;
using MBD.Core;
using MBD.Core.Entities;
using MBD.Transactions.Domain.Entities.Common;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Events;

namespace MBD.Transactions.Domain.Entities
{
    public class Transaction : BaseEntityWithEvent, IAggregateRoot
    {
        public Guid TenantId { get; private init; }
        public BankAccount BankAccount { get; private set; }
        public Category Category { get; private set; }
        public Guid? CreditCardBillId { get; private set; }
        public DateTime ReferenceDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime? PaymentDate { get; private set; }
        public TransactionStatus Status { get; private set; }
        public decimal Value { get; private set; }
        public string Description { get; private set; }

        public bool ItsPaid => PaymentDate != null && Status == TransactionStatus.Paid;
        private bool _valueChanged { get; set; } = false;

        public Transaction(Guid tenantId, BankAccount bankAccount, Category category, DateTime referenceDate, DateTime dueDate, decimal value, string description, DateTime? paymentDate)
        {
            Assertions.IsGreaterOrEqualsThan(value, 0, "O valor não pode ser menor que 0.");

            TenantId = tenantId;
            BankAccount = bankAccount;
            Category = category;
            CreditCardBillId = null;
            ReferenceDate = referenceDate;
            DueDate = dueDate;
            PaymentDate = null;
            Status = TransactionStatus.AwaitingPayment;
            Value = value;
            Description = description;

            if (paymentDate is not null)
                Pay(paymentDate.Value);

            AddDomainEvent(new TransactionCreatedDomainEvent(this));
        }

        #region EF
        protected Transaction() { }
        #endregion

        private void Pay(DateTime paymentDate)
        {
            if (!ItsPaid || PaymentDate != paymentDate || _valueChanged)
                AddDomainEvent(new RealizedPaymentDomainEvent(Id, paymentDate, Value, BankAccount.Id, Category.Type));

            PaymentDate = paymentDate;
            Status = TransactionStatus.Paid;
        }

        private void UndoPayment()
        {
            if (!ItsPaid)
                return;

            PaymentDate = null;
            Status = TransactionStatus.AwaitingPayment;

            AddDomainEvent(new ReversedPaymentDomainEvent(Id));
        }

        public void SetValue(decimal value)
        {
            Assertions.IsGreaterOrEqualsThan(value, 0, "O valor não pode ser menor que 0.");

            _valueChanged = value != Value;
            if (_valueChanged)
                AddDomainEvent(new ValueChangedDomainEvent(Id, Value, value));

            Value = value;
        }

        public void Update(BankAccount bankAccount, Category category, DateTime referenceDate, DateTime dueDate, decimal value, string description, DateTime? paymentDate)
        {
            BankAccount = bankAccount;
            Category = category;
            ReferenceDate = referenceDate;
            DueDate = dueDate;
            SetValue(value);
            Description = description;

            if (paymentDate is null)
                UndoPayment();
            else
                Pay(paymentDate.Value);

            AddDomainEvent(new TransactionUpdatedDomainEvent(this));
        }

        public void LinkCreditCardBill(Guid creditCardBillId)
        {
            Assertions.IsNull(CreditCardBillId, "A transação já possui fatura de cartão de crédito vinculada.");
            Assertions.IsNotEmpty(creditCardBillId, "O Id da fatura do cartão de crédito está inválido.");
            Assertions.IsFalse(ItsPaid, "Não é possível vincular uma fatura de cartão de crédito a uma transação já paga.");
            Assertions.IsTrue(Category.Type == TransactionType.Expense, "Não é possível vincular uma fatura de cartão de crédito a uma transação de receita.");

            CreditCardBillId = creditCardBillId;
            AddDomainEvent(new LinkedToCreditCardBillDomainEvent(Id, BankAccount.Id, CreditCardBillId.Value, CreatedAt, Value));
        }

        public void UnlinkCreditCardBill()
        {
            if (CreditCardBillId is null)
                return;

            AddDomainEvent(new UnlinkedToCreditCardBillDomainEvent(Id, BankAccount.Id, CreditCardBillId.Value));
            CreditCardBillId = null;
        }
    }
}