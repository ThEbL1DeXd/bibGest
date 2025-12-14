using bibGest.Data;
using bibGest.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ==================================================================
// <--- AJOUT 3 : CONFIGURATION DE LA BASE DE DONNEES
// ==================================================================
// On récupère la chaîne de connexion depuis appsettings.json
var connectionString = builder.Configuration.GetConnectionString("MaConnexionBibliotheque");

// On enregistre le service DbContext
builder.Services.AddDbContext<BibliothequeContext>(options =>
    options.UseSqlServer(connectionString));
// ==================================================================

// Authentication Configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrateur"));
    options.AddPolicy("BibliothecaireOrAdmin", policy => policy.RequireRole("Bibliothecaire", "Administrateur"));
    options.AddPolicy("MemberOnly", policy => policy.RequireRole("Membre"));
});

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();

// Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();