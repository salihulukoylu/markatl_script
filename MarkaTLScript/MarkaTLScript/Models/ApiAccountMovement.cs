using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class ApiAccountMovement
{
    public int Id { get; set; }

    public int ApiDefinitionId { get; set; }

    public string MovementType { get; set; }

    public decimal Amount { get; set; }

    public decimal PreviousBalance { get; set; }

    public decimal NewBalance { get; set; }

    public DateTime TransactionDate { get; set; }

    public string Description { get; set; }

    public virtual ApiDefinition ApiDefinition { get; set; }
}
