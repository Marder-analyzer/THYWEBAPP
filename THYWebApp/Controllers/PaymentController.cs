using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using THYWebApp.Data;
using THYWebApp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
namespace THYWebApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly THYContext _context;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(THYContext context, ILogger<PaymentController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: Payments/Create
        public IActionResult Create()
        {
            var selectedSeat = HttpContext.Session.GetString("SelectedSeat") ?? "Seçilmedi";
            var totalPrice = HttpContext.Session.GetString("TotalPrice") ?? "0.00";
            int flightId = HttpContext.Session.GetInt32("FlightId") ?? 0;
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? nonMemberCustomerId = HttpContext.Session.GetInt32("NonMemberCustomerId");
            ViewBag.SelectedSeat = selectedSeat;
            ViewBag.TotalPrice = totalPrice;
            ViewBag.FlightID = flightId;
            ViewBag.UserID = userId;
            ViewBag.NonMemberCustomerID = nonMemberCustomerId;
            
            return View();
        }

        [HttpPost]
        public IActionResult ProcessPayment(int FlightID, int? UserID,int? NonMemberCustomerID,string SeatNumber, string PaymentAmount, string cardHolder, string cardNumber, string expiryDate, string cvv, string billingAddress)
        {
            try
            {
                _logger.LogInformation("Ödeme işlemi başlatıldı: FlightID={FlightID}, SeatNumber={SeatNumber}", FlightID, SeatNumber);
                // UserID ve NonMemberCustomerID kontrolü
                if (UserID == null && NonMemberCustomerID == null)
                {
                    TempData["Error"] = "UserID veya NonMemberCustomerID belirtilmeli.";
                    return RedirectToAction("Create");
                }

                if (UserID != null && NonMemberCustomerID != null)
                {
                    TempData["Error"] = "Sadece birini belirtmelisiniz: UserID veya NonMemberCustomerID.";
                    return RedirectToAction("Create");
                }
                // TicketID için OUTPUT parametresi
                var ticketIDParam = new SqlParameter
                {
                    ParameterName = "@TicketID",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                // Yeni bir Ticket oluştur
                string ticketStatus = "Geçerli"; // Örnek durum

                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_InsertTicket @FlightID, @SeatNumber, @TicketStatus, @UserID, @NonMemberCustomerID, @TicketID OUTPUT",
                    new[]
                    {
                        new SqlParameter("@FlightID", FlightID),
                        new SqlParameter("@SeatNumber", SeatNumber),
                        new SqlParameter("@TicketStatus", ticketStatus),
                        new SqlParameter("@UserID", UserID ?? (object)DBNull.Value),
                        new SqlParameter("@NonMemberCustomerID", NonMemberCustomerID ?? (object)DBNull.Value),
                        ticketIDParam
                    }
                );

                if (ticketIDParam.Value == DBNull.Value)
                {
                    _logger.LogError("TicketID alınamadı. Stored procedure'de bir sorun olabilir.");
                    throw new Exception("TicketID alınamadı. Stored procedure'de bir sorun olabilir.");
                }

                int ticketID = (int)ticketIDParam.Value;

                _logger.LogInformation("Bilet oluşturuldu: TicketID={TicketID}", ticketID);

                // TicketID ile ödeme kaydı oluştur
                var paymentDate = DateTime.Now;
                var paymentMethod = "Kredi Kartı";
                var paymentStatus = "Tamamlandı";

                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_MakePayment @TicketID, @PaymentDate, @PaymentAmount, @PaymentMethod, @PaymentStatus",
                    new[]
                    {
                        new SqlParameter("@TicketID", ticketID),
                        new SqlParameter("@PaymentDate", paymentDate),
                        new SqlParameter("@PaymentAmount", decimal.Parse(PaymentAmount)),
                        new SqlParameter("@PaymentMethod", paymentMethod),
                        new SqlParameter("@PaymentStatus", paymentStatus)
                    }
                );

                _logger.LogInformation("Ödeme başarıyla tamamlandı: TicketID={TicketID}, PaymentAmount={PaymentAmount}", ticketID, PaymentAmount);

                TempData["Success"] = "Ödeme başarılı bir şekilde tamamlandı.";
                TempData["TicketID"] = ticketID;
                TempData["PaymentAmount"] = PaymentAmount;
                return RedirectToAction("Ticket", "Home", new { ticketId = ticketID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bir hata oluştu: {Message}", ex.Message);
                TempData["Error"] = "Bir hata oluştu: " + ex.Message;
                return RedirectToAction("Create");
            }
        }







    }
}
