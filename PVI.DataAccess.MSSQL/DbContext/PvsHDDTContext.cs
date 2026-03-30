using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace PVI.DAO.Entities.Models;

    public partial class PvsHDDTContext : DbContext
    {
        public PvsHDDTContext()
        {
        }

        public PvsHDDTContext(DbContextOptions<PvsHDDTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<HddtHsm> HddtHsms { get; set; } = null!;
       
    public string connectHddt = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["PvsTcdContext"]!;

   
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer(connectHddt);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HddtHsm>(entity =>
            {
                entity.ToTable("hddt_hsm");

                entity.HasKey(e => e.PrKey);

                entity.Property(e => e.PrKey).HasColumnName("pr_key");

                entity.Property(e => e.MaDonvi)
                    .HasMaxLength(8)
                    .HasColumnName("ma_donvi");

                entity.Property(e => e.PartitionAlias)
                    .HasMaxLength(250)
                    .HasColumnName("partition_alias");

                entity.Property(e => e.PartitionSerial)
                    .HasMaxLength(20)
                    .HasColumnName("partition_serial");

                entity.Property(e => e.PrivateKeyAlias)
                    .HasMaxLength(250)
                    .HasColumnName("private_key_alias");

                entity.Property(e => e.Password)
                   .HasMaxLength(500)
                   .HasColumnName("password");

                entity.Property(e => e.NgayHluc)
                   .HasColumnType("smalldatetime")
                   .HasColumnName("private_key_alias");

                entity.Property(e => e.TvanUsername)
                 .HasMaxLength(50)
                 .HasColumnName("tvan_username");

                entity.Property(e => e.TvanPassword)
                 .HasMaxLength(100)
                 .HasColumnName("tvan_password");

                entity.Property(e => e.TaxCode)
                 .HasMaxLength(50)
                 .HasColumnName("tax_code");

                entity.Property(e => e.Mst)
                 .HasMaxLength(500)
                 .HasColumnName("mst");

                entity.Property(e => e.SerialNumber)
                 .HasMaxLength(50)
                 .HasColumnName("serial_number");

            });
        OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

