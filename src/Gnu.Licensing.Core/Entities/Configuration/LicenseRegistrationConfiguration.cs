﻿using Gnu.Licensing.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace Gnu.Licensing.Core.Entities.Configuration
{
    public class LicenseRegistrationConfiguration : IEntityTypeConfiguration<LicenseRegistration>
    {
        public void Configure(EntityTypeBuilder<LicenseRegistration> builder)
        {
            builder.ToTable(nameof(LicenseRegistration))
                  .HasIndex(x => new { x.LicenseRegistrationId, x.LicenseName, x.LicenseEmail, x.IsActive }).IsUnique();

            builder.HasKey(x => x.LicenseRegistrationId);

            builder.Property(t => t.LicenseRegistrationId)
                   .IsRequired();

            builder.Property(t => t.ProductId)
                  .IsRequired();

            builder.Property(t => t.CompanyId)
                 .IsRequired();

            builder.Property(t => t.LicenseName)
                   .IsRequired();

            builder.Property(t => t.LicenseEmail)
                   .IsRequired();

            builder.Property(t => t.LicenseType)
                .HasConversion(EnumToInt.Instance)
                  .IsRequired();

            builder.Property(t => t.IsActive)
                .IsRequired();

            builder.Property(t => t.Quantity)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(t => t.ExpireUtc);

            builder.Property(x => x.CreatedDateTimeUtc)
                   .IsRequired()
                   .HasDefaultValue(DateTime.UtcNow);

            builder.Property(x => x.CreatedByUser)
                   .IsRequired();
        }
    }
}
