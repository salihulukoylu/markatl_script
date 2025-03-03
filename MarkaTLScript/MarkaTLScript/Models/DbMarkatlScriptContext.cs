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

    public virtual DbSet<ApiAccountMovement> ApiAccountMovements { get; set; }

    public virtual DbSet<ApiBalanceTransaction> ApiBalanceTransactions { get; set; }

    public virtual DbSet<ApiDefinition> ApiDefinitions { get; set; }

    public virtual DbSet<ApiTypeList> ApiTypeLists { get; set; }

    public virtual DbSet<BankAccount> BankAccounts { get; set; }

    public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; }

    public virtual DbSet<ExpenseCategory> ExpenseCategories { get; set; }

    public virtual DbSet<IncomeCategory> IncomeCategories { get; set; }

    public virtual DbSet<Operator> Operators { get; set; }

    public virtual DbSet<OperatorFirm> OperatorFirms { get; set; }

    public virtual DbSet<OperatorType> OperatorTypes { get; set; }

    public virtual DbSet<SystemLog> SystemLogs { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=185.210.92.195;port=3306;database=db_markatl_script;user=user_markatl;password=Ae*yh8834", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.5.27-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<ApiAccountMovement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("api_account_movements");

            entity.HasIndex(e => e.ApiDefinitionId, "fk_api_movement_api");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.ApiDefinitionId)
                .HasColumnType("int(11)")
                .HasColumnName("api_definition_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.MovementType)
                .IsRequired()
                .HasMaxLength(250)
                .HasColumnName("movement_type");
            entity.Property(e => e.NewBalance)
                .HasPrecision(10, 2)
                .HasColumnName("new_balance");
            entity.Property(e => e.PreviousBalance)
                .HasPrecision(10, 2)
                .HasColumnName("previous_balance");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");

            entity.HasOne(d => d.ApiDefinition).WithMany(p => p.ApiAccountMovements)
                .HasForeignKey(d => d.ApiDefinitionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_api_movement_api");
        });

        modelBuilder.Entity<ApiBalanceTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("api_balance_transactions");

            entity.HasIndex(e => e.ApiDefinitionId, "fk_api_balance_api");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.ApiDefinitionId)
                .HasColumnType("int(11)")
                .HasColumnName("api_definition_id");
            entity.Property(e => e.BankId)
                .HasColumnType("int(11)")
                .HasColumnName("bank_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");
            entity.Property(e => e.TransactionType)
                .IsRequired()
                .HasColumnType("enum('Borç Bakiye','Borç Ödeme','Nakit Bakiye')")
                .HasColumnName("transaction_type");

            entity.HasOne(d => d.ApiDefinition).WithMany(p => p.ApiBalanceTransactions)
                .HasForeignKey(d => d.ApiDefinitionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_api_balance_api");
        });

        modelBuilder.Entity<ApiDefinition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("api_definitions");

            entity.HasIndex(e => e.ApiTypeId, "fk_api_type");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.ApiDescription)
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("api_description");
            entity.Property(e => e.ApiStatus).HasColumnName("api_status");
            entity.Property(e => e.ApiTypeId)
                .HasColumnType("int(11)")
                .HasColumnName("api_type_id");
            entity.Property(e => e.FaturaStatus).HasColumnName("fatura_status");
            entity.Property(e => e.KontorStatus).HasColumnName("kontor_status");
            entity.Property(e => e.OyunStatus).HasColumnName("oyun_status");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.SiteAddress)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("site_address");
            entity.Property(e => e.UserCode)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("user_code");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("username");
            entity.Property(e => e.WorkingHours)
                .HasMaxLength(50)
                .HasColumnName("working_hours");

            entity.HasOne(d => d.ApiType).WithMany(p => p.ApiDefinitions)
                .HasForeignKey(d => d.ApiTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_api_type");
        });

        modelBuilder.Entity<ApiTypeList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("api_type_list");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.TypeName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bank_accounts");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AccountHolder)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("account_holder");
            entity.Property(e => e.AccountNo)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("account_no");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.BankLogo)
                .HasMaxLength(255)
                .HasColumnName("bank_logo");
            entity.Property(e => e.BankName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("bank_name");
            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .HasColumnName("branch_code");
            entity.Property(e => e.Iban)
                .IsRequired()
                .HasMaxLength(34)
                .HasColumnName("iban");
            entity.Property(e => e.MinDepositAmount)
                .HasPrecision(10, 2)
                .HasColumnName("min_deposit_amount");
            entity.Property(e => e.VisibleToReseller)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("visible_to_reseller");
        });

        modelBuilder.Entity<EfmigrationsHistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity
                .ToTable("__EFMigrationsHistory")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion)
                .IsRequired()
                .HasMaxLength(32);
        });

        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("expense_categories");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<IncomeCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("income_categories");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("title");
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
                .IsRequired()
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
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.MaxSubscriberNoLength)
                .HasColumnType("int(11)")
                .HasColumnName("max_subscriber_no_length");
            entity.Property(e => e.MinSubscriberNoLength)
                .HasColumnType("int(11)")
                .HasColumnName("min_subscriber_no_length");
            entity.Property(e => e.SystemName)
                .IsRequired()
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
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("firm_name");
            entity.Property(e => e.ImgPath)
                .HasMaxLength(50)
                .HasColumnName("img_path");
        });

        modelBuilder.Entity<OperatorType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("operator_types");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.TypeName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("system_logs");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.ActionName)
                .HasMaxLength(100)
                .HasColumnName("action_name");
            entity.Property(e => e.ControllerName)
                .HasMaxLength(100)
                .HasColumnName("controller_name");
            entity.Property(e => e.ExceptionMessage)
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("exception_message");
            entity.Property(e => e.ExceptionStackTrace)
                .HasColumnType("text")
                .HasColumnName("exception_stack_trace");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
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
            entity.Property(e => e.FaturaStatus).HasColumnName("fatura_status");
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
