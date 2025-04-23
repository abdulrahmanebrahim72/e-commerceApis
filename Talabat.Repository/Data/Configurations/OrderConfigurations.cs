using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Repository.Data.Configurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<OrderAg>
    {
        public void Configure(EntityTypeBuilder<OrderAg> builder)
        {
            builder.OwnsOne(o => o.ShippingAddress, shAd => shAd.WithOwner());

            builder.Property(o => o.Status)
                   .HasConversion(Ostatus => Ostatus.ToString(),
                                  Ostatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), Ostatus));

            //builder.HasOne(o => o.DeliveryMethod)
            //       .WithMany()
            //       .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(o => o.Items)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.SubTotal)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
