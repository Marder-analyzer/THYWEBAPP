using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using THYWebApp.Data;
using THYWebApp.Models;

namespace THYWebApp.Controllers
{
    public class NonMemberCustomerController : Controller
    {
        private readonly THYContext _context;

        public NonMemberCustomerController(THYContext context)
        {
            _context = context;
        }

        // GET: NonMemberCustomers
        public async Task<IActionResult> Index()
        {
            return View(await _context.NonMemberCustomer.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> SubmitNonMemberCustomer(
    string PassengerFirstName,
    string PassengerMiddleName, // Opsiyonel
    string PassengerLastName,
    string PassengerEmail,
    string PassengerPhoneNumber,
    string PassengerNationality,
    DateTime PassengerDateOfBirth,
    string PassengerTC,
    string PassengerGender)
        {
            if (string.IsNullOrWhiteSpace(PassengerFirstName) ||
                string.IsNullOrWhiteSpace(PassengerLastName) ||
                string.IsNullOrWhiteSpace(PassengerEmail) ||
                string.IsNullOrWhiteSpace(PassengerPhoneNumber) ||
                string.IsNullOrWhiteSpace(PassengerNationality) ||
                PassengerDateOfBirth == default ||
                string.IsNullOrWhiteSpace(PassengerGender))
            {
                TempData["Error"] = "Lütfen tüm alanları doldurun.";
                return RedirectToAction("Create", "NonMemberCustomer");
            }
            // İkinci ad boş ise null yap
            PassengerMiddleName = string.IsNullOrWhiteSpace(PassengerMiddleName) ? null : PassengerMiddleName;
            PassengerTC = string.IsNullOrWhiteSpace(PassengerTC) ? null : PassengerTC;
            try
            {
                // Veritabanına bağlan ve veri ekle
                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_InsertNonMemberCustomer", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@PassengerFirstName", PassengerFirstName);
                        command.Parameters.AddWithValue("@PassengerMiddleName", (object)PassengerMiddleName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PassengerLastName", PassengerLastName);
                        command.Parameters.AddWithValue("@PassengerEmail", PassengerEmail);
                        command.Parameters.AddWithValue("@PassengerPhoneNumber", PassengerPhoneNumber);
                        command.Parameters.AddWithValue("@PassengerNationality", PassengerNationality);
                        command.Parameters.AddWithValue("@PassengerDateOfBirth", PassengerDateOfBirth);
                        command.Parameters.AddWithValue("@PassengerTCKimlik", (object)PassengerTC ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PassengerGender", PassengerGender);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                // E-posta ve telefon bilgisini session'a kaydet
                if (string.IsNullOrEmpty(PassengerEmail) || string.IsNullOrEmpty(PassengerPhoneNumber))
                {
                    TempData["Error"] = "E-posta veya telefon numarası eksik.";
                    return RedirectToAction("Create", "NonMemberCustomer");
                }
                
                TempData["Success"] = "Kayıt başarıyla tamamlandı.";
                HttpContext.Session.SetString("PassengerName", PassengerFirstName+PassengerMiddleName+PassengerLastName);
                HttpContext.Session.SetString("PassengerEmail", PassengerEmail);
                HttpContext.Session.SetString("PassengerPhoneNumber", PassengerPhoneNumber);
                return RedirectToAction("Create", "Reservation");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "NonMemberCustomer");
            }
        }


        // GET: NonMemberCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nonMemberCustomer = await _context.NonMemberCustomer
                .FirstOrDefaultAsync(m => m.NonMemberCustomerID == id);
            if (nonMemberCustomer == null)
            {
                return NotFound();
            }

            return View(nonMemberCustomer);
        }

        // GET: NonMemberCustomers/Create
        public IActionResult Create()
        {
            var selectedFlightId = HttpContext.Session.GetInt32("FlightId");
            var selectedPackageName = HttpContext.Session.GetString("PackageName");
            var selectedPackagePrice = HttpContext.Session.GetString("PackagePrice");
            var selectedBaggageAllowance = HttpContext.Session.GetInt32("BaggageAllowance");

            Console.WriteLine($"FlightId: {selectedFlightId}, PackageName: {selectedPackageName}, PackagePrice: {selectedPackagePrice},BaggageAllowance: {selectedBaggageAllowance}");

            ViewBag.FlightId = selectedFlightId;
            ViewBag.BaggageAllowance = selectedBaggageAllowance;
            ViewBag.PackageName = selectedPackageName;
            ViewBag.PackagePrice = selectedPackagePrice;

            return View();
        }

        // POST: NonMemberCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // GET: NonMemberCustomers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nonMemberCustomer = await _context.NonMemberCustomer.FindAsync(id);
            if (nonMemberCustomer == null)
            {
                return NotFound();
            }
            return View(nonMemberCustomer);
        }

        // POST: NonMemberCustomers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NonMemberCustomerID,PassengerFirstName,PassengerMiddleName,PassengerLastName,PassengerTCKimlik,PassengerPassportNumber,PassengerEmail,PassengerPhoneNumber,PassengerNationality,PassengerDateOfBirth,PassengerGender")] NonMemberCustomer nonMemberCustomer)
        {
            if (id != nonMemberCustomer.NonMemberCustomerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nonMemberCustomer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NonMemberCustomerExists(nonMemberCustomer.NonMemberCustomerID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nonMemberCustomer);
        }

        // GET: NonMemberCustomers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nonMemberCustomer = await _context.NonMemberCustomer
                .FirstOrDefaultAsync(m => m.NonMemberCustomerID == id);
            if (nonMemberCustomer == null)
            {
                return NotFound();
            }

            return View(nonMemberCustomer);
        }

        // POST: NonMemberCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nonMemberCustomer = await _context.NonMemberCustomer.FindAsync(id);
            if (nonMemberCustomer != null)
            {
                _context.NonMemberCustomer.Remove(nonMemberCustomer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NonMemberCustomerExists(int id)
        {
            return _context.NonMemberCustomer.Any(e => e.NonMemberCustomerID == id);
        }
    }
}