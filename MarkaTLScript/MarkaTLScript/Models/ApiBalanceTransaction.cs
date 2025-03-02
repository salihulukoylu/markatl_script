using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class ApiBalanceTransaction
{
    public int Id { get; set; }

    public int ApiDefinitionId { get; set; }

    public string TransactionType { get; set; }

    public int? BankId { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public virtual ApiDefinition ApiDefinition { get; set; }
}
