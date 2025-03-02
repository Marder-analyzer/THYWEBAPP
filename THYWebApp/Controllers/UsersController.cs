using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using THYWebApp.Data;
using THYWebApp.Models;
using Microsoft.Data.SqlClient;
public class UsersController : Controller
{
    private readonly THYContext _context;

    public UsersController(THYContext context)
    {
        _context = context;
    }

    // Kullanıcı Listele (sp_GetUsers)
    public async Task<IActionResult> Index()
    {
        // Session'dan UserID'yi al
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["Error"] = "Oturum bulunamadı. Lütfen giriş yapın.";
            return RedirectToAction("Index", "Home");
        }
        // Kullanıcı bilgilerini view'dan sorgula
        var user = await _context.Users
            .FromSqlInterpolated($"SELECT TOP 1 * FROM vw_UsersWithType WHERE UserID = {userId}")
            .FirstOrDefaultAsync();

        if (user == null)
        {
            TempData["Error"] = "Kullanıcı bilgileri bulunamadı.";
            return RedirectToAction("Index", "Home");
        }

        HttpContext.Session.SetString("UserEmail", user.UserEmail);
        HttpContext.Session.SetString("UserPhone", user.UserPhone);
        return View(user);

    }
    [HttpGet]
    public IActionResult Create()
    {
        // Boş bir Users nesnesi oluşturup view'e gönderiyoruz
        var model = new Users();
        return View(model);
    }
    [HttpPost]
    public IActionResult Create(Users model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorCreateUser"] = "Lütfen tüm alanları doğru doldurun.";
            return View(model);
        }

        try
        {
            // Kullanıcı tipini belirleyin, burada varsayılan olarak "1" kullanılıyor
            int userTypeId = 1;
            // sp_InsertUser prosedürünü çağırarak kullanıcıyı ekle
            _context.Database.ExecuteSqlInterpolated($@"
                EXEC sp_InsertUser 
                    @UserTypeID = {userTypeId},
                    @UserName = {model.UserName},
                    @UserMiddleName = {model.UserMiddleName},
                    @UserLastName = {model.UserLastName},
                    @UserPassportNumber = {model.UserPassportNumber},
                    @UserNationality = {model.UserNationality},
                    @UserDateOfBirth = {model.UserDateOfBirth},
                    @UserEmail = {model.UserEmail},
                    @UserPhone = {model.UserPhone},
                    @UserTCKimlik = {model.UserTCKimlik},
                    @UserPassword = {model.UserPassword},
                    @UserAddress = {model.UserAddress}
            ");

            TempData["SuccessCreateUser"] = "Üyelik başarıyla oluşturuldu.";
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            TempData["ErrorCreateUser"] = "Üyelik oluşturulurken bir hata oluştu: " + ex.Message;
            return View(model);
        }
    }



}
