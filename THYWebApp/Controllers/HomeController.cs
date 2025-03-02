using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using THYWebApp.Data;
using THYWebApp.Services;
using THYWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
namespace THYWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly THYContext _context;
        private readonly TicketService _ticketService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(THYContext context, ILogger<HomeController> logger, TicketService ticketService)
        {
            _context = context;
            _logger = logger;
            _ticketService = ticketService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Index action'ı çağrıldı.");
                // Session'dan kullanıcı bilgilerini ViewBag'e aktar
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserTypeID = HttpContext.Session.GetInt32("UserTypeID");
                var airports = await _context.Airport
                    .FromSqlInterpolated($"EXEC sp_GetAirports")
                    .ToListAsync();

                if (airports != null && airports.Count > 0)
                {
                    ViewBag.Airports = airports;
                }
                else
                {
                    TempData["Error"] = "Havaalanı bilgisi bulunamadı.";
                    ViewBag.Airports = new List<string>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Index action'ında hata oluştu.");
                TempData["Error"] = $"Hata: {ex.Message}";
                ViewBag.Airports = new List<string>();
            }

            return View();
        }

        // Ticket Details sayfasını yönlendirme
        // TicketDetails sayfasına yönlendirme
        public async Task<IActionResult> Ticket(int ticketId)
        {
            _logger.LogInformation("TicketID: {ticketId} Home/Ticket çağrıldı.", ticketId);

            try
            {
                var ticket = await _ticketService.GetTicketDetailsAsync(ticketId);

                if (ticket == null)
                {
                    TempData["ErrorTicket"] = "Bilet bulunamadı.";
                    _logger.LogWarning("Bilet bulunamadı. TicketId: {TicketId}", ticketId);
                    return View("Error");
                }

                return View(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ticket action'ında hata oluştu. TicketId: {TicketId}", ticketId);
                TempData["ErrorTicket"] = "Bir hata oluştu.";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string loginType, string loginInput, string password, bool rememberMe)
        {
            _logger.LogInformation("Login denemesi başladı. LoginType: {LoginType}, LoginInput: {LoginInput}", loginType, loginInput);

            // Giriş bilgilerinin eksikliği kontrolü
            if (string.IsNullOrWhiteSpace(loginInput) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorLogin"] = "Lütfen tüm alanları doldurun.";
                _logger.LogWarning("Login giriş bilgileri eksik.");
                return RedirectToAction("Index");
            }

            try
            {
                // SQL parametrelerini tanımlayın
                var parameters = new[]
                {
            new SqlParameter("@loginType", loginType),
            new SqlParameter("@loginInput", loginInput),
            new SqlParameter("@password", password)
        };

                // Kullanıcıyı SQL sorgusu ile alın
                var user = await _context.Users
                    .FromSqlRaw($@"
                SELECT TOP 1 * 
                FROM vw_UsersWithType 
                WHERE 
                    ((@loginType = 'Email' AND UserEmail = @loginInput) 
                    OR (@loginType = 'TCIdentity' AND UserTCKimlik = @loginInput) 
                    OR (@loginType = 'Phone' AND UserPhone = @loginInput)
                    OR (@loginType = 'MembershipNumber' AND MembershipNumber = @loginInput)) 
                    AND UserPassword = @password", parameters)
                    .FirstOrDefaultAsync();

                // Kullanıcı bulunamazsa
                if (user == null)
                {
                    TempData["ErrorLogin"] = "Girdiğiniz bilgilerle eşleşen bir kullanıcı bulunamadı.";
                    _logger.LogWarning("Kullanıcı bulunamadı. LoginType: {LoginType}, LoginInput: {LoginInput}", loginType, loginInput);
                    return RedirectToAction("Index");
                }
                HttpContext.Session.SetString("UserName", user.UserName+user.UserMiddleName+user.UserLastName);
                HttpContext.Session.SetInt32("UserId", user.UserID);
                // Session ayarları
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                // UserTypeID null olup olmadığını kontrol et ve yalnızca değer varsa session'a kaydet
                
                
                HttpContext.Session.SetInt32("UserTypeID", user.UserTypeID);
                ViewBag.UserTypeID = HttpContext.Session.GetInt32("UserTypeID");
               

                // Remember Me özelliği (isteğe bağlı)
                if (rememberMe)
                {
                    Response.Cookies.Append("UserName", user.UserName, new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30),
                        HttpOnly = true
                    });
                    Response.Cookies.Append("UserTypeID", (user.UserTypeID.ToString() ?? "0"), new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30),
                        HttpOnly = true
                    });

                }

                _logger.LogInformation("Login başarılı. UserName: {UserName}, UserTypeID: {UserTypeID}", user.UserName, user.UserTypeID);
                _logger.LogInformation("UserTypeID Type: {Type}", user.UserTypeID.GetType());
                _logger.LogInformation("Kullanıcı tipi kontrol ediliyor: UserTypeID={UserTypeID}", user.UserTypeID);

                if (user.UserTypeID == 2)
                {
                    _logger.LogInformation("Personel kullanıcısı tespit edildi: UserTypeID={UserTypeID}", user.UserTypeID);
                }

                if (user.UserTypeID == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (user.UserTypeID == 2)
                {
                    _logger.LogInformation("Personel kullanıcısı tespit edildi: UserTypeID={UserTypeID}", user.UserTypeID);
                    return RedirectToAction("Index", "Employee");
                }
                else if (user.UserTypeID == 3)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    TempData["ErrorLogin"] = "Yetkisiz kullanıcı girişi.";
                    _logger.LogWarning("Yetkisiz kullanıcı giriş denemesi. UserName: {UserName}, UserTypeID: {UserTypeID}", user.UserName, user.UserTypeID);
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                // Hata durumlarında loglama ve geri bildirim
                _logger.LogError(ex, "Login işleminde hata oluştu.");
                TempData["ErrorLogin"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            // Session'ı temizle
            HttpContext.Session.Clear();

            // Tarayıcı önbelleğini temizlemek için gerekli header'ları ekle
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            _logger.LogInformation("Kullanıcı başarıyla çıkış yaptı.");
            return RedirectToAction("Index");
        }

    }
}
