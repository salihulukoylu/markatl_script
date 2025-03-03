using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class IncomeCategory
{
    public int Id { get; set; }

    public string Title { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BankAccountMovement> BankAccountMovements { get; set; } = new List<BankAccountMovement>();
}
