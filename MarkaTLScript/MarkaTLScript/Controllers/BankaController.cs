using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarkaTLScript.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarkaTLScript.Controllers
{
    public class BankaController : Controller
    {
        private readonly DbMarkatlScriptContext _db;
        private readonly ILogService _logService;

        public BankaController(ILogService logService, DbMarkatlScriptContext db)
        {
            _logService = logService;
            _db = db;
        }

        // GET: /Bank/BankaAyarlari
        public async Task<IActionResult> BankaAyarlari()
        {
            try
            {
                var banks = await _db.BankAccounts.ToListAsync();
                return View("~/Views/Odeme/BankaAyarlari.cshtml", banks);
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(BankaAyarlari));
                TempData["ErrorMessage"] = "Bir hata oluştu, lütfen daha sonra tekrar deneyiniz.";
                return View("~/Views/Odeme/BankaAyarlari.cshtml", Enumerable.Empty<BankAccount>());
            }
        }

        // POST: /Bank/AjaxCreateBank
        [HttpPost]
        public async Task<IActionResult> AjaxCreateBank([FromBody] BankAccount bank)
        {
            if (bank == null)
            {
                return Json(new { success = false, message = "Banka verisi boş." });
            }

            try
            {
                // If BankLogo is not needed, ignore its value.
                _db.BankAccounts.Add(bank);
                await _db.SaveChangesAsync();
               
                return Json(new { success = true, message = "Banka başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(AjaxCreateBank));
                return Json(new { success = false, message = "Banka eklenirken bir hata oluştu." });
            }
        }

        // GET: /Bank/AjaxGetBank/{id}
        [HttpGet]
        public async Task<IActionResult> AjaxGetBank(int id)
        {
            try
            {
                var bank = await _db.BankAccounts.FirstOrDefaultAsync(b => b.Id == id);
                if (bank == null)
                {
                    return Json(new { success = false, message = "Banka bulunamadı." });
                }
                return Json(new { success = true, bankData = bank });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(AjaxGetBank));
                return Json(new { success = false, message = "Banka bilgileri alınamadı." });
            }
        }

        // POST: /Bank/UpdateBank
        [HttpPost]
        public async Task<IActionResult> UpdateBank([FromBody] BankAccount updatedBank)
        {
            if (updatedBank == null || updatedBank.Id <= 0)
            {
                return Json(new { success = false, message = "Geçersiz banka verisi." });
            }

            try
            {
                var bank = await _db.BankAccounts.FirstOrDefaultAsync(b => b.Id == updatedBank.Id);
                if (bank == null)
                {
                    return Json(new { success = false, message = "Banka bulunamadı." });
                }

                // Update the properties. Note: BankLogo is commented out.
                bank.BankName = updatedBank.BankName;
                bank.AccountHolder = updatedBank.AccountHolder;
                // bank.BankLogo = updatedBank.BankLogo;
                bank.BranchCode = updatedBank.BranchCode;
                bank.AccountNo = updatedBank.AccountNo;
                bank.Iban = updatedBank.Iban;
                bank.MinDepositAmount = updatedBank.MinDepositAmount;
                bank.VisibleToReseller = updatedBank.VisibleToReseller;
                bank.Active = updatedBank.Active;

                _db.BankAccounts.Update(bank);
                await _db.SaveChangesAsync();
         
                return Json(new { success = true, message = "Banka başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(UpdateBank));
                return Json(new { success = false, message = "Banka güncellenirken bir hata oluştu." });
            }
        }

        // POST: /Bank/UpdateBankStatus
        [HttpPost]
        public async Task<IActionResult> UpdateBankStatus([FromBody] BankStatusUpdateModel model)
        {
            if (model == null || model.Id <= 0)
            {
                return Json(new { success = false, message = "Geçersiz veri." });
            }

            try
            {
                var bank = await _db.BankAccounts.FirstOrDefaultAsync(b => b.Id == model.Id);
                if (bank == null)
                {
                    return Json(new { success = false, message = "Banka bulunamadı." });
                }

                bank.Active = model.Active;
                _db.BankAccounts.Update(bank);
                await _db.SaveChangesAsync();
         
                return Json(new { success = true, message = "Banka durumu güncellendi." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(UpdateBankStatus));
                return Json(new { success = false, message = "Banka durumu güncellenirken bir hata oluştu." });
            }
        }

        // POST: /Bank/UpdateVisibleStatus
        [HttpPost]
        public async Task<IActionResult> UpdateVisibleStatus([FromBody] BankVisibleStatusUpdateModel model)
        {
            if (model == null || model.Id <= 0)
            {
                return Json(new { success = false, message = "Geçersiz veri." });
            }

            try
            {
                var bank = await _db.BankAccounts.FirstOrDefaultAsync(b => b.Id == model.Id);
                if (bank == null)
                {
                    return Json(new { success = false, message = "Banka bulunamadı." });
                }

                bank.VisibleToReseller = model.VisibleToReseller;
                _db.BankAccounts.Update(bank);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Banka görünürlük durumu güncellendi." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(UpdateVisibleStatus));
                return Json(new { success = false, message = "Banka görünürlük durumu güncellenirken bir hata oluştu." });
            }
        }


    }

    // This model is used for updating bank status via AJAX.
    public class BankStatusUpdateModel
    {
        public int Id { get; set; }
        public bool Active { get; set; }
    }

    public class BankVisibleStatusUpdateModel
    {
        public int Id { get; set; }
        public bool VisibleToReseller { get; set; }
    }
}
