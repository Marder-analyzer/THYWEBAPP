using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using THYWebApp.Data;

namespace THYWebApp.Controllers
{
    public class FlightStateController : Controller
    {
        private readonly THYContext _context;
        private readonly ILogger<FlightStateController> _logger;
        public FlightStateController(THYContext context, ILogger<FlightStateController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Index(string flightNumber)
        {
            try
            {
                var flight = _context.Flightt
     .FromSql($"EXEC sp_GetFlightByNumber @FlightNumber = {flightNumber}")
     .AsEnumerable() // Sonuçları belleğe çek
     .FirstOrDefault(); // Tek kayıt döndür


                if (flight == null)
                {
                    TempData["Error"] = "Uçuş bilgisi bulunamadı.";
                    return RedirectToAction("Index", "Home");
                }

                return View(flight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Index action'ında hata oluştu. FlightNumber: {FlightNumber}", flightNumber);
                TempData["Error"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index", "Home");
            }
        }

    }

}
