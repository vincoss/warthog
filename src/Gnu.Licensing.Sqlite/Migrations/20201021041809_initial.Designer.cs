﻿// <auto-generated />
using System;
using Gnu.Licensing.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Gnu.Licensing.Sqlite.Migrations
{
    [DbContext(typeof(SqliteContext))]
    [Migration("20201021041809_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("Gnu.Licensing.Core.Entities.LicenseActivation", b =>
                {
                    b.Property<Guid>("LicenseActivationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("AttributesChecksum")
                        .HasColumnType("NVARCHAR");

                    b.Property<string>("ChecksumType")
                        .IsRequired()
                        .HasColumnType("VARCHAR(12) COLLATE NOCASE");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedByUser")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(64) COLLATE NOCASE");

                    b.Property<DateTime>("CreatedDateTimeUtc")
                        .HasColumnType("DATETIME");

                    b.Property<string>("LicenseAttributes")
                        .HasColumnType("NVARCHAR COLLATE NOCASE");

                    b.Property<string>("LicenseChecksum")
                        .IsRequired()
                        .HasColumnType("NVARCHAR");

                    b.Property<Guid>("LicenseId")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("LicenseString")
                        .IsRequired()
                        .HasColumnType("NVARCHAR COLLATE NOCASE");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("VARCHAR(36)");

                    b.HasKey("LicenseActivationId");

                    b.HasIndex("LicenseActivationId")
                        .IsUnique();

                    b.ToTable("LicenseActivation");
                });

            modelBuilder.Entity("Gnu.Licensing.Core.Entities.LicenseCompany", b =>
                {
                    b.Property<Guid>("LicenseCompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(256) COLLATE NOCASE");

                    b.Property<string>("CreatedByUser")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(64) COLLATE NOCASE");

                    b.Property<DateTime>("CreatedDateTimeUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DATETIME")
                        .HasDefaultValue(new DateTime(2020, 10, 21, 4, 18, 9, 312, DateTimeKind.Utc).AddTicks(2518));

                    b.HasKey("LicenseCompanyId");

                    b.HasIndex("CompanyName")
                        .IsUnique();

                    b.HasIndex("LicenseCompanyId")
                        .IsUnique();

                    b.ToTable("LicenseCompany");
                });

            modelBuilder.Entity("Gnu.Licensing.Core.Entities.LicenseProduct", b =>
                {
                    b.Property<Guid>("LicenseProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("VARCHAR(36)");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedByUser")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(64) COLLATE NOCASE");

                    b.Property<DateTime>("CreatedDateTimeUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DATETIME")
                        .HasDefaultValue(new DateTime(2020, 10, 21, 4, 18, 9, 317, DateTimeKind.Utc).AddTicks(2329));

                    b.Property<string>("ProductDescription")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(1024) COLLATE NOCASE");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(256) COLLATE NOCASE");

                    b.Property<string>("SignKeyName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(64) COLLATE NOCASE");

                    b.HasKey("LicenseProductId");

                    b.HasIndex("LicenseProductId")
                        .IsUnique();

                    b.HasIndex("ProductName")
                        .IsUnique();

                    b.ToTable("LicenseProduct");
                });

            modelBuilder.Entity("Gnu.Licensing.Core.Entities.LicenseRegistration", b =>
                {
                    b.Property<Guid>("LicenseRegistrationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("Comment")
                        .HasColumnType("NVARCHAR(1024) COLLATE NOCASE");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedByUser")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(64) COLLATE NOCASE");

                    b.Property<DateTime>("CreatedDateTimeUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DATETIME")
                        .HasDefaultValue(new DateTime(2020, 10, 21, 4, 18, 9, 322, DateTimeKind.Utc).AddTicks(3520));

                    b.Property<DateTime?>("ExpireUtc")
                        .HasColumnType("DATETIME");

                    b.Property<bool>("IsActive")
                        .HasColumnType("BOOLEAN");

                    b.Property<string>("LicenseEmail")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(256) COLLATE NOCASE");

                    b.Property<string>("LicenseName")
                        .IsRequired()
                        .HasColumnType("NVARCHAR(256) COLLATE NOCASE");

                    b.Property<int>("LicenseType")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<int>("Quantity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(1);

                    b.HasKey("LicenseRegistrationId");

                    b.HasIndex("LicenseRegistrationId", "LicenseName", "LicenseEmail", "IsActive")
                        .IsUnique();

                    b.ToTable("LicenseRegistration");
                });
#pragma warning restore 612, 618
        }
    }
}