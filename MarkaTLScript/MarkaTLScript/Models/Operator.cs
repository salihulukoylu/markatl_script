using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class Operator
{
    public int Id { get; set; }

    public int FirmId { get; set; }

    public int TypeId { get; set; }

    public string SystemName { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public int MinSubscriberNoLength { get; set; }

    public int MaxSubscriberNoLength { get; set; }

    public int? DisplayOrder { get; set; }

    public string? BackgroundColor { get; set; }

    public string? TextColor { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual OperatorFirm Firm { get; set; } = null!;

    public virtual OperatorType Type { get; set; } = null!;
}
