using System.Linq;
using System.Threading.Tasks;
using System;
using MarkaTLScript.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarkaTLScript.Controllers
{
    public class IncomeController : Controller
    {
        private readonly DbMarkatlScriptContext _db;
        private readonly ILogService _logService;

        public IncomeController(ILogService logService, DbMarkatlScriptContext db)
        {
            _logService = logService;
            _db = db;
        }
        public async Task<IActionResult> GelirKayitlari()
        {
            try
            {
               
                return View("~/Views/Kontor/GelirKayitlari.cshtml");
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(IncomeController), nameof(GelirKayitlari));
                TempData["ErrorMessage"] = "Bir hata oluştu, lütfen daha sonra tekrar deneyiniz.";
                // Hata durumunda boş liste veya uygun hata sayfası dönebilirsiniz.
                return View("~/Views/Kontor/GelirKayitlari.cshtml", Enumerable.Empty<Operator>());
            }
        }

    }
}
