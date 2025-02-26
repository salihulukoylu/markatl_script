using System;
using System.Collections.Generic;

namespace MarkaTLScript.Models;

public partial class SystemSetting
{
    public int Id { get; set; }

    public string? SiteName { get; set; }

    public string? CompanyName { get; set; }

    public string? SupportPhone { get; set; }

    public bool? SystemStatus { get; set; }

    public string? SiteClosedMessage { get; set; }

    public bool? TelegramNotificationEnabled { get; set; }

    public bool? KontorStatus { get; set; }

    public bool? GameStatus { get; set; }

    public bool? SmsStatus { get; set; }

    public string? SafeIps { get; set; }
}
