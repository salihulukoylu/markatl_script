using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class ApiDefinition
{
    public int Id { get; set; }

    public int ApiTypeId { get; set; }

    public string ApiDescription { get; set; }

    public string SiteAddress { get; set; }

    public string UserCode { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public bool KontorStatus { get; set; }

    public bool FaturaStatus { get; set; }

    public bool OyunStatus { get; set; }

    public string WorkingHours { get; set; }

    public bool ApiStatus { get; set; }

    public virtual ICollection<ApiAccountMovement> ApiAccountMovements { get; set; } = new List<ApiAccountMovement>();

    public virtual ICollection<ApiBalanceTransaction> ApiBalanceTransactions { get; set; } = new List<ApiBalanceTransaction>();

    public virtual ApiTypeList ApiType { get; set; }
}
