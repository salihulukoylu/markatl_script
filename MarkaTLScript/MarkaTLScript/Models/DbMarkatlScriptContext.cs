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

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=185.210.92.195;port=3306;database=db_markatl_script;user=user_markatl;password=Ae*yh8834", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.5.27-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("system_settings");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.GameStatus).HasColumnName("game_status");
            entity.Property(e => e.KontorStatus).HasColumnName("kontor_status");
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
