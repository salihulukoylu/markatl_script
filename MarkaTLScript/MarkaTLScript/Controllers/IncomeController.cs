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
               
                return View("~/Views/Muhasebe/GelirKayitlari.cshtml");
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(IncomeController), nameof(GelirKayitlari));
                TempData["ErrorMessage"] = "Bir hata oluştu, lütfen daha sonra tekrar deneyiniz.";
                // Hata durumunda boş liste veya uygun hata sayfası dönebilirsiniz.
                return View("~/Views/Muhasebe/GelirKayitlari.cshtml", Enumerable.Empty<Operator>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AjaxAddIncomeCategory([FromBody] IncomeCategory model)
        {
            if (model == null || string.IsNullOrEmpty(model.Title))
            {
                return Json(new { success = false, message = "Gerekli bilgiler eksik." });
            }

            try
            {
                var newCategory = new IncomeCategory
                {
                    Title = model.Title,
                    IsActive = model.IsActive
                };

                _db.IncomeCategories.Add(newCategory);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Gelir kalemi başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(IncomeController), nameof(AjaxAddIncomeCategory));
                return Json(new { success = false, message = "Gelir kalemi eklenirken hata oluştu." });
            }
        }
    }
}
