using FrontOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOffice.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title).HasMaxLength(200).IsRequired();
            builder.Property(n => n.Message).HasMaxLength(1000).IsRequired();
            builder.Property(n => n.Type).HasMaxLength(50);

            builder.Property(n => n.TargetUrl).HasMaxLength(500);
            builder.Property(n => n.EntityId).HasMaxLength(100);

            builder.Property(n => n.ProfilCode).HasMaxLength(100);

            builder.Property(n => n.IsRead).HasColumnType("bit");

            // Mapping du flag Broadcast
            builder.Property(n => n.IsBroadcast).HasColumnType("bit");

            builder.Property(n => n.DateEnvoi).HasColumnType("bigint");

            // Index pour accélérer la recherche d'historique
            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => n.ProfilCode);
            builder.HasIndex(n => n.IsBroadcast);
            builder.HasIndex(n => n.DateEnvoi);
        }
    }
}
