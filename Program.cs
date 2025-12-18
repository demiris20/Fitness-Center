using kampus_fit.Models;
using kampus_fit.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý (SQL Server)
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity Servislerinin Eklenmesi (Üyelik Sistemi)
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Þifre kurallarý (Ödev olduðu için test etmesi kolay olsun diye basitleþtirdik)
    options.Password.RequireDigit = false;           // Rakam zorunlu deðil
    options.Password.RequireLowercase = false;       // Küçük harf zorunlu deðil
    options.Password.RequireUppercase = false;       // Büyük harf zorunlu deðil
    options.Password.RequireNonAlphanumeric = false; // Özel karakter (@, #) zorunlu deðil
    options.Password.RequiredLength = 3;             // En az 3 karakter olsun yeter

    options.User.RequireUniqueEmail = true;          // Ayný e-posta ile tekrar kayýt olunamaz
})
.AddEntityFrameworkStores<GymDbContext>()
.AddDefaultTokenProviders();

// 3. MVC Servisleri
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. Hata Yönetimi ve Güvenlik
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 5. Yetkilendirme Sýrasý (Burasý Çok Önemli!)
app.UseAuthentication(); // ÖNCE: Kimlik Doðrulama (Giriþ yapýlmýþ mý?)
app.UseAuthorization();  // SONRA: Yetkilendirme (Buraya girmeye yetkisi var mý?)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- VERÝ TOHUMLAMA (SEED DATA) ---
// Uygulama her baþladýðýnda veritabanýný kontrol et, boþsa doldur.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Az önce oluþturduðumuz sýnýfý çaðýrýyoruz
        kampus_fit.Models.SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanýna örnek veri eklenirken bir hata oluþtu.");
    }
}
// ----------------------------------

app.Run(); // Bu satýr zaten vardý, onun üstüne ekle.

app.Run();