using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class ExpenseCategory
{
    public int Id { get; set; }

    public string Title { get; set; }

    public bool? IsActive { get; set; }
}
