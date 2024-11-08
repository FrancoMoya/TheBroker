using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TheBroker.DTOs;

namespace TheBroker.Models;

public partial class DbOrdersContext : DbContext
{
    public DbOrdersContext()
    {
    }

    public DbOrdersContext(DbContextOptions<DbOrdersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersHistory> OrdersHistories { get; set; }

    public virtual DbSet<StockMarketShare> StockMarketShares { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.TxNumber).HasName("PK__ORDERS__61EC9A88A72AEE77");

            entity.ToTable("ORDERS");

            entity.Property(e => e.TxNumber).HasColumnName("TX_NUMBER");
            entity.Property(e => e.Action)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("ACTION");
            entity.Property(e => e.IdSymbol).HasColumnName("ID_SYMBOL");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ORDER_DATE");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.IdSymbolNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdSymbol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ORDERS__ID_SYMBO__3E52440B");
        });

        modelBuilder.Entity<OrdersHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ORDERS_H__3214EC277CDDDC46");

            entity.ToTable("ORDERS_HISTORY");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Action)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("ACTION");
            entity.Property(e => e.ChangedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CHANGED_DATE");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.IdSymbol).HasColumnName("ID_SYMBOL");
            entity.Property(e => e.StatusAfter)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STATUS_AFTER");
            entity.Property(e => e.StatusBefore)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STATUS_BEFORE");
            entity.Property(e => e.TxNumber).HasColumnName("TX_NUMBER");
        });

        modelBuilder.Entity<StockMarketShare>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__STOCK_MA__3214EC27D3D81D8B");

            entity.ToTable("STOCK_MARKET_SHARES");

            entity.HasIndex(e => e.Symbol, "UQ__STOCK_MA__D3A69A008F6AF893").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Symbol)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("SYMBOL");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("UNIT_PRICE");
        });

        OnModelCreatingPartial(modelBuilder);

        // Para que EF Core lo reconozca como una entidad sin clave
        modelBuilder.Entity<ListOrdersDTO>().HasNoKey();
        modelBuilder.Entity<ListOrdersExecutedDTO>().HasNoKey();
        // Esto indica a EF Core que ListOrdersDTO es un tipo de datos para consultas específicas y no una tabla con clave primaria.
        modelBuilder.Entity<OrderDetailsDTO>().HasNoKey();

        modelBuilder.Entity<ListStockDTO>().HasNoKey()
            .Property(p => p.UNIT_PRICE)
            .HasPrecision(10, 2);
        modelBuilder.Entity<StockDetailsDTO>().HasNoKey()
            .Property(p => p.UNIT_PRICE)
        .HasPrecision(10, 2);

        modelBuilder.Entity<ListOhDTO>().HasNoKey();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}
