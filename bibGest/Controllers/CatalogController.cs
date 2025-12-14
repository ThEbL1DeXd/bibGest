using bibGest.Data;
using bibGest.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bibGest.Controllers;

public class CatalogController : Controller
{
    private readonly BibliothequeContext _context;

    public CatalogController(BibliothequeContext context)
    {
        _context = context;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? int.Parse(userIdClaim) : null;
    }

    // GET: /Catalog
    public async Task<IActionResult> Index(string? search, int? categoryId, int page = 1)
    {
        var pageSize = 12;
        var userId = GetCurrentUserId();

        var query = _context.Livres
            .Include(l => l.Categorie)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(l => 
                l.Titre.Contains(search) || 
                l.Auteur.Contains(search) ||
                l.Isbn.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(l => l.CategorieId == categoryId);
        }

        var totalBooks = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

        var books = await query
            .OrderBy(l => l.Titre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Get user's active reservations and loans
        var userReservations = userId.HasValue 
            ? await _context.Reservations
                .Where(r => r.UtilisateurId == userId && r.Statut == "EnAttente")
                .Select(r => r.LivreId)
                .ToListAsync()
            : new List<int>();

        var userLoans = userId.HasValue
            ? await _context.Emprunts
                .Where(e => e.UtilisateurId == userId && e.DateRetourReelle == null)
                .Select(e => e.LivreId)
                .ToListAsync()
            : new List<int>();

        var catalogItems = books.Select(b => new BookCatalogItem
        {
            Book = b,
            IsReservedByCurrentUser = userReservations.Contains(b.LivreId),
            IsBorrowedByCurrentUser = userLoans.Contains(b.LivreId)
        }).ToList();

        var viewModel = new CatalogViewModel
        {
            Books = catalogItems,
            Categories = await _context.Categories.OrderBy(c => c.NomCategorie).ToListAsync(),
            SearchTerm = search,
            CategoryId = categoryId,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize
        };

        return View(viewModel);
    }

    // GET: /Catalog/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();

        var book = await _context.Livres
            .Include(l => l.Categorie)
            .FirstOrDefaultAsync(l => l.LivreId == id);

        if (book == null)
        {
            return NotFound();
        }

        var isReserved = userId.HasValue && await _context.Reservations
            .AnyAsync(r => r.LivreId == id && r.UtilisateurId == userId && r.Statut == "EnAttente");

        var isBorrowed = userId.HasValue && await _context.Emprunts
            .AnyAsync(e => e.LivreId == id && e.UtilisateurId == userId && e.DateRetourReelle == null);

        var reservationCount = await _context.Reservations
            .CountAsync(r => r.LivreId == id && r.Statut == "EnAttente");

        var viewModel = new BookDetailsViewModel
        {
            Book = book,
            IsReservedByCurrentUser = isReserved,
            IsBorrowedByCurrentUser = isBorrowed,
            ReservationCount = reservationCount
        };

        return View(viewModel);
    }

    // POST: /Catalog/Reserve/5
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(int id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var book = await _context.Livres.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        // Check if already reserved
        var existingReservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.LivreId == id && r.UtilisateurId == userId && r.Statut == "EnAttente");

        if (existingReservation != null)
        {
            TempData["Error"] = "Vous avez déjà réservé ce livre.";
            return RedirectToAction("Details", new { id });
        }

        // Check if already borrowed
        var existingLoan = await _context.Emprunts
            .FirstOrDefaultAsync(e => e.LivreId == id && e.UtilisateurId == userId && e.DateRetourReelle == null);

        if (existingLoan != null)
        {
            TempData["Error"] = "Vous avez déjà emprunté ce livre.";
            return RedirectToAction("Details", new { id });
        }

        // Create reservation
        var reservation = new bibGest.Models.Reservation
        {
            LivreId = id,
            UtilisateurId = userId.Value,
            DateReservation = DateTime.Now,
            Statut = "EnAttente"
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Livre réservé avec succès ! Vous serez notifié quand il sera disponible.";
        return RedirectToAction("Details", new { id });
    }

    // GET: /Catalog/ByCategory/5
    public async Task<IActionResult> ByCategory(int id, int page = 1)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        ViewData["CategoryName"] = category.NomCategorie;
        return await Index(null, id, page);
    }

    // GET: /Catalog/Search
    public async Task<IActionResult> Search(string query)
    {
        return await Index(query, null, 1);
    }
}
