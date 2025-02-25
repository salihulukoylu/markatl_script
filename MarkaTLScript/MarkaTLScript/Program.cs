using Microsoft.EntityFrameworkCore;
using MarkaTLScript.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Dil ve kültür ayarlarý
var supportedCultures = new[] { new CultureInfo("tr-TR") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext'i kaydediyoruz. Baðlantý dizesini appsettings.json'dan alabiliriz.
builder.Services.AddDbContext<DbMarkatlScriptContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Varsayýlan rota
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// Özel rotalar
app.MapControllerRoute(
    name: "sistem-ayarlari",
    pattern: "SistemAyarlari",
    defaults: new { controller = "Settings", action = "SystemSettings" });

app.MapControllerRoute(
    name: "kullanici-ayarlari",
    pattern: "KullaniciAyarlari",
    defaults: new { controller = "Settings", action = "UserSettings" });

app.MapControllerRoute(
    name: "bildirim-ayarlari",
    pattern: "BildirimAyarlari",
    defaults: new { controller = "Settings", action = "NotificationSettings" });

app.Run();
