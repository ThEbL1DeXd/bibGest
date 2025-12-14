using bibGest.Models;

namespace bibGest.ViewModels;

public class UserDashboardViewModel
{
    public Utilisateur User { get; set; } = null!;
    public List<Emprunt> CurrentLoans { get; set; } = new();
    public List<Emprunt> OverdueLoans { get; set; } = new();
    public List<Emprunt> ReadingHistory { get; set; } = new();
    public List<Reservation> ActiveReservations { get; set; } = new();
    public int TotalBooksRead { get; set; }
    public int CurrentLoansCount { get; set; }
    public int ReservationsCount { get; set; }
}

public class CatalogViewModel
{
    public List<BookCatalogItem> Books { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 12;
}

public class BookCatalogItem
{
    public Livre Book { get; set; } = null!;
    public bool IsAvailable => Book.QuantiteDisponible > 0;
    public bool IsReservedByCurrentUser { get; set; }
    public bool IsBorrowedByCurrentUser { get; set; }
    public int WaitlistPosition { get; set; }
}

public class BookDetailsViewModel
{
    public Livre Book { get; set; } = null!;
    public bool IsAvailable => Book.QuantiteDisponible > 0;
    public bool IsReservedByCurrentUser { get; set; }
    public bool IsBorrowedByCurrentUser { get; set; }
    public int ReservationCount { get; set; }
    public List<Emprunt> RecentLoans { get; set; } = new();
}

public class LibrarianDashboardViewModel
{
    public int TotalBooks { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public int PendingReservations { get; set; }
    public List<Emprunt> RecentLoans { get; set; } = new();
    public List<Emprunt> OverdueLoansList { get; set; } = new();
    public List<Reservation> PendingReservationsList { get; set; } = new();
}

public class AdminDashboardViewModel
{
    public int TotalBooks { get; set; }
    public int TotalUsers { get; set; }
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public int TotalReservations { get; set; }
    public int TotalPenalties { get; set; }
    public decimal TotalPenaltyAmount { get; set; }
    public decimal UnpaidPenaltyAmount { get; set; }
    public List<CategoryStats> CategoryStats { get; set; } = new();
    public List<MonthlyStats> MonthlyStats { get; set; } = new();
}

public class CategoryStats
{
    public string CategoryName { get; set; } = null!;
    public int BookCount { get; set; }
    public int LoanCount { get; set; }
}

public class MonthlyStats
{
    public string Month { get; set; } = null!;
    public int Loans { get; set; }
    public int Returns { get; set; }
}
