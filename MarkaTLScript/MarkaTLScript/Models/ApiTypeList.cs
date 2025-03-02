using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class ApiTypeList
{
    public int Id { get; set; }

    public string TypeName { get; set; }

    public virtual ICollection<ApiDefinition> ApiDefinitions { get; set; } = new List<ApiDefinition>();
}
