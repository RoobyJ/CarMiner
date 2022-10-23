using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CarMiner.Entities;

namespace CarMiner.Context
{
    public partial class carsContext : DbContext
    {
        public carsContext()
        {
        }

        public carsContext(DbContextOptions<carsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Add> Adds { get; set; } = null!;
        public virtual DbSet<Brand> Brands { get; set; } = null!;
        public virtual DbSet<Model> Models { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;database=caroffers;uid=root;password=pineapple;persist security info=False;connect timeout=300", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.31-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb3_polish_ci")
                .HasCharSet("utf8mb3");

            modelBuilder.Entity<Add>(entity =>
            {
                entity.HasKey(e => e.Idadd)
                    .HasName("PRIMARY");

                entity.ToTable("adds");

                entity.HasIndex(e => e.Idbrand, "FKbrandadd_idx");

                entity.HasIndex(e => e.Idmodel, "FKmodeladd_idx");

                entity.HasIndex(e => e.Idadd, "idadd_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Idotomoto, "idotomoto_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idadd).HasColumnName("idadd");

                entity.Property(e => e.Fuel)
                    .HasMaxLength(45)
                    .HasColumnName("fuel");

                entity.Property(e => e.Idbrand).HasColumnName("idbrand");

                entity.Property(e => e.Idmodel).HasColumnName("idmodel");

                entity.Property(e => e.Idotomoto).HasColumnName("idotomoto");

                entity.Property(e => e.Mileage)
                    .HasMaxLength(45)
                    .HasColumnName("mileage");

                entity.Property(e => e.Power)
                    .HasMaxLength(45)
                    .HasColumnName("power");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Prodyear).HasColumnName("prodyear");

                entity.HasOne(d => d.IdbrandNavigation)
                    .WithMany(p => p.Adds)
                    .HasForeignKey(d => d.Idbrand)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbrandadd");

                entity.HasOne(d => d.IdmodelNavigation)
                    .WithMany(p => p.Adds)
                    .HasForeignKey(d => d.Idmodel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKmodeladd");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.Idbrand)
                    .HasName("PRIMARY");

                entity.ToTable("brands");

                entity.HasIndex(e => e.Brandname, "brandname_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Idbrand, "idbrands_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idbrand).HasColumnName("idbrand");

                entity.Property(e => e.Brandname)
                    .HasMaxLength(45)
                    .HasColumnName("brandname");
            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.HasKey(e => e.Idmodel)
                    .HasName("PRIMARY");

                entity.ToTable("models");

                entity.HasIndex(e => e.Idbrand, "FKmodelbrand_idx");

                entity.HasIndex(e => e.Idmodel, "idmodels_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Modelname, "modelname_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idmodel).HasColumnName("idmodel");

                entity.Property(e => e.Idbrand).HasColumnName("idbrand");

                entity.Property(e => e.Modelname)
                    .HasMaxLength(45)
                    .HasColumnName("modelname");

                entity.HasOne(d => d.IdbrandNavigation)
                    .WithMany(p => p.Models)
                    .HasForeignKey(d => d.Idbrand)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKbrandmodel");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
