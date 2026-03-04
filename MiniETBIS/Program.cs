using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniETBIS.Data;
using MiniETBIS.Mappings;
using MiniETBIS.Models;
using MiniETBIS.Services;
using MiniETBIS.Validators;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/miniETBIS-.log", rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Render PostgreSQL URL formatini Npgsql formatina donustur
    // postgres://user:pass@host:port/dbname -> Host=...;Port=...;Database=...;Username=...;Password=...
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Veritabani baglanti dizesi bulunamadi.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCompanyValidator>();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await SeedDataAsync(services);
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Veritabani baslatma sirasinda kritik hata olustu");
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS redirect sadece development'ta - container ortaminda sertifika olmaz
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task SeedDataAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var context = services.GetRequiredService<AppDbContext>();

    // Rolleri olustur
    string[] roles = { "Admin", "Firma" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Admin kullaniciyi olustur
    const string adminEmail = "admin@miniETBIS.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Eger firma verisi varsa tekrar ekleme
    if (await context.Companies.AnyAsync())
        return;

    // 30 sahte firma verisi
    var firmaVerileri = new[]
    {
        ("Anadolu Elektronik A.S.", "1234567890", "Istanbul", "Elektronik"),
        ("Bosphorus Giyim Ltd.", "1234567891", "Istanbul", "Giyim & Moda"),
        ("Cappadocia Gida San.", "1234567892", "Kayseri", "Gida & Icecek"),
        ("Deniz Kitap Dagitim", "1234567893", "Izmir", "Kitap & Kirtasiye"),
        ("Efes Spor Malzemeleri", "1234567894", "Izmir", "Spor & Outdoor"),
        ("Fikir Yazilim Teknoloji", "1234567895", "Ankara", "Yazilim & Teknoloji"),
        ("Galata Kozmetik A.S.", "1234567896", "Istanbul", "Kozmetik & Bakim"),
        ("Hitit Otomotiv Parcalari", "1234567897", "Ankara", "Otomotiv"),
        ("Ilgaz Ev Dekorasyon", "1234567898", "Ankara", "Ev & Dekorasyon"),
        ("Jasmin Saglik Medikal", "1234567899", "Bursa", "Saglik & Medikal"),
        ("Karadeniz Gida Market", "2345678901", "Trabzon", "Gida & Icecek"),
        ("Lara Moda Tasarim", "2345678902", "Antalya", "Giyim & Moda"),
        ("Marmara Elektronik Paz.", "2345678903", "Kocaeli", "Elektronik"),
        ("Nemrut Oyuncak Dunyasi", "2345678904", "Gaziantep", "Oyuncak & Hobi"),
        ("Olimpos Spor Ekipmanlari", "2345678905", "Antalya", "Spor & Outdoor"),
        ("Pamukkale Tekstil San.", "2345678906", "Denizli", "Giyim & Moda"),
        ("Rumeli Bilisim Cozumleri", "2345678907", "Istanbul", "Yazilim & Teknoloji"),
        ("Selcuk Gida Urunleri", "2345678908", "Konya", "Gida & Icecek"),
        ("Truva Kitabevi", "2345678909", "Bursa", "Kitap & Kirtasiye"),
        ("Uludag Dogal Urunler", "3456789012", "Bursa", "Gida & Icecek"),
        ("Van Golu Su Urunleri", "3456789013", "Istanbul", "Gida & Icecek"),
        ("Yakamoz Kozmetik Ltd.", "3456789014", "Izmir", "Kozmetik & Bakim"),
        ("Zenit Teknoloji A.S.", "3456789015", "Ankara", "Elektronik"),
        ("Akdeniz Mobilya San.", "3456789016", "Mersin", "Ev & Dekorasyon"),
        ("Bogazici Saglik A.S.", "3456789017", "Istanbul", "Saglik & Medikal"),
        ("Cukurova Tarim Market", "3456789018", "Adana", "Gida & Icecek"),
        ("Doga Spor Yasam", "3456789019", "Mugla", "Spor & Outdoor"),
        ("Erciyes Oto Yedek Parca", "4567890123", "Kayseri", "Otomotiv"),
        ("Firat Oyuncak Sanayi", "4567890124", "Gaziantep", "Oyuncak & Hobi"),
        ("Gediz Dijital Cozumler", "4567890125", "Izmir", "Yazilim & Teknoloji"),
    };

    var random = new Random(42);

    string[] urunAdlari = {
        "Akilli Saat", "Bluetooth Kulaklik", "Tablet Kilifi", "USB Bellek", "Powerbank",
        "Spor Ayakkabi", "Kaban", "T-Shirt", "Kot Pantolon", "Elbise",
        "Zeytinyagi", "Bal", "Cay", "Kahve", "Kuruyemis",
        "Roman", "Defter", "Kalem Seti", "Ajanda", "Cocuk Kitabi",
        "Yoga Mati", "Dambil Seti", "Kosu Bandi", "Bisiklet Kasgi", "Cadir",
        "Laptop Standi", "Mekanik Klavye", "Monitor", "Yazici", "Web Kamera",
        "Yuz Kremi", "Sampuan", "Parfum", "Ruj", "Maskara",
        "Araba Parfumu", "Jant Kapagi", "Ayna", "Far Ampulu", "Silecek",
        "Masa Lambasi", "Perde", "Hali", "Vazo", "Cerceve",
        "Tansiyon Aleti", "Termometre", "Maske", "Vitamin", "Ilk Yardim Kiti",
        "Lego Seti", "Puzzle", "Tahta Oyuncak", "Uzaktan Kumandali Araba", "Bebek"
    };

    string[] sehirler = { "Istanbul", "Ankara", "Izmir", "Bursa", "Antalya", "Kocaeli", "Gaziantep", "Konya", "Adana", "Trabzon", "Kayseri", "Denizli", "Mersin", "Mugla", "Samsun" };

    var companies = new List<Company>();
    var products = new List<Product>();
    var sales = new List<Sale>();

    for (int i = 0; i < firmaVerileri.Length; i++)
    {
        var (name, tax, city, sector) = firmaVerileri[i];

        // Her firma icin kullanici olustur
        var email = $"firma{i + 1}@miniETBIS.com";
        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
        var createResult = await userManager.CreateAsync(user, "Firma123!");
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Firma");
        }

        var company = new Company
        {
            Name = name,
            TaxNumber = tax,
            City = city,
            Sector = sector,
            UserId = user.Id,
            CreatedDate = DateTime.UtcNow.AddDays(-random.Next(1, 365))
        };
        companies.Add(company);
    }

    context.Companies.AddRange(companies);
    await context.SaveChangesAsync();

    // Her firma icin 2-4 urun olustur
    foreach (var company in companies)
    {
        int urunSayisi = random.Next(2, 5);
        var usedIndices = new HashSet<int>();
        for (int j = 0; j < urunSayisi; j++)
        {
            int idx;
            do { idx = random.Next(urunAdlari.Length); } while (usedIndices.Contains(idx));
            usedIndices.Add(idx);

            var product = new Product
            {
                Name = urunAdlari[idx],
                Category = company.Sector,
                Price = Math.Round((decimal)(random.NextDouble() * 990 + 10), 2),
                CompanyId = company.Id
            };
            products.Add(product);
        }
    }

    context.Products.AddRange(products);
    await context.SaveChangesAsync();

    // Her urun icin 3-8 satis olustur
    foreach (var product in products)
    {
        int satisSayisi = random.Next(3, 9);
        for (int k = 0; k < satisSayisi; k++)
        {
            int qty = random.Next(1, 51);
            var sale = new Sale
            {
                ProductId = product.Id,
                Quantity = qty,
                TotalAmount = product.Price * qty,
                SaleDate = DateTime.UtcNow.AddDays(-random.Next(0, 180)),
                City = sehirler[random.Next(sehirler.Length)]
            };
            sales.Add(sale);
        }
    }

    context.Sales.AddRange(sales);
    await context.SaveChangesAsync();

    Log.Information("Seed verisi olusturuldu: {FirmaSayisi} firma, {UrunSayisi} urun, {SatisSayisi} satis",
        companies.Count, products.Count, sales.Count);
}
