using MBD.Infrastructure.Core.Configuration;
using MBD.Transactions.Domain.Entities;
using MBD.Transactions.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MBD.Transactions.Infrastructure.Context.Configuration
{
    public class TransactionConfiguration : BaseEntityConfiguration<Transaction>
    {
        public override void Configure(EntityTypeBuilder<Transaction> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.TenantId)
                .IsRequired();

            builder.Property(x => x.CreditCardBillId)
                .IsRequired(false);

            builder.Property(x => x.ReferenceDate)
                .IsRequired();

            builder.Property(x => x.DueDate)
                .IsRequired();

            builder.Property(x => x.PaymentDate)
                .IsRequired(false);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasColumnType("VARCHAR(20)")
                .HasMaxLength(20)
                .HasConversion(new EnumToStringConverter<TransactionStatus>());

            builder.Property(x => x.Value)
                .IsRequired()
                .HasColumnType("NUMERIC(18,2)")
                .HasPrecision(18, 2);

            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100);

            builder.HasOne(x => x.Category)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BankAccount)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}