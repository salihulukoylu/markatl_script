using MarkaTLScript.Models;
using Microsoft.AspNetCore.Mvc;

namespace MarkaTLScript.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly DbMarkatlScriptContext _db;
        private readonly ILogService _logService;

        public ExpenseController(ILogService logService, DbMarkatlScriptContext db)
        {
            _logService = logService;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
