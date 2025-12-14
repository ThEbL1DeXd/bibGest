using bibGest.Data; // <--- AJOUT 1 : Import du namespace de vos données
using Microsoft.EntityFrameworkCore; // <--- AJOUT 2 : Import pour SQL Server

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();