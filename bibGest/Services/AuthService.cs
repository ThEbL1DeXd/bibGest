using bibGest.Data;
using bibGest.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace bibGest.Services;

public class AuthService : IAuthService
{
    private readonly BibliothequeContext _context;

    public AuthService(BibliothequeContext context)
    {
        _context = context;
    }

    public async Task<Utilisateur?> AuthenticateAsync(string email, string password)
    {
        var user = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == email && u.EstActif);

        if (user == null)
            return null;

        if (!VerifyPassword(password, user.MotDePasseHash))
            return null;

        return user;
    }

    public async Task<Utilisateur?> RegisterAsync(string nom, string prenom, string email, string password, string? telephone)
    {
        // Check if email already exists
        var existingUser = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser != null)
            return null;

        var user = new Utilisateur
        {
            Nom = nom,
            Prenom = prenom,
            Email = email,
            MotDePasseHash = HashPassword(password),
            Role = "Membre",
            Telephone = telephone,
            DateInscription = DateTime.Now,
            EstActif = true
        };

        _context.Utilisateurs.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // First try to match hashed password
        var hashOfInput = HashPassword(password);
        if (hashOfInput == hashedPassword)
            return true;
        
        // Fallback: check if password matches plain text (for legacy data)
        // WARNING: Remove this in production!
        if (password == hashedPassword)
            return true;
        
        return false;
    }

    public async Task<Utilisateur?> GetUserByIdAsync(int userId)
    {
        return await _context.Utilisateurs.FindAsync(userId);
    }

    public async Task<Utilisateur?> GetUserByEmailAsync(string email)
    {
        return await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);
    }
}
