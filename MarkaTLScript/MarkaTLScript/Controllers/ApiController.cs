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
    public class ApiController : Controller
    {
        private readonly DbMarkatlScriptContext _db;
        private readonly ILogService _logService;

        public ApiController(ILogService logService, DbMarkatlScriptContext db)
        {
            _logService = logService;
            _db = db;
        }
        public async Task<IActionResult> ApiListesi()
        {
            try
            {
                var apis = await _db.ApiDefinitions
                    .Include(a => a.ApiType) // API türü bilgilerini çekiyoruz
                    .ToListAsync();

                return View("~/Views/Kontor/KontorApileri.cshtml", apis);
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ApiController), nameof(ApiListesi));
                TempData["ErrorMessage"] = "Bir hata oluştu, lütfen daha sonra tekrar deneyiniz.";
                return View("~/Views/Kontor/KontorApileri.cshtml", Enumerable.Empty<ApiDefinition>());
            }
        }



        // API Türleri listesini döner
        [HttpGet]
        public IActionResult GetApiTypes()
        {
            try
            {
                var types = _db.ApiTypeLists
                               .Select(t => new { id = t.Id, type_name = t.TypeName })
                               .ToList();
                return Json(new { success = true, data = types });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ApiController), nameof(GetApiTypes));
                return Json(new { success = false, message = "API türleri alınırken bir hata oluştu." });
            }
        }

        // Yeni API tanımını ekler
        [HttpPost]
        public IActionResult AjaxCreateApi([FromBody] JsonElement requestData)
        {
            try
            {
                // JSON'u doğrudan ApiCreateDto modeline çeviriyoruz
                var dto = JsonConvert.DeserializeObject<ApiCreateDto>(requestData.GetRawText());

                // Gerekli alan kontrolü
                if (dto == null || dto.ApiTypeId <= 0 ||
                    string.IsNullOrEmpty(dto.ApiDescription) ||
                    string.IsNullOrEmpty(dto.SiteAddress) ||
                    string.IsNullOrEmpty(dto.UserCode) ||
                    string.IsNullOrEmpty(dto.Username) ||
                    string.IsNullOrEmpty(dto.Password))
                {
                    return BadRequest(new { success = false, message = "Lütfen tüm gerekli alanları doldurunuz." });
                }

                var newApi = new ApiDefinition
                {
                    ApiTypeId = dto.ApiTypeId,
                    ApiDescription = dto.ApiDescription,
                    SiteAddress = dto.SiteAddress,
                    UserCode = dto.UserCode,
                    Username = dto.Username,
                    Password = dto.Password,
                    KontorStatus = dto.KontorStatus,
                    FaturaStatus = dto.FaturaStatus,
                    OyunStatus = dto.OyunStatus,
                    ApiStatus = dto.ApiStatus,    // Genel API durumu
                    WorkingHours = dto.WorkingHours,
                   
                };

                _db.ApiDefinitions.Add(newApi);
                _db.SaveChanges();

                return Json(new { success = true, message = "API başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ApiController), nameof(AjaxCreateApi));
                return BadRequest(new { success = false, message = "API eklenirken bir hata oluştu.", error = ex.Message });
            }
        }

        // Belirtilen API tanımını detaylarıyla getirir
        [HttpGet]
        public IActionResult AjaxGetApi(int id)
        {
            try
            {
                var apiData = _db.ApiDefinitions
                                 .Where(a => a.Id == id)
                                 .Select(a => new
                                 {
                                     a.Id,
                                     a.ApiTypeId,
                                     a.ApiDescription,
                                     a.SiteAddress,
                                     a.UserCode,
                                     a.Username,
                                     a.Password,
                                     a.KontorStatus,
                                     a.FaturaStatus,
                                     a.OyunStatus,
                                     a.ApiStatus,          // Genel API durumu
                                     a.WorkingHours
                                 })
                                 .FirstOrDefault();

                if (apiData == null)
                {
                    return Json(new { success = false, message = "API bulunamadı!" });
                }

                return Json(new { success = true, apiData });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ApiController), nameof(AjaxGetApi));
                return Json(new { success = false, message = "API bilgileri alınırken bir hata oluştu." });
            }
        }

        // Mevcut API tanımını günceller
        [HttpPost]
        public IActionResult UpdateApi([FromBody] ApiDefinition model)
        {
            try
            {
                var existingApi = _db.ApiDefinitions.Find(model.Id);
                if (existingApi == null)
                {
                    return Json(new { success = false, message = "API bulunamadı!" });
                }

                existingApi.ApiTypeId = model.ApiTypeId;
                existingApi.ApiDescription = model.ApiDescription;
                existingApi.SiteAddress = model.SiteAddress;
                existingApi.UserCode = model.UserCode;
                existingApi.Username = model.Username;
                existingApi.Password = model.Password;
                existingApi.KontorStatus = model.KontorStatus;
                existingApi.FaturaStatus = model.FaturaStatus;
                existingApi.OyunStatus = model.OyunStatus;
                existingApi.ApiStatus = model.ApiStatus; // Genel API durumu güncelleniyor
                existingApi.WorkingHours = model.WorkingHours;

                _db.SaveChanges();

                return Json(new { success = true, message = "API başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ApiController), nameof(UpdateApi));
                return Json(new { success = false, message = "API güncellenirken bir hata oluştu." });
            }
        }

        // Durum alanlarını (kontör, fatura, oyun, genel API durumu) dinamik olarak günceller
        [HttpPost]
        public IActionResult UpdateApiStatus([FromBody] ApiStatusUpdateDto dto)
        {
            try
            {
                var apiDef = _db.ApiDefinitions.Find(dto.Id);
                if (apiDef == null)
                {
                    return Json(new { success = false, message = "API bulunamadı!" });
                }

                // dto.Field hangi status alanını güncelleyeceğimizi belirler
                switch (dto.Field.ToLower())
                {
                    case "kontorstatus":
                        apiDef.KontorStatus = dto.Value;
                        break;
                    case "faturastatus":
                        apiDef.FaturaStatus = dto.Value;
                        break;
                    case "oyunstatus":
                        apiDef.OyunStatus = dto.Value;
                        break;
                    case "apistatus":
                        apiDef.ApiStatus = dto.Value;
                        break;
                    default:
                        return Json(new { success = false, message = "Geçersiz alan." });
                }

                _db.SaveChanges();
                return Json(new { success = true, message = "API durumu güncellendi!" });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ApiController), nameof(UpdateApiStatus));
                return Json(new { success = false, message = "Durum güncellenirken bir hata oluştu." });
            }
        }


        // Yeni API bakiye işlemi ekleme (ekleme/çıkarma/virman)
        [HttpPost]
        public async Task<IActionResult> AddBalanceTransaction([FromBody] ApiBalanceTransactionDto dto)
        {
            try
            {
                // Gerekli alan kontrolü
                if (dto == null || dto.ApiDefinitionId <= 0 || dto.Amount <= 0 || string.IsNullOrEmpty(dto.TransactionType))
                {
                    return BadRequest(new { success = false, message = "Gerekli bilgiler eksik." });
                }

                var transaction = new ApiBalanceTransaction
                {
                    ApiDefinitionId = dto.ApiDefinitionId,
                    TransactionType = dto.TransactionType,
                    BankId = dto.BankId, // Varsa
                    Amount = dto.Amount,
                    Description = dto.Description,
                    TransactionDate = DateTime.Now
                };

                _db.ApiBalanceTransactions.Add(transaction);
                await _db.SaveChangesAsync();

                // İlgili API'nin bakiye güncellenmesi veya hesap hareketi oluşturulması işlemleri burada yapılabilir.
                // API hesabındaki mevcut bakiye alınır
                var currentBalance = await _db.ApiAccountMovements
                    .Where(m => m.ApiDefinitionId == dto.ApiDefinitionId)
                    .OrderByDescending(m => m.TransactionDate)
                    .Select(m => m.NewBalance)
                    .FirstOrDefaultAsync();

                // Yeni bakiye hesaplanır
                decimal newBalance = currentBalance + dto.Amount; // Örneğin, ekleme işlemi için

                // ApiAccountMovement kaydı oluşturulur
                var accountMovement = new ApiAccountMovement
                {
                    ApiDefinitionId = dto.ApiDefinitionId,
                    MovementType = "Bakiye Güncelleme", // Örnek işlem türü
                    Amount = dto.Amount,
                    PreviousBalance = currentBalance,
                    NewBalance = newBalance,
                    TransactionDate = DateTime.Now,
                    Description = dto.Description
                };

                _db.ApiAccountMovements.Add(accountMovement);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Bakiye işlemi başarıyla kaydedildi." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ApiController), nameof(AddBalanceTransaction));
                return Json(new { success = false, message = "Bakiye işlemi kaydedilirken hata oluştu." });
            }
        }

        // API hesap hareketlerini listeleyen endpoint
        [HttpGet]
        public async Task<IActionResult> ListAccountMovements(int apiDefinitionId)
        {
            try
            {
                var movements = await _db.ApiAccountMovements
                    .Where(m => m.ApiDefinitionId == apiDefinitionId)
                    .OrderByDescending(m => m.TransactionDate)
                    .ToListAsync();

                return Json(new { success = true, data = movements });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ApiController), nameof(ListAccountMovements));
                return Json(new { success = false, message = "Hesap hareketleri alınırken hata oluştu." });
            }
        }

        // API'ler arası virman işlemi örneği: Belirli bir API'den çıkarıp başka bir API'ye ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> TransferBalance([FromBody] ApiTransferDto dto)
        {
            try
            {
                if (dto == null || dto.SourceApiId <= 0 || dto.TargetApiId <= 0 || dto.Amount <= 0)
                {
                    return BadRequest(new { success = false, message = "Geçersiz bilgiler." });
                }

                // İşlemin ayrıntılarını kaydetmek için iki bakiye işlemi oluşturabilirsiniz.
                // Örneğin:
                // 1. Kaynak API'den bakiyeyi düşürmek için "Virman Çıkışı" işlemi.
                var sourceTransaction = new ApiBalanceTransaction
                {
                    ApiDefinitionId = dto.SourceApiId,
                    TransactionType = "Virman Çıkışı", // Örnek işlem türü
                    Amount = dto.Amount,
                    Description = $"Virman: {dto.Amount} {dto.Description}",
                    TransactionDate = DateTime.Now
                };
                _db.ApiBalanceTransactions.Add(sourceTransaction);

                // 2. Hedef API'ye bakiyeyi eklemek için "Virman Girişi" işlemi.
                var targetTransaction = new ApiBalanceTransaction
                {
                    ApiDefinitionId = dto.TargetApiId,
                    TransactionType = "Virman Girişi", // Örnek işlem türü
                    Amount = dto.Amount,
                    Description = $"Virman: {dto.Amount} {dto.Description}",
                    TransactionDate = DateTime.Now
                };
                _db.ApiBalanceTransactions.Add(targetTransaction);

                await _db.SaveChangesAsync();

                // Her iki işlem için ayrıca hesap hareketleri oluşturabilirsiniz.

                return Json(new { success = true, message = "Virman işlemi başarıyla tamamlandı." });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, nameof(ApiController), nameof(TransferBalance));
                return Json(new { success = false, message = "Virman işlemi sırasında hata oluştu." });
            }
        }

    }

    public class ApiBalanceTransactionDto
    {
        public int ApiDefinitionId { get; set; }
        public string TransactionType { get; set; } // 'Borç Bakiye', 'Borç Ödeme', 'Nakit Bakiye', vs.
        public int? BankId { get; set; } // Opsiyonel
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    // API transfer işlemi için DTO
    public class ApiTransferDto
    {
        public int SourceApiId { get; set; }
        public int TargetApiId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
    // Yeni API ekleme için kullanılan DTO
    public class ApiCreateDto
    {
        public int ApiTypeId { get; set; }
        public string ApiDescription { get; set; }
        public string SiteAddress { get; set; }
        public string UserCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool KontorStatus { get; set; }
        public bool FaturaStatus { get; set; }
        public bool OyunStatus { get; set; }
        public bool ApiStatus { get; set; } // Genel API durumu
        public string WorkingHours { get; set; }
    }

    // UpdateApiStatus endpoint'inde kullanılacak DTO
    public class ApiStatusUpdateDto
    {
        public int Id { get; set; }
        public string Field { get; set; }
        public bool Value { get; set; }
    }
}
