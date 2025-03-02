using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class SystemLog
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string ExceptionMessage { get; set; }

    public string ExceptionStackTrace { get; set; }

    public string ControllerName { get; set; }

    public string ActionName { get; set; }
}
