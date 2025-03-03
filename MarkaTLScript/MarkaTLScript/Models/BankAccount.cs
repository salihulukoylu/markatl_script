using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class BankAccount
{
    public int Id { get; set; }

    public string BankName { get; set; }

    public string AccountHolder { get; set; }

    public string BankLogo { get; set; }

    public string BranchCode { get; set; }

    public string AccountNo { get; set; }

    public string Iban { get; set; }

    public decimal MinDepositAmount { get; set; }

    public bool VisibleToReseller { get; set; }

    public bool Active { get; set; }

    public virtual ICollection<BankAccountMovement> BankAccountMovements { get; set; } = new List<BankAccountMovement>();
}
