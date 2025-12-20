using kampus_fit.Models;
using kampus_fit.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity Ayarlarý
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Geliþtirme aþamasýnda þifre kurallarýný gevþettik (Kolay giriþ için)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<GymDbContext>()
.AddDefaultTokenProviders();

// 3. MVC Servisleri
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. Hata Yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 5. Yetkilendirme Zinciri
app.UseAuthentication(); // Kimsin?
app.UseAuthorization();  // Yetkin var mý?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- 6. VERÝ BAÞLATMA ---
// Bu kýsým veritabaný boþsa örnek verileri ekler.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // ÖNCEKÝ ADIMDA VERDÝÐÝM 'SeedData.cs' DOSYASINI BURADA ÇALIÞTIRIYORUZ
        kampus_fit.Models.SeedData.Initialize(services);

        // DÝKKAT: Eðer projenin 'Repo' klasöründe 'RoleSeeder.cs' yoksa alttaki satýr hata verir.
        // O yüzden þimdilik yorum satýrý yaptým. Proje çalýþýnca açabilirsin.
        // await kampus_fit.Repo.RoleSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabaný baþlatýlýrken hata oluþtu.");
    }
}
// ---------------------------------------------

app.Run();