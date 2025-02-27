using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class OperatorType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Operator> Operators { get; set; } = new List<Operator>();
}
