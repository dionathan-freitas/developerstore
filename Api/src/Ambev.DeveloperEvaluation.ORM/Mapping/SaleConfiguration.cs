using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.SaleNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(s => s.SaleDate)
                   .IsRequired();

            builder.Property(s => s.TotalAmount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(s => s.IsCancelled)
                   .HasDefaultValue(false);

            builder.HasMany(s => s.Items)
                   .WithOne()
                   .HasForeignKey(i => i.SaleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
