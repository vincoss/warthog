﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Gnu.Licensing.Core.Entities.Configuration
{
    public class LicenseActivationConfiguration : IEntityTypeConfiguration<LicenseActivation>
    {
        public void Configure(EntityTypeBuilder<LicenseActivation> builder)
        {
            builder.ToTable(nameof(LicenseActivation))
                   .HasIndex(x => new { x.LicenseActivationId }).IsUnique();

            builder.HasKey(x => x.LicenseActivationId);

            builder.Property(t => t.LicenseActivationId)
                .IsRequired();

            builder.Property(t => t.LicenseId)
                   .IsRequired();

            builder.Property(t => t.ProductId)
                .IsRequired();

            builder.Property(t => t.CompanyId)
               .IsRequired();

            builder.Property(t => t.LicenseString)
                   .IsRequired();

            builder.Property(t => t.LicenseAttributes);

            builder.Property(t => t.LicenseChecksum)
                .IsRequired();

            builder.Property(t => t.AttributesChecksum);

            builder.Property(t => t.ChecksumType)
              .IsRequired();

            builder.Property(x => x.CreatedDateTimeUtc)
                   .IsRequired();

            builder.Property(x => x.CreatedByUser)
                   .IsRequired();
        }
    }
}
