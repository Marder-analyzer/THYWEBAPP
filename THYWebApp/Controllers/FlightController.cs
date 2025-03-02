using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using THYWebApp.Data;
using THYWebApp.Models;

namespace THYWebApp.Controllers
{
    public class FlightController : Controller
    {
        private readonly THYContext _context;
        private readonly ILogger<FlightController> _logger;

        public FlightController(THYContext context, ILogger<FlightController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Flights for Admin
        public async Task<IActionResult> AdminIndex()
        {
            try
            {
                var flights = await _context.Flight
                    .FromSqlInterpolated($"EXEC sp_GetFlights")
                    .ToListAsync();

                Console.WriteLine($"Çekilen Uçuş Sayısı (Admin): {flights.Count}");
                return View("AdminIndex", flights);
            }
            catch (Exception ex)
            {
                // Hata loglama
                return Content($"Hata: {ex.Message}");
            }
        }
        [HttpPost]
        public IActionResult SelectFlight(int flightId, string packageName, decimal packagePrice,int baggageAllowance)
        {
            // Seçilen uçuş paket bilgilerini session'a kaydet
            HttpContext.Session.SetInt32("FlightId", flightId);
            HttpContext.Session.SetInt32("BaggageAllowance", baggageAllowance);
            HttpContext.Session.SetString("PackageName", packageName);
            HttpContext.Session.SetString("PackagePrice", packagePrice.ToString());
            // Kullanıcının oturumda UserName bilgisi olup olmadığını kontrol et
            var userName = HttpContext.Session.GetString("UserName");
            // Yolcu bilgileri sayfasına yönlendir
            if (!string.IsNullOrEmpty(userName))
            {
                // Kullanıcı giriş yapmışsa User/Index'e yönlendir
                return RedirectToAction("Index", "Users");
            }
            else
            {
                // Kullanıcı giriş yapmamışsa NonMemberCustomer/Create'e yönlendir
                return RedirectToAction("Create", "NonMemberCustomer");
            }
        }
        [HttpGet]
        public IActionResult SearchFlight()
        {
            return View();
        }
        // GET: Flights for Users
        public async Task<IActionResult> Index(string departureAirportName, string arrivalAirportName, DateTime? departureDate, string passengerType)
        {
            _logger.LogInformation("Index action called with parameters: DepartureAirportName={DepartureAirportName}, ArrivalAirportName={ArrivalAirportName}, DepartureDate={DepartureDate}, PassengerType={PassengerType}", departureAirportName, arrivalAirportName, departureDate, passengerType);

            try
            {
                // Parametrelerin doldurulduğunu kontrol et
                if (string.IsNullOrWhiteSpace(departureAirportName) || string.IsNullOrWhiteSpace(arrivalAirportName) || !departureDate.HasValue || string.IsNullOrWhiteSpace(passengerType))
                {
                    TempData["Error"] = "Lütfen tüm alanları doldurun.";
                    _logger.LogWarning("Missing input parameters.");
                    return View(Enumerable.Empty<Flight>());
                }

                // Uçuş sorgusunu havaalanı isimlerini kullanarak çalıştır
                var flights = await _context.Flight
                    .FromSqlInterpolated($"EXEC sp_SearchFlight @DepartureAirportName = {departureAirportName}, @ArrivalAirportName = {arrivalAirportName}, @DepartureDate = {departureDate}, @PassengerType = {passengerType}")
                    .ToListAsync();

                _logger.LogInformation("Number of flights retrieved: {Count}", flights.Count);

                if (!flights.Any())
                {
                    TempData["Error"] = "Belirtilen kriterlere uygun uçuş bulunamadı.";
                    _logger.LogWarning("No flights found for the given criteria.");
                    return View(Enumerable.Empty<Flight>());
                }

                // TravelDuration değerini al ve Session'a kaydet
                var firstFlight = flights.FirstOrDefault();
                if (firstFlight != null)
                {
                    if (firstFlight.TravelDuration != null)
                    {
                        HttpContext.Session.SetString("TravelDuration", firstFlight.TravelDuration);
                        _logger.LogInformation("TravelDuration set to session: {TravelDuration}", firstFlight.TravelDuration);
                    }
                    else
                    {
                        _logger.LogWarning("TravelDuration is null for the first flight.");
                    }
                }

                ViewBag.TravelDuration = HttpContext.Session.GetString("TravelDuration");

                // ViewBag ile uçuş bilgilerinin özetini paylaşabilirsiniz
                ViewBag.DepartureAirportName = departureAirportName;
                ViewBag.ArrivalAirportName = arrivalAirportName;
                ViewBag.DepartureDate = departureDate;

                // Session'a uçuş bilgilerini kaydet
                HttpContext.Session.SetString("DepartureAirportName", departureAirportName);
                HttpContext.Session.SetString("ArrivalAirportName", arrivalAirportName);
                HttpContext.Session.SetString("DepartureDate", departureDate.Value.ToString("yyyy-MM-ddTHH:mm"));
                HttpContext.Session.SetString("PassengerType", passengerType);

                return View(flights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Index action.");
                TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                return View(Enumerable.Empty<Flight>());
            }
        }










        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound("Geçersiz uçuş ID'si.");
            }

            try
            {
                var flight = await _context.Flight
                    .FromSqlInterpolated($"EXEC sp_GetFlightDetails @FlightID = {id}")
                    .FirstOrDefaultAsync();

                if (flight == null)
                {
                    return NotFound("Uçuş bulunamadı.");
                }

                return View(flight);
            }
            catch (Exception ex)
            {
                // Hata loglama
                return Content($"Hata: {ex.Message}");
            }
        }


        // POST: Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlightNumber,AircraftID,DepartureAirportID,ArrivalAirportID,FlightDepartureDateTime,FlightArrivalDateTime,FlightDelayTime,DepartureGateID,ArrivalGateID,FlightStatus,FlightAirlineCompany,FlightPriceID")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC sp_InsertFlight @FlightNumber = {flight.FlighNumber}, @AircraftID = {flight.AircraftID}, @DepartureAirportID = {flight.DepartureAirportID}, @ArrivalAirportID = {flight.ArrivalAirportID}, @FlightDepartureDateTime = {flight.FlightDepartureDateTime}, @FlightArrivalDateTime = {flight.FlightArrivalDateTime}, @FlightDelayTime = {flight.FlightDelayTime}, @DepartureGateID = {flight.DepartureGateID}, @ArrivalGateID = {flight.ArrivalGateID}, @FlightStatus = {flight.FlightStatus}, @FlightAirlineCompany = {flight.FlightAirlineCompany}, @FlightPriceID = {flight.FlightPriceID}");

                    return RedirectToAction(nameof(AdminIndex));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Hata: {ex.Message}";
                    return View(flight);
                }
            }
            return View(flight);
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound("Geçersiz uçuş ID'si.");
            }

            try
            {
                var flight = await _context.Flight.FindAsync(id);
                if (flight == null)
                {
                    return NotFound("Uçuş bulunamadı.");
                }

                return View(flight);
            }
            catch (Exception ex)
            {
                // Hata loglama
                return Content($"Hata: {ex.Message}");
            }
        }

        // POST: Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FlightID,FlightNumber,AircraftID,DepartureAirportID,ArrivalAirportID,FlightDepartureDateTime,FlightArrivalDateTime,FlightDelayTime,DepartureGateID,ArrivalGateID,FlightStatus,FlightAirlineCompany,FlightPriceID")] Flight flight)
        {
            if (id != flight.FlightID)
            {
                return NotFound("Geçersiz uçuş ID'si.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC sp_UpdateFlight @FlightID = {flight.FlightID}, @FlightNumber = {flight.FlighNumber}, @AircraftID = {flight.AircraftID}, @DepartureAirportID = {flight.DepartureAirportID}, @ArrivalAirportID = {flight.ArrivalAirportID}, @FlightDepartureDateTime = {flight.FlightDepartureDateTime}, @FlightArrivalDateTime = {flight.FlightArrivalDateTime}, @FlightDelayTime = {flight.FlightDelayTime}, @DepartureGateID = {flight.DepartureGateID}, @ArrivalGateID = {flight.ArrivalGateID}, @FlightStatus = {flight.FlightStatus}, @FlightAirlineCompany = {flight.FlightAirlineCompany}, @FlightPriceID = {flight.FlightPriceID}");

                    return RedirectToAction(nameof(AdminIndex));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Hata: {ex.Message}";
                    return View(flight);
                }
            }
            return View(flight);
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound("Geçersiz uçuş ID'si.");
            }

            try
            {
                var flight = await _context.Flight.FindAsync(id);
                if (flight == null)
                {
                    return NotFound("Uçuş bulunamadı.");
                }

                return View(flight);
            }
            catch (Exception ex)
            {
                // Hata loglama
                return Content($"Hata: {ex.Message}");
            }
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC sp_DeleteFlight @FlightID = {id}");
                return RedirectToAction(nameof(AdminIndex));
            }
            catch (Exception ex)
            {
                // Hata loglama
                return Content($"Hata: {ex.Message}");
            }
        }

        private bool FlightExists(int id)
        {
            return _context.Flight.Any(e => e.FlightID == id);
        }
    }
}