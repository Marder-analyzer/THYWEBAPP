using Microsoft.EntityFrameworkCore;
using THYWebApp.Data;
using THYWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<THYContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging() // Hassas veri loglama
           .LogTo(Console.WriteLine, LogLevel.Information)); // Konsola loglama

// Add services to the container
builder.Services.AddControllersWithViews(); // Controllers ve Views destekleniyor
// Session ayarlarını ekleyin
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true; // Güvenlik
    options.Cookie.IsEssential = true; // GDPR için gerekli
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Konsol loglama ekler
// TicketService servisini DI container'a ekleyin
builder.Services.AddScoped<TicketService>();
builder.Logging.AddDebug();   // Debug loglama ekler
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "THYWebApp v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Session middleware'i ekleyin
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
