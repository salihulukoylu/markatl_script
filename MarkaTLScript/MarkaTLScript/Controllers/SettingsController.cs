using MarkaTLScript.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarkaTLScript.Controllers
{
    public class SettingsController : Controller
    {
        private readonly DbMarkatlScriptContext _db;

        public SettingsController(DbMarkatlScriptContext db)
        {
            _db = db;
        }

        // GET: Settings/SystemSettings
        public async Task<IActionResult> SystemSettings()
        {
            // Sistemdeki mevcut ayarları alıyoruz.
            var systemSetting = await _db.SystemSettings.FirstOrDefaultAsync();
            if (systemSetting == null)
            {
                // Eğer sistemde hiç ayar yoksa, yeni bir ayar modeli oluştur
                systemSetting = new SystemSetting();
            }

            return View("~/Views/Settings/SistemAyarlari.cshtml", systemSetting);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SystemSettings(SystemSetting model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Mevcut kaydı veritabanından al
                    var existingSetting = await _db.SystemSettings.FirstOrDefaultAsync();

                    if (existingSetting == null)
                    {
                        // Kayıt yoksa yeni ekle
                        _db.SystemSettings.Add(model);
                        await _db.SaveChangesAsync();
                        // Eklenen kaydı tekrar okuyalım
                        existingSetting = model;

                        TempData["SweetAlertMessage"] = "Güncelleme başarıyla tamamlandı!";
                        TempData["SweetAlertType"] = "success";
                    }
                    else
                    {
                        // Kayıt varsa güncelle
                        existingSetting.SiteName = model.SiteName;
                        existingSetting.CompanyName = model.CompanyName;
                        existingSetting.SupportPhone = model.SupportPhone;
                        existingSetting.SystemStatus = model.SystemStatus;
                        existingSetting.SiteClosedMessage = model.SiteClosedMessage;
                        existingSetting.TelegramNotificationEnabled = model.TelegramNotificationEnabled;
                        existingSetting.KontorStatus = model.KontorStatus;
                        existingSetting.GameStatus = model.GameStatus;
                        existingSetting.SmsStatus = model.SmsStatus;
                        existingSetting.SafeIps = model.SafeIps;


                        await _db.SaveChangesAsync();
                        TempData["SweetAlertMessage"] = "Güncelleme başarıyla tamamlandı!";
                        TempData["SweetAlertType"] = "success";
                    }



                }
                else
                {
                    TempData["SweetAlertMessage"] = "Form doğrulama hatası. Eksik veya hatalı alanlar var.";
                    TempData["SweetAlertType"] = "warning";
                }

                // ModelState geçerli değilse,
                // gelen model ile aynı sayfayı göster
               
            }
            catch (Exception ex)
            {
                TempData["SweetAlertMessage"] = "Güncelleme başarısız. " + ex.Message;
                TempData["SweetAlertType"] = "error";
            }

            return View("~/Views/Settings/SistemAyarlari.cshtml", model);
        }



        // GET: Settings/UserSettings
        public IActionResult UserSettings()
        {
            // Burada, kullanıcı ayarlarıyla ilgili veriler alınacak
            return View("~/Views/Settings/KullaniciAyarlari.cshtml");
        }

        // GET: Settings/NotificationSettings
        public IActionResult NotificationSettings()
        {
            // Bildirim sesleriyle ilgili ayarları almak
            return View("~/Views/Settings/BildirimAyarlari.cshtml");
        }
    }
}
