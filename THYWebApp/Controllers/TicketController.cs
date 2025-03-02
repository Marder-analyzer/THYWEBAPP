using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using THYWebApp.Data;
using THYWebApp.Models;
using THYWebApp.Services;

namespace THYWebApp.Controllers
{
    public class TicketController : Controller
    {
        private readonly THYContext _context;
        private readonly ILogger<TicketController> _logger;
        private readonly TicketService _ticketService;
        public TicketController(THYContext context, ILogger<TicketController> logger, TicketService ticketService)
        {
            _context = context;
            _logger = logger;
            _ticketService = ticketService;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int reservationID, string lastName)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByReservationIDAndLastNameAsync(reservationID, lastName);
                _logger.LogInformation($"Aranan bilet: ReservationID: {reservationID}, LastName: {lastName}");


                if (ticket == null)
                {
                    TempData["ErrorEditTicket"] = "Bilet bulunamadı. Lütfen bilgilerinizi kontrol edin.";
                    return RedirectToAction("Index", "Home"); // Kullanıcıyı hatayla Home/Index'e yönlendir
                }

                // Eğer bilet bulunursa `editTicket` View'ına yönlendir
                return View("~/Views/Home/editTicket.cshtml", ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bilet düzenleme işleminde bir hata oluştu.");
                TempData["ErrorEditTicket"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Ticket model)
        {
            _logger.LogInformation($"Gönderilen Model: TicketID={model.TicketID}, ReservationID={model.ReservationID}, Email={model.PassengerEmail}, Phone={model.PassengerPhoneNumber}");
            try
            {
                var isUpdated = await _ticketService.UpdateTicketAsync(model);

                if (isUpdated)
                {
                    _logger.LogInformation($"Bilet başarıyla güncellendi.");
                    return RedirectToAction("Edit", new { reservationID = model.ReservationID, lastName = model.LastName });
                }
                else
                {
                    TempData["Error"] = "Bilet güncellenemedi. Lütfen tekrar deneyin.";
                    return View("~/Views/Home/editTicket.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bilet güncelleme işleminde bir hata oluştu.");
                TempData["Error"] = "Bilet güncellenirken bir hata oluştu. Lütfen tekrar deneyin.";
                return View("~/Views/Home/editTicket.cshtml", model);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int ticketID)
        {
            _logger.LogInformation($"Silinecek TicketID: {ticketID}");
            try
            {
                var isDeleted = await _ticketService.DeleteTicketAsync(ticketID);

                if (isDeleted)
                {
                    TempData["Success"] = "Bilet başarıyla silindi.";

                }
                else
                {
                    TempData["Error"] = "Bilet silinemedi. Geçerli bir TicketID sağlayın.";
                }

                return RedirectToAction("Index", "Home"); // Silme sonrası yönlendirme
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bilet silme işlemi sırasında bir hata oluştu.");
                TempData["Error"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Index", "Home");
            }
        }


    }
}
