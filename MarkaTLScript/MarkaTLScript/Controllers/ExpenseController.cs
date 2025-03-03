using System.Linq;
using System.Threading.Tasks;
using System;
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
        public async Task<IActionResult> GiderKayitlari()
        {
            try
            {

                return View("~/Views/Muhasebe/GiderKayitlari.cshtml");
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(IncomeController), nameof(GiderKayitlari));
                TempData["ErrorMessage"] = "Bir hata oluştu, lütfen daha sonra tekrar deneyiniz.";
                // Hata durumunda boş liste veya uygun hata sayfası dönebilirsiniz.
                return View("~/Views/Muhasebe/GiderKayitlari.cshtml", Enumerable.Empty<Operator>());
            }
        }

        // POST: /Expense/AjaxAddExpenseCategory
        [HttpPost]
        public async Task<IActionResult> AjaxAddExpenseCategory([FromBody] ExpenseCategory model)
        {
            if (model == null || string.IsNullOrEmpty(model.Title))
            {
                return Json(new { success = false, message = "Gerekli bilgiler eksik." });
            }

            try
            {
                var newCategory = new ExpenseCategory
                {
                    Title = model.Title,
                    IsActive = model.IsActive
                };

                _db.ExpenseCategories.Add(newCategory);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Gider kalemi başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ExpenseController), nameof(AjaxAddExpenseCategory));
                return Json(new { success = false, message = "Gider kalemi eklenirken hata oluştu." });
            }
        }


    }
}
