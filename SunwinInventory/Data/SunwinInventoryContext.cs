using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Models;

namespace SunwinInventory.Data;

public partial class SunwinInventoryContext : DbContext
{
    public SunwinInventoryContext()
    {
    }

    public SunwinInventoryContext(DbContextOptions<SunwinInventoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Designation> Designations { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductName> ProductNames { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-B2C6QFK;Database=SunwinInventoryDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.DesignationId).HasName("PK__Designat__BABD60DE9A113DDD");

            entity.HasIndex(e => e.DesignationName, "IX_Designations_DesignationName").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DesignationName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ProductC__19093A0B465E5D0D");

            entity.HasIndex(e => e.CategoryName, "IX_ProductCategories_CategoryName").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductName>(entity =>
        {
            entity.HasKey(e => e.ProductNameId).HasName("PK__ProductN__343A705C9C6DB33E");

            entity.HasIndex(e => e.ProductCode, "IX_ProductNames_ProductCode").IsUnique();

            entity.HasIndex(e => e.Name, "IX_ProductNames_ProductName").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AE8DE9228");

            entity.HasIndex(e => e.RoleName, "IX_Roles_RoleName").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B447F1EE01");

            entity.HasIndex(e => e.SupplierCode, "IX_Suppliers_SupplierCode").IsUnique();

            entity.HasIndex(e => e.SupplierName, "IX_Suppliers_SupplierName").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.ContactPerson).HasMaxLength(150);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.SupplierCode).HasMaxLength(50);
            entity.Property(e => e.SupplierName).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C5E0C50CA");

            entity.HasIndex(e => e.UserName, "IX_Users_UserName").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.MustChangePassword).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__2608AFF9EEDCFAF6");

            entity.HasIndex(e => e.WarehouseName, "UQ_Warehouses_WarehouseName").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.ContactPerson).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.WarehouseName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
