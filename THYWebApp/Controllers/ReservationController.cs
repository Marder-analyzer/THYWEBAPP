using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THYWebApp.Data;
using THYWebApp.Models;
using Microsoft.Data.SqlClient;
using THYWebApp.Models.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace THYWebApp.Controllers
{
    public class ReservationController : Controller
    {
        private readonly THYContext _context;
        private readonly ILogger<ReservationController> _logger;
        public ReservationController(THYContext context, ILogger<ReservationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Create()
        {
            // Session'dan verileri çek
            var email = (HttpContext.Session.GetString("PassengerEmail") ?? HttpContext.Session.GetString("UserEmail"))?.Trim();
            var phone = (HttpContext.Session.GetString("PassengerPhoneNumber") ?? HttpContext.Session.GetString("UserPhone"))?.Trim();
            var packageName = (HttpContext.Session.GetString("PackageName") ?? "Bilinmiyor").Trim();
            var packagePrice = (HttpContext.Session.GetString("PackagePrice") ?? "Bilinmiyor").Trim();
            var baggageAllowance = HttpContext.Session.GetInt32("BaggageAllowance");
            var departureAirportName = (HttpContext.Session.GetString("DepartureAirportName") ?? "Bilinmiyor").Trim();
            var arrivalAirportName = (HttpContext.Session.GetString("ArrivalAirportName") ?? "Bilinmiyor").Trim();
            var departureDate = (HttpContext.Session.GetString("DepartureDate") ?? "Bilinmiyor").Trim();
            departureAirportName = departureAirportName.Replace("˜", "İzmir");
            arrivalAirportName = arrivalAirportName.Replace("", "");
            var userId = HttpContext.Session.GetInt32("UserId");
            // Null kontrolü
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(packageName) || string.IsNullOrEmpty(packagePrice) ||
                string.IsNullOrEmpty(departureAirportName) || string.IsNullOrEmpty(arrivalAirportName) ||
                string.IsNullOrEmpty(departureDate) )
            {
                TempData["Error"] = "Session bilgileri eksik. Lütfen tekrar deneyin.";
                return RedirectToAction("Index", "NonMemberCustomer");
            }
            // Paket fiyatını decimal'e çevir
            if (!decimal.TryParse(packagePrice, out var packagePriceDecimal))
            {
                TempData["Error"] = "Paket fiyatı geçersiz.";
                return RedirectToAction("Index", "NonMemberCustomer");
            }

            // İndirimli fiyatı hesapla
            decimal discountedPrice = packagePriceDecimal;
            if (userId.HasValue)
            {
                discountedPrice = await GetDiscountedPriceAsync(userId.Value, packagePriceDecimal);
                HttpContext.Session.SetString("PackagePrice", discountedPrice.ToString());
            }
            // ViewBag ile View'a gönder
            ViewBag.PassengerEmail = email;
            ViewBag.PassengerPhoneNumber = phone;
            ViewBag.PackageName = packageName;
            ViewBag.PackagePrice = packagePriceDecimal.ToString("C");
            ViewBag.DiscountedPrice = discountedPrice.ToString("C"); // İndirimli fiyat
            ViewBag.DepartureAirportName = departureAirportName;
            ViewBag.ArrivalAirportName = arrivalAirportName;
            ViewBag.BaggageAllowance = baggageAllowance;
            if (DateTime.TryParse(departureDate, out var parsedDate))
            {
                ViewBag.DepartureDate = parsedDate.ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                ViewBag.DepartureDate = "Geçersiz Tarih";
            }
            return View();
        }

        public async Task<decimal> GetDiscountedPriceAsync(int userId, decimal flightPrice)
        {
            decimal discountedPrice = flightPrice; // Varsayılan olarak orijinal fiyatı kullan

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        await connection.OpenAsync();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_GetMemberDiscount";
                        command.CommandType = CommandType.StoredProcedure;

                        // Parametreleri ekle
                        var userIdParam = command.CreateParameter();
                        userIdParam.ParameterName = "@UserID";
                        userIdParam.Value = userId;
                        userIdParam.DbType = DbType.Int32;
                        command.Parameters.Add(userIdParam);

                        var flightPriceParam = command.CreateParameter();
                        flightPriceParam.ParameterName = "@FlightPrice";
                        flightPriceParam.Value = flightPrice;
                        flightPriceParam.DbType = DbType.Decimal;
                        command.Parameters.Add(flightPriceParam);

                        // Çıktı parametresi
                        var discountParam = command.CreateParameter();
                        discountParam.ParameterName = "@DiscountPrice";
                        discountParam.DbType = DbType.Decimal;
                        discountParam.Direction = ParameterDirection.Output;
                        discountParam.Precision = 18;
                        discountParam.Scale = 2;
                        command.Parameters.Add(discountParam);

                        // Prosedürü çalıştır
                        await command.ExecuteNonQueryAsync();

                        // İndirimli fiyatı al
                        if (discountParam.Value != DBNull.Value)
                        {
                            discountedPrice = (decimal)discountParam.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Indirim hesaplama sırasında bir hata oluştu. UserID: {UserId}", userId);
            }

            return discountedPrice;
        }



        public async Task<IActionResult> CreateReservation()
        {
            try
            {
                var packageName = HttpContext.Session.GetString("PackageName");
                var FlyPacketsID = await _context.FlyPacket
    .FromSqlInterpolated($@"SELECT dbo.fn_GetFlyPacketIdByName({packageName}) AS FlyPacketsID")
    .Select(p => p.FlyPacketsID)
    .FirstOrDefaultAsync();
                
                var email = (HttpContext.Session.GetString("PassengerEmail"));
                // NonMemberCustomerID kontrolü
                int? NonMemberCustomerID = null;
                if (!string.IsNullOrEmpty(email))
                {
                    NonMemberCustomerID = await _context.NonMemberCustomer
    .FromSqlInterpolated($@"SELECT dbo.fn_GetNonMemberCustomerIdByEmail({email}) AS NonMemberCustomerID")
    .Select(n => n.NonMemberCustomerID)
    .FirstOrDefaultAsync();

                    HttpContext.Session.SetInt32("NonMemberCustomerId", (int)NonMemberCustomerID);
                }
                else
                {
                    NonMemberCustomerID = null;
                    email = HttpContext.Session.GetString("UserEmail");
                }
                var FlightID = HttpContext.Session.GetInt32("FlightId");
                int? UserID = HttpContext.Session.GetInt32("UserId");
                var reservationDate = DateTime.Now;
                var reservationStatus = "Onaylandı";
                Console.WriteLine($"noncustomerıd: {NonMemberCustomerID}");
                Console.WriteLine($"package name: {packageName}");
                Console.WriteLine($"flightid: {FlightID}");
                Console.WriteLine($"FlyPacketsID: {FlyPacketsID}");
                Console.WriteLine($"email: {email}");
                Console.WriteLine($"userid: {UserID}");
                Console.WriteLine($"reservationDate: {reservationDate}");
                Console.WriteLine($"reservationStatus: {reservationStatus}");
                if (UserID == null && NonMemberCustomerID == null)
                {
                    TempData["Error"] = "UserID veya NonMemberCustomerID belirtilmeli.";
                    return RedirectToAction("Create", "Reservation");
                }

                if (UserID != null && NonMemberCustomerID != null)
                {
                    TempData["Error"] = "Sadece birini belirtmelisiniz: UserID veya NonMemberCustomerID.";
                    return RedirectToAction("Create", "Reservation");
                }
                if (FlyPacketsID == 0 || NonMemberCustomerID == 0 || FlightID == null || string.IsNullOrEmpty(packageName) || string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "Gerekli bilgiler eksik. Lütfen tekrar deneyin.";
                    return RedirectToAction("Create");
                }

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC sp_InsertReservation
                @UserID = {UserID},
                @NonMemberCustomerID = {NonMemberCustomerID},
                @FlightID = {FlightID},
                @FlyPacketsID = {FlyPacketsID},
                @ReservationDate = {reservationDate},
                @ReservationStatus = {reservationStatus}");

                TempData["Success"] = "Rezervasyon başarıyla oluşturuldu.";
                return RedirectToAction("Select", "Seat");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rezervasyon oluşturulurken bir hata oluştu.");
                TempData["Error"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index", "Reservation");
            }
        }


        // GET: Reservations/Details/5
        /*public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FromSqlRaw("EXEC sp_GetReservationById @ReservationID", new SqlParameter("@ReservationID", id))
                .FirstOrDefaultAsync();

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }*/


        /*// POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationID,NonMemberCustomerID,UserID,FlightID,FlyPacketsID,ReservationDate,ReservationStatus")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_InsertReservation @NonMemberCustomerID, @UserID, @FlightID, @FlyPacketsID, @ReservationDate, @ReservationStatus",
                        new SqlParameter("@NonMemberCustomerID", (object)reservation.NonMemberCustomerID ?? DBNull.Value),
                        new SqlParameter("@UserID", (object)reservation.UserID ?? DBNull.Value),
                        new SqlParameter("@FlightID", (object)reservation.FlightID ?? DBNull.Value),
                        new SqlParameter("@FlyPacketsID", (object)reservation.FlyPacketsID ?? DBNull.Value),
                        new SqlParameter("@ReservationDate", reservation.ReservationDate),
                        new SqlParameter("@ReservationStatus", reservation.ReservationStatus)
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log hatası
                    return Content($"Hata: {ex.Message}");
                }
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FromSqlRaw("EXEC sp_GetReservationById @ReservationID", new SqlParameter("@ReservationID", id))
                .FirstOrDefaultAsync();

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationID,NonMemberCustomerID,UserID,FlightID,FlyPacketsID,ReservationDate,ReservationStatus")] Reservation reservation)
        {
            if (id != reservation.ReservationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_UpdateReservation @ReservationID, @NonMemberCustomerID, @UserID, @FlightID, @FlyPacketsID, @ReservationDate, @ReservationStatus",
                        new SqlParameter("@ReservationID", reservation.ReservationID),
                        new SqlParameter("@NonMemberCustomerID", (object)reservation.NonMemberCustomerID ?? DBNull.Value),
                        new SqlParameter("@UserID", (object)reservation.UserID ?? DBNull.Value),
                        new SqlParameter("@FlightID", (object)reservation.FlightID ?? DBNull.Value),
                        new SqlParameter("@FlyPacketsID", (object)reservation.FlyPacketsID ?? DBNull.Value),
                        new SqlParameter("@ReservationDate", reservation.ReservationDate),
                        new SqlParameter("@ReservationStatus", reservation.ReservationStatus)
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log hatası
                    return Content($"Hata: {ex.Message}");
                }
            }
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FromSqlRaw("EXEC sp_GetReservationById @ReservationID", new SqlParameter("@ReservationID", id))
                .FirstOrDefaultAsync();

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DeleteReservation @ReservationID",
                    new SqlParameter("@ReservationID", id)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log hatası
                return Content($"Hata: {ex.Message}");
            }
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.ReservationID == id);
        }*/
    }
}
