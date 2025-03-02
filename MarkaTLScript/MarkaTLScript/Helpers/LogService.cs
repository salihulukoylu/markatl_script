// ILogService.cs
using MarkaTLScript.Models;
using System;

public interface ILogService
{
    void LogError(Exception ex, string controllerName, string actionName);
}

// LogService.cs
public class LogService : ILogService
{
    private readonly DbMarkatlScriptContext _db;
    public LogService(DbMarkatlScriptContext db)
    {
        _db = db;
    }

    public void LogError(Exception ex, string controllerName, string actionName)
    {
        var log = new SystemLog
        {
            Timestamp = DateTime.Now,
            ExceptionMessage = ex.Message,
            ExceptionStackTrace = ex.StackTrace,
            ControllerName = controllerName,
            ActionName = actionName
        };

        _db.SystemLogs.Add(log);
        _db.SaveChanges();
    }
}
