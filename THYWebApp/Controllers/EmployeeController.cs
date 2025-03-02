using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using THYWebApp.Data;

namespace THYWebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly THYContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(THYContext context, ILogger<EmployeeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var flights = _context.Flight
                    .Select(f => new
                    {
                        f.FlighNumber,
                        f.FlightDepartureDateTime,
                        f.FlightArrivalDateTime
                    })
                    .ToList();

                var currentTime = DateTime.Now;

                var flightStatuses = flights.Select(flight =>
                {
                    if (currentTime < flight.FlightDepartureDateTime)
                    {
                        // Kalkışa kalan süre
                        var timeToDeparture = flight.FlightDepartureDateTime - currentTime;
                        var formattedTimeToDeparture = FormatTimeDifference(timeToDeparture);
                        return new
                        {
                            FlighNumber = flight.FlighNumber,
                            Status = $"Kalkış - {formattedTimeToDeparture} sonra"
                        };
                    }
                    else if (currentTime >= flight.FlightDepartureDateTime && currentTime < flight.FlightArrivalDateTime)
                    {
                        // İnişe kalan süre
                        var timeToArrival = flight.FlightArrivalDateTime - currentTime;
                        var formattedTimeToArrival = FormatTimeDifference(timeToArrival);
                        return new
                        {
                            FlighNumber = flight.FlighNumber,
                            Status = $"İniş - {formattedTimeToArrival} sonra"
                        };
                    }
                    else
                    {
                        // Uçuş tamamlandı
                        return new
                        {
                            FlighNumber = flight.FlighNumber,
                            Status = "Tamamlandı"
                        };
                    }
                }).ToList();

                ViewBag.FlightStatuses = flightStatuses;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Bir hata oluştu: {ex.Message}";
                return View();
            }
        }

        private string FormatTimeDifference(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} gün";
            else if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours} saat";
            else if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes} dakika";
            else
                return "birkaç saniye";
        }

        [HttpPost]
        public IActionResult UpdateFlightStatus(string flightNumber)
        {
            if (string.IsNullOrEmpty(flightNumber))
            {
                TempData["ErrorUpdateFlightStatus"] = "Lütfen geçerli bir uçuş numarası girin.";
                return RedirectToAction("Index");
            }

            // Checkbox değerlerini kontrol et
            bool isDeparture = Request.Form["isDeparture"] == "on";
            bool isArrival = Request.Form["isArrival"] == "on";

            try
            {
                // Güncellenecek durumu belirleme
                string newStatus = isDeparture ? "Kalkış Yaptı" : isArrival ? "İniş Yaptı" : null;

                if (string.IsNullOrEmpty(newStatus))
                {
                    TempData["ErrorUpdateFlightStatus"] = "Lütfen bir durum seçin (Kalkış Yaptı veya İniş Yaptı).";
                    return RedirectToAction("Index");
                }

                // Prosedürü çağır
                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_UpdateFlightStatus 
                @FlighNumber = {flightNumber}, 
                @NewStatus = {newStatus}");

                TempData["SuccessUpdateFlightStatus"] = "Uçuş durumu başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorUpdateFlightStatus"] = $"Uçuş durumu güncellenirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Uçuş durumu güncellenirken hata oluştu.");
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult ReportError(string aircraftName, string errorDescription)
        {
            if (string.IsNullOrEmpty(aircraftName) || string.IsNullOrEmpty(errorDescription))
            {
                TempData["ErrorReportError"] = "Lütfen tüm alanları doldurun.";
                return RedirectToAction("Index");
            }

            try
            {
                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_InsertAircraftErrorReport 
                @AircraftName = {aircraftName}, 
                @ErrorDescription = {errorDescription}");

                TempData["SuccessReportError"] = "Hata raporu başarıyla kaydedildi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorReportError"] = $"Hata raporu kaydedilirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateFuelLevel(string aircraftName, decimal fuelLevel)
        {
            if (string.IsNullOrEmpty(aircraftName) || fuelLevel <= 0)
            {
                TempData["ErrorUpdateFuelLevel"] = "Lütfen geçerli bir uçak adı ve yakıt seviyesi girin.";
                return RedirectToAction("Index");
            }

            try
            {
                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_UpdateAircraftFuelLevel 
                @AircraftName = {aircraftName}, 
                @FuelLevel = {fuelLevel}");

                TempData["SuccessUpdateFuelLevel"] = "Yakıt durumu başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorUpdateFuelLevel"] = $"Yakıt durumu güncellenirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateAircraftAvailability(string aircraftName, bool availabilityStatus)
        {
            if (string.IsNullOrEmpty(aircraftName))
            {
                TempData["ErrorUpdateAircraftAvailability"] = "Lütfen uçak adını girin.";
                return RedirectToAction("Index");
            }

            try
            {
                // Stored procedure çağrısı
                _context.Database.ExecuteSqlInterpolated($@"
                    EXEC sp_UpdateAircraftAvailability 
                        @AircraftName = {aircraftName}, 
                        @NewAvailability = {availabilityStatus}");

                TempData["SuccessUpdateAircraftAvailability"] = "Uçak müsaitlik durumu başarıyla güncellendi.";
                _logger.LogInformation($"Uçak {aircraftName} için müsaitlik durumu {availabilityStatus} olarak güncellendi.");
            }
            catch (Exception ex)
            {
                TempData["ErrorUpdateAircraftAvailability"] = $"Uçak müsaitlik durumu güncellenirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Uçak müsaitlik durumu güncellenirken hata oluştu.");
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateFlightGates(string flightName, int? departureGate, int? arrivalGate)
        {
            // Gerekli alanları kontrol et
            if (string.IsNullOrEmpty(flightName) || departureGate == null || arrivalGate == null)
            {
                TempData["ErrorUpdateFlightGates"] = "Lütfen tüm alanları eksiksiz doldurun.";
                return RedirectToAction("Index");
            }

            try
            {
                // Stored procedure çağrısı
                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_UpdateFlightGates 
                @FlighNumber = {flightName},
                @NewDepartureGateID = {departureGate},
                @NewArrivalGateID = {arrivalGate}
        ");

                TempData["SuccessUpdateFlightGates"] = "Kapılar başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorUpdateFlightGates"] = $"Kapı güncellenirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Kapı güncellenirken hata oluştu.");
            }

            return RedirectToAction("Index");
        }


    }
}
