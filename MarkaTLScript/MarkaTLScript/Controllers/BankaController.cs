using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarkaTLScript.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarkaTLScript.ViewModels;
using System.Collections.Generic;

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
                var viewModels = new List<BankAccountViewModel>();

                foreach (var bank in banks)
                {
                    // Sum income movements for this bank
                    var income = await _db.BankAccountMovements
                        .Where(m => m.BankId == bank.Id && m.TransactionType.ToLower() == "income")
                        .SumAsync(m => (decimal?)m.Amount) ?? 0;

                    // Sum expense movements for this bank
                    var expense = await _db.BankAccountMovements
                        .Where(m => m.BankId == bank.Id && m.TransactionType.ToLower() == "expense")
                        .SumAsync(m => (decimal?)m.Amount) ?? 0;

                    viewModels.Add(new BankAccountViewModel
                    {
                        BankAccount = bank,
                        Balance = income - expense
                    });
                }

                return View("~/Views/Odeme/BankaAyarlari.cshtml", viewModels);
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(BankaAyarlari));
                TempData["ErrorMessage"] = "Bir hata oluştu, lütfen daha sonra tekrar deneyiniz.";
                return View("~/Views/Odeme/BankaAyarlari.cshtml", Enumerable.Empty<BankAccountViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListAccountMovements(int bankId)
        {
            try
            {
                var movements = await _db.BankAccountMovements
                    .Where(m => m.BankId == bankId)
                    .OrderByDescending(m => m.TransactionDate)
                    .Select(m => new
                    {
                        transactionDate = m.TransactionDate.ToString("yyyy-MM-dd HH:mm"),
                        transactionType = m.TransactionType,
                        amount = m.Amount,
                        description = m.Description
                    })
                    .ToListAsync();

                return Json(new { success = true, data = movements });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(ListAccountMovements));
                return Json(new { success = false, message = "Hesap hareketleri alınırken hata oluştu." });
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


        [HttpGet]
        public async Task<IActionResult> AjaxGetBanks()
        {
            try
            {
                var banks = await _db.BankAccounts
                    .Select(b => new
                    {
                        id = b.Id,
                        bankName = b.BankName,
                        accountHolder = b.AccountHolder
                    })
                    .ToListAsync();
                return Json(new { success = true, data = banks });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(AjaxGetBanks));
                return Json(new { success = false, message = "Banka verileri alınırken hata oluştu." });
            }
        }

        // GET: /Banka/GetIncomeCategories
        [HttpGet]
        public async Task<IActionResult> AjaxGetIncomeCategories()
        {
            try
            {
                var incomeCategories = await _db.IncomeCategories
                    .Select(ic => new
                    {
                        id = ic.Id,
                        title = ic.Title
                    })
                    .ToListAsync();
                return Json(new { success = true, data = incomeCategories });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(AjaxGetIncomeCategories));
                return Json(new { success = false, message = "Gelir kalemleri alınırken hata oluştu." });
            }
        }

        // GET: /Banka/GetExpenseCategories
        [HttpGet]
        public async Task<IActionResult> AjaxGetExpenseCategories()
        {
            try
            {
                var expenseCategories = await _db.ExpenseCategories
                    .Select(ec => new
                    {
                        id = ec.Id,
                        title = ec.Title
                    })
                    .ToListAsync();
                return Json(new { success = true, data = expenseCategories });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(AjaxGetExpenseCategories));
                return Json(new { success = false, message = "Gider kalemleri alınırken hata oluştu." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddIncomeMovement([FromBody] IncomeMovementDto dto)
        {
            if (dto == null || dto.BankId <= 0 || dto.IncomeCategoryId <= 0 || dto.Amount <= 0 || string.IsNullOrEmpty(dto.TransactionDate))
            {
                return Json(new { success = false, message = "Gerekli bilgiler eksik." });
            }

            try
            {
                // Create a new bank account movement for income
                var movement = new BankAccountMovement
                {
                    BankId = dto.BankId,
                    TransactionType = "income",
                    IncomeCategoryId = dto.IncomeCategoryId,
                    Amount = dto.Amount,
                    TransactionDate = DateTime.Parse(dto.TransactionDate),
                    Description = dto.Description
                };

                _db.BankAccountMovements.Add(movement);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Gelir işlemi başarıyla kaydedildi." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(AddIncomeMovement));
                return Json(new { success = false, message = "Gelir işlemi kaydedilirken hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddExpenseMovement([FromBody] ExpenseMovementDto dto)
        {
            if (dto == null || dto.BankId <= 0 || dto.ExpenseCategoryId <= 0 || dto.Amount <= 0 || string.IsNullOrEmpty(dto.TransactionDate))
            {
                return Json(new { success = false, message = "Gerekli bilgiler eksik." });
            }

            try
            {
                // Create a new bank account movement for expense
                var movement = new BankAccountMovement
                {
                    BankId = dto.BankId,
                    TransactionType = "expense",
                    ExpenseCategoryId = dto.ExpenseCategoryId,
                    Amount = dto.Amount,
                    TransactionDate = DateTime.Parse(dto.TransactionDate),
                    Description = dto.Description
                };

                _db.BankAccountMovements.Add(movement);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Gider işlemi başarıyla kaydedildi." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(BankaController), nameof(AddExpenseMovement));
                return Json(new { success = false, message = "Gider işlemi kaydedilirken hata oluştu." });
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

    public class IncomeMovementDto
    {
        public int BankId { get; set; }
        public int IncomeCategoryId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionDate { get; set; }
        public string Description { get; set; }
    }

    public class ExpenseMovementDto
    {
        public int BankId { get; set; }
        public int ExpenseCategoryId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionDate { get; set; }
        public string Description { get; set; }
    }


}
