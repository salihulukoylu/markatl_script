using MarkaTLScript.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarkaTLScript.Controllers
{
    public class OperatorController : Controller
    {
        private readonly DbMarkatlScriptContext _db;
        private readonly ILogService _logService;

        public OperatorController(ILogService logService, DbMarkatlScriptContext db)
        {
            _logService = logService;
            _db = db;
        }

        public async Task<IActionResult> OperatorSettings()
        {
            try
            {
                var operators = await _db.Operators
                    .Include(o => o.Firm)
                    .Include(o => o.Type)
                    .ToListAsync();

                return View("~/Views/Kontor/OperatorAyarlari.cshtml", operators);
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(OperatorController), nameof(OperatorSettings));
                TempData["ErrorMessage"] = "Bir hata oluştu, lütfen daha sonra tekrar deneyiniz.";
                // Hata durumunda boş liste veya uygun hata sayfası dönebilirsiniz.
                return View("~/Views/Kontor/OperatorAyarlari.cshtml", Enumerable.Empty<Operator>());
            }
        }

        [HttpGet]
        public IActionResult GetFirms()
        {
            try
            {
                var firms = _db.OperatorFirms.Select(f => new { f.Id, f.FirmName }).ToList();
                return Json(new { success = true, data = firms });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(OperatorController), nameof(GetFirms));
                return Json(new { success = false, message = "Firma verileri alınırken bir hata oluştu." });
            }
        }

        [HttpGet]
        public IActionResult GetTypes()
        {
            try
            {
                var types = _db.OperatorTypes.Select(t => new { t.Id, t.TypeName }).ToList();
                return Json(new { success = true, data = types });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(OperatorController), nameof(GetTypes));
                return Json(new { success = false, message = "Operatör tipi verileri alınırken bir hata oluştu." });
            }
        }

        [HttpGet]
        public IActionResult AjaxGetOperator(int id)
        {
            try
            {
                var operatorData = _db.Operators
                    .Where(o => o.Id == id)
                    .Select(o => new
                    {
                        o.Id,
                        o.SystemName,
                        o.DisplayName,
                        o.MinSubscriberNoLength,
                        o.MaxSubscriberNoLength,
                        o.BackgroundColor,
                        o.TextColor,
                        o.IsActive,
                        o.FirmId,
                        o.TypeId,
                    })
                    .FirstOrDefault();

                if (operatorData == null)
                {
                    return Json(new { success = false, message = "Operatör bulunamadı!" });
                }

                return Json(new { success = true, operatorData });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(OperatorController), nameof(AjaxGetOperator));
                return Json(new { success = false, message = "Operatör bilgileri alınırken bir hata oluştu." });
            }
        }

        [HttpPost]
        public IActionResult UpdateOperatorStatus([FromBody] Operator model)
        {
            try
            {
                var existingOperator = _db.Operators.Find(model.Id);

                if (existingOperator == null)
                {
                    return Json(new { success = false, message = "Operatör bulunamadı!" });
                }

                // Sadece aktiflik durumunu güncelle
                existingOperator.IsActive = model.IsActive;
                _db.SaveChanges();

                return Json(new { success = true, message = "Operatör durumu başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(OperatorController), nameof(UpdateOperatorStatus));
                return Json(new { success = false, message = "Operatör durumu güncellenirken bir hata oluştu." });
            }
        }

        [HttpPost]
        public IActionResult UpdateOperator([FromBody] Operator model)
        {
            try
            {
                var existingOperator = _db.Operators.Find(model.Id);

                if (existingOperator == null)
                {
                    return Json(new { success = false, message = "Operatör bulunamadı!" });
                }

                // Güncellenen alanları set et
                existingOperator.SystemName = model.SystemName;
                existingOperator.DisplayName = model.DisplayName;
                existingOperator.MinSubscriberNoLength = model.MinSubscriberNoLength;
                existingOperator.MaxSubscriberNoLength = model.MaxSubscriberNoLength;
                existingOperator.BackgroundColor = model.BackgroundColor;
                existingOperator.TextColor = model.TextColor;
                existingOperator.IsActive = model.IsActive;

                _db.SaveChanges();

                return Json(new { success = true, message = "Operatör başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(OperatorController), nameof(UpdateOperator));
                return Json(new { success = false, message = "Operatör güncellenirken bir hata oluştu." });
            }
        }

        [HttpPost]
        public IActionResult AjaxCreateOperator([FromBody] JsonElement requestData)
        {
            try
            {
                // JSON'u doğrudan OperatorCreateDto modeline çeviriyoruz
                var dto = JsonConvert.DeserializeObject<OperatorCreateDto>(requestData.GetRawText());

                // Boş veya hatalı veri kontrolü
                if (dto == null || dto.FirmId <= 0 || dto.TypeId <= 0)
                {
                    return BadRequest(new { success = false, message = "Geçerli bir Operatör veya Tip seçiniz." });
                }
                if (dto.MinNoLength <= 0 || dto.MaxNoLength < dto.MinNoLength)
                {
                    return BadRequest(new { success = false, message = "Min-Max Abone No uzunlukları hatalı!" });
                }

                // Yeni operatör nesnesi oluştur ve gelen verileri set et
                var newOperator = new Operator
                {
                    FirmId = dto.FirmId,
                    TypeId = dto.TypeId,
                    SystemName = dto.SystemName,
                    DisplayName = dto.DisplayName,
                    MinSubscriberNoLength = dto.MinNoLength,
                    MaxSubscriberNoLength = dto.MaxNoLength,
                    BackgroundColor = dto.BgColor ?? "#FFFFFF",
                    TextColor = dto.TxtColor ?? "#000000",
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now
                };

                _db.Operators.Add(newOperator);
                _db.SaveChanges();

                return Json(new { success = true, message = "Operatör başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(OperatorController), nameof(AjaxCreateOperator));
                return BadRequest(new { success = false, message = "Bir hata oluştu!", error = ex.Message });
            }
        }

        public class OperatorCreateDto
        {
            public int FirmId { get; set; }
            public int TypeId { get; set; }
            public string SystemName { get; set; }
            public string DisplayName { get; set; }
            public int MinNoLength { get; set; }
            public int MaxNoLength { get; set; }
            public string BgColor { get; set; }
            public string TxtColor { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
