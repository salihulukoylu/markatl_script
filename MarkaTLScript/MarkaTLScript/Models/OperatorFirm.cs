using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class OperatorFirm
{
    public int Id { get; set; }

    public string FirmName { get; set; } = null!;

    public virtual ICollection<Operator> Operators { get; set; } = new List<Operator>();
}
