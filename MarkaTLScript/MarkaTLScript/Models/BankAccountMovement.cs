using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class BankAccountMovement
{
    public int Id { get; set; }

    public int BankId { get; set; }

    public string TransactionType { get; set; }

    public int? IncomeCategoryId { get; set; }

    public int? ExpenseCategoryId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual BankAccount Bank { get; set; }

    public virtual ExpenseCategory ExpenseCategory { get; set; }

    public virtual IncomeCategory IncomeCategory { get; set; }
}
