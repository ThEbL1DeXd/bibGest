using bibGest.Data;
using bibGest.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bibGest.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly BibliothequeContext _context;

    public DashboardController(BibliothequeContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "Membre";
    }

    // GET: /Dashboard
    public async Task<IActionResult> Index()
    {
        var role = GetCurrentUserRole();

        return role switch
        {
            "Administrateur" => RedirectToAction("Admin"),
            "Bibliothecaire" => RedirectToAction("Librarian"),
            _ => RedirectToAction("Member")
        };
    }

    // GET: /Dashboard/Member - User Dashboard
    public async Task<IActionResult> Member()
    {
        var userId = GetCurrentUserId();
        var user = await _context.Utilisateurs.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var today = DateTime.Now;

        var viewModel = new UserDashboardViewModel
        {
            User = user,
            
            // Current loans (not returned)
            CurrentLoans = await _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.UtilisateurId == userId && e.DateRetourReelle == null)
                .OrderByDescending(e => e.DateEmprunt)
                .ToListAsync(),
            
            // Overdue loans
            OverdueLoans = await _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.UtilisateurId == userId 
                    && e.DateRetourReelle == null 
                    && e.DateRetourPrevue < today)
                .ToListAsync(),
            
            // Reading history (returned books)
            ReadingHistory = await _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.UtilisateurId == userId && e.DateRetourReelle != null)
                .OrderByDescending(e => e.DateRetourReelle)
                .Take(10)
                .ToListAsync(),
            
            // Active reservations
            ActiveReservations = await _context.Reservations
                .Include(r => r.Livre)
                .Where(r => r.UtilisateurId == userId && r.Statut == "EnAttente")
                .OrderByDescending(r => r.DateReservation)
                .ToListAsync(),
            
            // Stats
            TotalBooksRead = await _context.Emprunts
                .CountAsync(e => e.UtilisateurId == userId && e.DateRetourReelle != null),
            CurrentLoansCount = await _context.Emprunts
                .CountAsync(e => e.UtilisateurId == userId && e.DateRetourReelle == null),
            ReservationsCount = await _context.Reservations
                .CountAsync(r => r.UtilisateurId == userId && r.Statut == "EnAttente")
        };

        return View(viewModel);
    }

    // GET: /Dashboard/Librarian - Librarian Dashboard
    [Authorize(Policy = "BibliothecaireOrAdmin")]
    public async Task<IActionResult> Librarian()
    {
        var today = DateTime.Now;

        var viewModel = new LibrarianDashboardViewModel
        {
            TotalBooks = await _context.Livres.SumAsync(l => l.QuantiteTotale),
            TotalMembers = await _context.Utilisateurs.CountAsync(u => u.Role == "Membre"),
            ActiveLoans = await _context.Emprunts.CountAsync(e => e.DateRetourReelle == null),
            OverdueLoans = await _context.Emprunts.CountAsync(e => e.DateRetourReelle == null && e.DateRetourPrevue < today),
            PendingReservations = await _context.Reservations.CountAsync(r => r.Statut == "EnAttente"),
            
            RecentLoans = await _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Utilisateur)
                .OrderByDescending(e => e.DateEmprunt)
                .Take(10)
                .ToListAsync(),
            
            OverdueLoansList = await _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Utilisateur)
                .Where(e => e.DateRetourReelle == null && e.DateRetourPrevue < today)
                .OrderBy(e => e.DateRetourPrevue)
                .ToListAsync(),
            
            PendingReservationsList = await _context.Reservations
                .Include(r => r.Livre)
                .Include(r => r.Utilisateur)
                .Where(r => r.Statut == "EnAttente")
                .OrderBy(r => r.DateReservation)
                .ToListAsync()
        };

        return View(viewModel);
    }

    // GET: /Dashboard/Admin - Admin Dashboard
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Admin()
    {
        var today = DateTime.Now;

        var viewModel = new AdminDashboardViewModel
        {
            TotalBooks = await _context.Livres.SumAsync(l => l.QuantiteTotale),
            TotalUsers = await _context.Utilisateurs.CountAsync(),
            TotalLoans = await _context.Emprunts.CountAsync(),
            ActiveLoans = await _context.Emprunts.CountAsync(e => e.DateRetourReelle == null),
            OverdueLoans = await _context.Emprunts.CountAsync(e => e.DateRetourReelle == null && e.DateRetourPrevue < today),
            TotalReservations = await _context.Reservations.CountAsync(),
            TotalPenalties = await _context.Penalites.CountAsync(),
            TotalPenaltyAmount = await _context.Penalites.SumAsync(p => p.Montant),
            UnpaidPenaltyAmount = await _context.Penalites.Where(p => !p.EstPayee).SumAsync(p => p.Montant),
            
            CategoryStats = await _context.Categories
                .Select(c => new CategoryStats
                {
                    CategoryName = c.NomCategorie,
                    BookCount = c.Livres.Sum(l => l.QuantiteTotale),
                    LoanCount = c.Livres.SelectMany(l => l.Emprunts).Count()
                })
                .ToListAsync()
        };

        return View(viewModel);
    }

    // GET: /Dashboard/MyLoans - Current Loans
    public async Task<IActionResult> MyLoans()
    {
        var userId = GetCurrentUserId();

        var loans = await _context.Emprunts
            .Include(e => e.Livre)
            .Where(e => e.UtilisateurId == userId && e.DateRetourReelle == null)
            .OrderByDescending(e => e.DateEmprunt)
            .ToListAsync();

        return View(loans);
    }

    // GET: /Dashboard/ReadingHistory - Reading History
    public async Task<IActionResult> ReadingHistory()
    {
        var userId = GetCurrentUserId();

        var history = await _context.Emprunts
            .Include(e => e.Livre)
            .Where(e => e.UtilisateurId == userId && e.DateRetourReelle != null)
            .OrderByDescending(e => e.DateRetourReelle)
            .ToListAsync();

        return View(history);
    }

    // GET: /Dashboard/MyReservations - User's Reservations
    public async Task<IActionResult> MyReservations()
    {
        var userId = GetCurrentUserId();

        var reservations = await _context.Reservations
            .Include(r => r.Livre)
            .Where(r => r.UtilisateurId == userId)
            .OrderByDescending(r => r.DateReservation)
            .ToListAsync();

        return View(reservations);
    }

    // POST: /Dashboard/CancelReservation - Cancel a reservation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var userId = GetCurrentUserId();
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.ReservationId == id && r.UtilisateurId == userId);

        if (reservation == null)
        {
            return NotFound();
        }

        reservation.Statut = "Annulee";
        await _context.SaveChangesAsync();

        TempData["Success"] = "Réservation annulée avec succès.";
        return RedirectToAction("MyReservations");
    }
}
