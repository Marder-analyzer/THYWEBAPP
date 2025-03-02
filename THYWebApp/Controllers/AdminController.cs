using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using THYWebApp.Data;
using THYWebApp.Models;
using THYWebApp.Services;

namespace THYWebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly THYContext _context;
        private readonly TicketService _ticketService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(THYContext context, ILogger<AdminController> logger, TicketService ticketService)
        {
            _context = context;
            _logger = logger;
            _ticketService = ticketService;
        }
            public IActionResult Index()
        {
            try
            {
                // Hataları stored procedure ile al
                var errorReports = _context.AircraftErrorReport
                    .FromSqlInterpolated($"EXEC sp_GetAircraftErrorReports")
                    .ToList();

                // View'e model olarak gönder
                return View(errorReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hatalar yüklenirken bir sorun oluştu.");
                ViewBag.ErrorMessage = "Hatalar yüklenirken bir sorun oluştu.";
                return View(new List<AircraftErrorReport>()); // Boş bir liste döndür
            }
        }
        [HttpGet]
        public IActionResult GetAircrafts()
        {
            try
            {
                var aircrafts = _context.Aircraft
                    .FromSqlInterpolated($"EXEC sp_GetAircrafts")
                    .ToList();

                if (aircrafts.Count == 0)
                {
                    return Content("<p>Henüz uçak kaydı bulunmamaktadır.</p>", "text/html");
                }

                string tableHtml = "<table border='1' cellpadding='10'><thead><tr>" +
                                   "<th>Uçak ID</th><th>Uçak Adı</th><th>Model</th><th>Üretim Tarihi</th><th>Koltuk Kapasitesi</th>" +
                                   "</tr></thead><tbody>";

                foreach (var aircraft in aircrafts)
                {
                    tableHtml += $"<tr><td>{aircraft.AircraftID}</td><td>{aircraft.AircraftName}</td>" +
                                 $"<td>{aircraft.AircraftModel}</td><td>{aircraft.AircraftProductionDate.ToShortDateString()}</td>" +
                                 $"<td>{aircraft.AircraftSeatCapacity}</td></tr>";
                }

                tableHtml += "</tbody></table>";
                return Content(tableHtml, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uçaklar alınırken hata oluştu.");
                return Content($"<p>Hata oluştu: {ex.Message}</p>", "text/html");
            }
        }
        [HttpGet]
        public IActionResult GetAirports()
        {
            try
            {
                var airports = _context.Airport
                    .FromSqlInterpolated($"EXEC sp_GetAirports")
                    .ToList();

                if (airports.Count == 0)
                {
                    return Content("<p>Henüz havaalanı kaydı bulunmamaktadır.</p>", "text/html");
                }

                string tableHtml = "<table border='1' cellpadding='10'><thead><tr>" +
                                   "<th>Havaalanı ID</th><th>Havaalanı Adı</th>" +
                                   "</tr></thead><tbody>";

                foreach (var airport in airports)
                {
                    tableHtml += $"<tr><td>{airport.AirportID}</td><td>{airport.AirportName}</td>" ;
                }

                tableHtml += "</tbody></table>";
                return Content(tableHtml, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Havaalanlar alınırken hata oluştu.");
                return Content($"<p>Hata oluştu: {ex.Message}</p>", "text/html");
            }
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _context.Users
                    .FromSqlInterpolated($"EXEC sp_GetUsers")
                    .ToList();

                if (users.Count == 0)
                {
                    return Content("<p>Henüz kullanıcı kaydı bulunmamaktadır.</p>", "text/html");
                }

                string tableHtml = "<table border='1' cellpadding='10'><thead><tr>" +
                                   "<th>Yetki Derecesi</th><th>Adı</th>" +
                                   "</tr></thead><tbody>";

                foreach (var user in users)
                {
                    string fullName = $"{user.UserName}";

                    if (!string.IsNullOrWhiteSpace(user.UserMiddleName))
                    {
                        fullName += $" {user.UserMiddleName}";
                    }

                    fullName += $" {user.UserLastName}";

                    tableHtml += $"<tr><td>{user.UserTypeID}</td><td>{fullName}</td>";
                }

                tableHtml += "</tbody></table>";
                return Content(tableHtml, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcılar alınırken hata oluştu.");
                return Content($"<p>Hata oluştu: {ex.Message}</p>", "text/html");
            }
        }
        [HttpPost]
        public IActionResult AddAircraft(string AircraftName, string AircraftModel, DateTime AircraftProductionDate, int AircraftSeatCapacity)
        {
            if (string.IsNullOrEmpty(AircraftName) ||
                string.IsNullOrEmpty(AircraftModel) ||
                AircraftSeatCapacity <= 0 ||
                AircraftProductionDate == default)
            {
                TempData["ErrorAddAircraft"] = "Lütfen tüm alanları doğru doldurun.";
                return View("Index"); // Formun bulunduğu sayfaya geri dön
            }

            try
            {
                _context.Database.ExecuteSqlRaw(
    "EXEC sp_InsertAircraft @AircraftName, @AircraftModel, @AircraftProductionDate, @AircraftSeatCapacity",
    new SqlParameter("@AircraftName", AircraftName),
    new SqlParameter("@AircraftModel", AircraftModel),
    new SqlParameter("@AircraftProductionDate", AircraftProductionDate),
    new SqlParameter("@AircraftSeatCapacity", AircraftSeatCapacity));


                TempData["SuccessAddAircraft"] = "Uçak başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorAddAircraft"] = $"Bir hata oluştu: {ex.Message}";
            }

            return View("Index"); // Formun bulunduğu sayfaya geri dön
        }
        [HttpGet]
        public IActionResult GetFlights()
        {
            try
            {
                // Prosedürden uçuş bilgilerini çek
                var flights = _context.AdminFlight
                    .FromSqlInterpolated($"EXEC sp_GetFlights")
                    .ToList();

                // Verileri HTML formatında döndür
                var html = new StringBuilder();
                html.Append("<table border='1' cellpadding='10'>");
                html.Append("<thead><tr><th>Numarası</th><th>Uçak</th><th>Kalkış</th><th>Varış</th><th>Gecikme</th></tr></thead>");
                html.Append("<tbody>");
                foreach (var flight in flights)
                {
                    html.Append("<tr>");
                    html.Append($"<td>{flight.FlighNumber}</td>");
                    html.Append($"<td>{flight.AircraftName}</td>");
                    html.Append($"<td>{flight.DepartureAirportName} ({flight.DepartureAirportCode})</td>");
                    html.Append($"<td>{flight.ArrivalAirportName} ({flight.ArrivalAirportCode})</td>");
                    html.Append($"<td>{FormatDelayTime(flight.FlightDelayTime)}</td>");
                    html.Append("</tr>");
                }

                html.Append("</tbody></table>");
                return Content(html.ToString(), "text/html");
            }
            catch (Exception ex)
            {
                return Content($"<p>Hata oluştu: {ex.Message}</p>", "text/html");
            }
        }

        // Helper method
        private string FormatDelayTime(int delayMinutes)
        {
            if (delayMinutes >= 1440)
            {
                int days = delayMinutes / 1440;
                int hours = (delayMinutes % 1440) / 60;
                int minutes = delayMinutes % 60;
                return $"{days} gün {hours} saat {minutes} dakika";
            }
            else if (delayMinutes >= 60)
            {
                int hours = delayMinutes / 60;
                int minutes = delayMinutes % 60;
                return $"{hours} saat {minutes} dakika";
            }
            else
            {
                return $"{delayMinutes} dakika";
            }
        }
        [HttpPost]
        public IActionResult AddAirport(string airportName, string airportCity, string airportCountry, string airportCode)
        {
            if (string.IsNullOrEmpty(airportName) || string.IsNullOrEmpty(airportCity) ||
                string.IsNullOrEmpty(airportCountry) || string.IsNullOrEmpty(airportCode))
            {
                TempData["ErrorAddAirport"] = "Lütfen tüm alanları doldurun.";
                _logger.LogWarning("Havaalanı ekleme işlemi: Eksik bilgi ile gönderildi.");
                return RedirectToAction("Index");
            }

            try
            {
                // Stored procedure'ü çağır
                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_InsertAirport 
                @AirportName = {airportName}, 
                @AirportCity = {airportCity}, 
                @AirportCountry = {airportCountry}, 
                @AirportCode = {airportCode}");

                TempData["SuccessAddAirport"] = "Havaalanı başarıyla eklendi.";
                _logger.LogInformation("Havaalanı başarıyla eklendi: {AirportName} - {AirportCode}", airportName, airportCode);
            }
            catch (Exception ex)
            {
                TempData["ErrorAddAirport"] = $"Havaalanı eklenirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Havaalanı ekleme işlemi sırasında bir hata oluştu. Havaalanı: {AirportName}, Kod: {AirportCode}", airportName, airportCode);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteAirport(string airportCode)
        {
            if (string.IsNullOrEmpty(airportCode))
            {
                TempData["ErrorDeleteAirport"] = "Lütfen bir havaalanı kodu girin.";
                _logger.LogWarning("Havaalanı silme işlemi: Eksik AirportCode ile gönderildi.");
                return RedirectToAction("Index");
            }

            try
            {
                // Stored procedure'ü çağır
                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_DeleteAirport 
                @AirportCode = {airportCode}");

                TempData["SuccessDeleteAirport"] = "Havaalanı başarıyla silindi.";
                _logger.LogInformation("Havaalanı başarıyla silindi: {AirportCode}", airportCode);
            }
            catch (Exception ex)
            {
                TempData["ErrorDeleteAirport"] = $"Havaalanı silinirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Havaalanı silme işlemi sırasında bir hata oluştu. Kod: {AirportCode}", airportCode);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult AddStaff(
    int role,
    string firstName,
    string lastName,
    string passportNumber,
    string country,
    DateTime birthDate,
    string email,
    string phone,
    string password,
    string tcKimlik,
    string? middleName = null,
    string? address = null)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(country) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorAddStaff"] = "Lütfen tüm gerekli alanları doldurun.";
                return RedirectToAction("Index");
            }

            try
            {
                // Nullable değerler için doğru şekilde null gönderimi yapılır
                var middleNameParam = string.IsNullOrEmpty(middleName) ? null : middleName;
                var addressParam = string.IsNullOrEmpty(address) ? null : address;

                // Stored procedure çağrısı
                _context.Database.ExecuteSqlInterpolated($@"
        EXEC sp_InsertUser 
            @UserTypeID = {role},
            @UserName = {firstName},
            @UserMiddleName = {middleNameParam},
            @UserLastName = {lastName},
            @UserPassportNumber = {passportNumber},
            @UserNationality = {country},
            @UserDateOfBirth = {birthDate},
            @UserEmail = {email},
            @UserPhone = {phone},
            @UserTCKimlik = {tcKimlik},
            @UserPassword = {password},
            @UserAddress = {addressParam}
        ");

                TempData["SuccessAddStaff"] = "Yetkili başarıyla eklendi.";
                _logger.LogInformation("Yetkili başarıyla eklendi: {Email}", email);
            }
            catch (Exception ex)
            {
                TempData["ErrorAddStaff"] = $"Yetkili eklenirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Yetkili eklenirken hata oluştu.");
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteStaff(int userId)
        {
            if (userId <= 0)
            {
                TempData["ErrorDeleteStaff"] = "Geçerli bir Yetkili ID'si girin.";
                return RedirectToAction("Index");
            }

            try
            {
                // Stored procedure çağrısı
                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_DeleteUser @UserID = {userId}");

                TempData["SuccessDeleteStaff"] = "Yetkili başarıyla silindi.";
                _logger.LogInformation("Yetkili silindi: UserID {UserID}", userId);
            }
            catch (Exception ex)
            {
                TempData["ErrorDeleteStaff"] = $"Yetkili silinirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Yetkili silinirken hata oluştu. UserID: {UserID}", userId);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteFlight(int flightID)
        {
            try
            {
                _context.Database.ExecuteSqlInterpolated($"EXEC sp_DeleteFlight @FlightID = {flightID}");

                TempData["SuccessDeleteFlight"] = "Uçuş başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorDeleteFlight"] = $"Bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult DeleteAircraft(string aircraftName)
        {
            try
            {
                // Stored procedure'ü çalıştır
                _context.Database.ExecuteSqlInterpolated(
                    $"EXEC sp_DeleteAircraft @AircraftName={aircraftName}");
                TempData["SuccessDeleteAircraft"] = "Uçak başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorDeleteAircraft"] = $"Bir hata oluştu: {ex.Message}";

            }
            return View("Index");
        }
        [HttpPost]
        public IActionResult InsertFlight(
            string flightNumber,
            int aircraftID,
            int departureAirportID,
            int arrivalAirportID,
            DateTime flightDepartureDateTime,
            DateTime flightArrivalDateTime,
            int departureGateID,
            int arrivalGateID,
            string flightStatus,
            string flightAirlineCompany,
            int flightPriceID)
        {
            try
            {
                _context.Database.ExecuteSqlInterpolated($@"
                    EXEC sp_InsertFlight 
                        @FlighNumber={flightNumber}, 
                        @AircraftID={aircraftID},
                        @DepartureAirportID={departureAirportID},
                        @ArrivalAirportID={arrivalAirportID},
                        @FlightDepartureDateTime={flightDepartureDateTime},
                        @FlightArrivalDateTime={flightArrivalDateTime},
                        @DepartureGateID={departureGateID},
                        @ArrivalGateID={arrivalGateID},
                        @FlightStatus={flightStatus},
                        @FlightAirlineCompany={flightAirlineCompany},
                        @FlightPriceID={flightPriceID}
                ");

                TempData["SuccessAddFlight"] = "Uçuş başarıyla eklendi!";
                return RedirectToAction("Index"); // Index veya uçuş ekleme sayfasına döndür
            }
            catch (Exception ex)
            {
                TempData["ErrorAddFlight"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult UpdateAircraftByName(AdminAircraft model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.AircraftName))
                {
                    TempData["ErrorUpdateAircraft"] = "Uçak adı zorunludur.";
                    return RedirectToAction("Index");
                }

                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_UpdateAircraft
                @AircraftName = {model.AircraftName}, 
                @AircraftModel = {model.AircraftModel ?? string.Empty}, 
                @AircraftProductionDate = {model.ProductionDate ?? DateTime.Now}, 
                @AircraftSeatCapacity = {model.SeatCapacity ?? 0}");

                TempData["SuccessUpdateAircraft"] = "Uçak başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uçak güncelleme sırasında hata oluştu.");
                TempData["ErrorUpdateAircraft"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult UpdateFlight(AdminFlight model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.FlighNumber) || string.IsNullOrEmpty(model.AircraftName) ||
                    string.IsNullOrEmpty(model.DepartureAirportName) || string.IsNullOrEmpty(model.ArrivalAirportName))
                {
                    TempData["ErrorUpdateFlight"] = "Uçuş numarası, uçak adı ve havaalanı isimleri zorunludur.";
                    return RedirectToAction("Index");
                }

                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_UpdateFlight
                @FlighNumber = {model.FlighNumber},
                @AircraftName = {model.AircraftName},
                @DepartureAirportName = {model.DepartureAirportName},
                @ArrivalAirportName = {model.ArrivalAirportName},
                @FlightDelayTime = {model.FlightDelayTime}");

                TempData["SuccessUpdateFlight"] = "Uçuş başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uçuş güncelleme sırasında hata oluştu.");
                TempData["ErrorUpdateFlight"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult UpdateAirport(AdminAirport model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.AirportName))
                {
                    TempData["ErrorUpdateAirport"] = "Havaalanı adı zorunludur.";
                    return RedirectToAction("Index");
                }

                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_UpdateAirport 
                @AirportName = {model.AirportName},
                @City = {model.City},
                @Country = {model.Country},
                @AirportCode = {model.AirportCode}");

                TempData["SuccessUpdateAirport"] = "Havaalanı başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Havaalanı güncelleme sırasında hata oluştu.");
                TempData["ErrorUpdateAirport"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult UpdateUser(AdminUser model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserTCKimlik))
                {
                    TempData["ErrorUpdateUser"] = "TC Kimlik numarası zorunludur.";
                    return RedirectToAction("Index");
                }

                _context.Database.ExecuteSqlInterpolated($@"
            EXEC sp_UpdateUser
                @UserTCKimlik = {model.UserTCKimlik},
                @UserTypeID = {model.UserTypeID},
                @UserName = {model.UserName},
                @UserMiddleName = {model.UserMiddleName},
                @UserLastName = {model.UserLastName},
                @UserNationality = {model.UserNationality},
                @UserDateOfBirth = {model.UserDateOfBirth},
                @UserEmail = {model.UserEmail},
                @UserPassword = {model.UserPassword}");

                TempData["SuccessUpdateUser"] = "Kullanıcı başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı güncelleme sırasında hata oluştu.");
                TempData["ErrorUpdateUser"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index");
            }
        }


    }




}

