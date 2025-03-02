using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using THYWebApp.Data;
using THYWebApp.Models;
using THYWebApp.Services;

namespace THYWebApp.Controllers
{
    public class SeatController : Controller
    {
        private readonly THYContext _context;
        private readonly ILogger<SeatController> _logger;
        private readonly TicketService _ticketService;

        public SeatController(THYContext context,ILogger<SeatController> logger, TicketService ticketService)
        {
            _context = context;
            _logger = logger;
            _ticketService = ticketService;
        }

        // GET: Seats/Create
        public IActionResult Select()
        {
            var packagePrice = (HttpContext.Session.GetString("PackagePrice") ?? "Bilinmiyor").Trim();
            var passengerName = (HttpContext.Session.GetString("PassengerName") ?? HttpContext.Session.GetString("UserName"))?.Trim();
            int flightId = HttpContext.Session.GetInt32("FlightId") ?? 0;

            Console.WriteLine($"FlightID: {flightId}"); // FlightID kontrolü için log
            Console.WriteLine($"SQL Query: EXEC sp_GetSeatStatusByFlight @FlightID = {flightId}");

            try
            {
                var seats = _context.Seat
                    .FromSqlRaw("EXEC sp_GetSeatStatusByFlight @FlightID = {0}", flightId)
                    .ToList();

                Console.WriteLine($"Seats Count: {seats.Count}"); // Gelen veri sayısını logla
                foreach (var seat in seats)
                {
                    Console.WriteLine($"SeatNumber: {seat.SeatNumber}, Status: {seat.SeatStatus}");
                }

                ViewBag.Seats = seats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            ViewBag.PassengerName = passengerName;
            ViewBag.PackagePrice = packagePrice;
            return View();
        }
        public async Task<IActionResult> CheckTicket(int ticketID)
        {
            try
            {

                // Prosedürden biletin koltuk numarası, yolcu adı ve uçuşun koltuk bilgilerini çek
                var (seatNumber, passengerName, seatList) = await _ticketService.GetSeatInfoByTicketIDAsync(ticketID);

                // Eğer kullanıcıya ait koltuk numarası yoksa veya koltuk bilgileri boşsa hata göster
                if (string.IsNullOrEmpty(seatNumber) || seatList == null || !seatList.Any())
                {
                    TempData["ErrorCheckTicket"] = "Geçerli bir TicketID bulunamadı veya uçuş koltuk bilgisi mevcut değil.";
                    return RedirectToAction("Index", "Home");
                }

                // ViewBag ile verileri View'e gönder
                ViewBag.SeatInfo = seatList; // Uçuşun tüm koltuk bilgileri
                ViewBag.UserSeat = seatNumber; // Kullanıcının koltuk numarası
                ViewBag.PassengerName = passengerName; // Yolcunun adı
                ViewBag.TicketID = ticketID;
                return View("Check-in", seatList); // "Check-in" View'ine yönlendirme
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama ve geri bildirim
                _logger.LogError(ex, "Check-in sırasında hata oluştu. TicketID: {TicketID}", ticketID);
                TempData["ErrorCheckTicket"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult SaveSeat(string SeatNumber, string TotalPrice)
        {
            if (string.IsNullOrEmpty(SeatNumber))
            {
                TempData["Error"] = "Koltuk numarası boş olamaz.";
                return RedirectToAction("Select");
            }

            HttpContext.Session.SetString("SelectedSeat", SeatNumber);
            HttpContext.Session.SetString("TotalPrice", TotalPrice);

            TempData["Success"] = "Koltuk başarıyla kaydedildi.";
         
            return RedirectToAction("Create", "Payment");
        }
        [HttpPost]
        public async Task<IActionResult> SaveSeatNumber(int ticketID, string seatNumber)
        {
            try
            {
                var updateResult = await _ticketService.UpdateTicketSeatNumberAsync(ticketID, seatNumber);

                if (updateResult)
                {
                    TempData["Success"] = "Koltuk başarıyla güncellendi.";
                }
                else
                {
                    TempData["Error"] = "Koltuk güncelleme sırasında bir hata oluştu.";
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Koltuk kaydetme sırasında hata oluştu. TicketID: {TicketID}, SeatNumber: {SeatNumber}", ticketID, seatNumber);
                TempData["Error"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index", "Home");
            }
        }


    }
}
