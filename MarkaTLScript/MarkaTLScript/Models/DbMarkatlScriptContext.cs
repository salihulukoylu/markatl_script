using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace MarkaTLScript.Models;

public partial class DbMarkatlScriptContext : DbContext
{
    public DbMarkatlScriptContext()
    {
    }

    public DbMarkatlScriptContext(DbContextOptions<DbMarkatlScriptContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; }

    public virtual DbSet<Operator> Operators { get; set; }

    public virtual DbSet<OperatorFirm> OperatorFirms { get; set; }

    public virtual DbSet<OperatorType> OperatorTypes { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=185.210.92.195;port=3306;database=db_markatl_script;user=user_markatl;password=Ae*yh8834", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.5.27-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<EfmigrationsHistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity
                .ToTable("__EFMigrationsHistory")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Operator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("operators");

            entity.HasIndex(e => e.FirmId, "firm_id");

            entity.HasIndex(e => e.TypeId, "type_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.BackgroundColor)
                .HasMaxLength(7)
                .HasColumnName("background_color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .HasColumnName("display_name");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("display_order");
            entity.Property(e => e.FirmId)
                .HasColumnType("int(11)")
                .HasColumnName("firm_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.MaxSubscriberNoLength)
                .HasColumnType("int(11)")
                .HasColumnName("max_subscriber_no_length");
            entity.Property(e => e.MinSubscriberNoLength)
                .HasColumnType("int(11)")
                .HasColumnName("min_subscriber_no_length");
            entity.Property(e => e.SystemName)
                .HasMaxLength(255)
                .HasColumnName("system_name");
            entity.Property(e => e.TextColor)
                .HasMaxLength(7)
                .HasColumnName("text_color");
            entity.Property(e => e.TypeId)
                .HasColumnType("int(11)")
                .HasColumnName("type_id");

            entity.HasOne(d => d.Firm).WithMany(p => p.Operators)
                .HasForeignKey(d => d.FirmId)
                .HasConstraintName("operators_ibfk_1");

            entity.HasOne(d => d.Type).WithMany(p => p.Operators)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("operators_ibfk_2");
        });

        modelBuilder.Entity<OperatorFirm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("operator_firms");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.FirmName)
                .HasMaxLength(255)
                .HasColumnName("firm_name");
        });

        modelBuilder.Entity<OperatorType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("operator_types");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("system_settings");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.GameStatus).HasColumnName("game_status");
            entity.Property(e => e.KontorStatus).HasColumnName("kontor_status");
            entity.Property(e => e.SafeIps)
                .HasMaxLength(2000)
                .HasColumnName("safe_ips");
            entity.Property(e => e.SiteClosedMessage)
                .HasColumnType("text")
                .HasColumnName("site_closed_message");
            entity.Property(e => e.SiteName)
                .HasMaxLength(255)
                .HasColumnName("site_name");
            entity.Property(e => e.SmsStatus).HasColumnName("sms_status");
            entity.Property(e => e.SupportPhone)
                .HasMaxLength(50)
                .HasColumnName("support_phone");
            entity.Property(e => e.SystemStatus).HasColumnName("system_status");
            entity.Property(e => e.TelegramNotificationEnabled).HasColumnName("telegram_notification_enabled");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
