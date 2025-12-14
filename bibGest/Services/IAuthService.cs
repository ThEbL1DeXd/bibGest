using bibGest.Models;

namespace bibGest.Services;

public interface IAuthService
{
    Task<Utilisateur?> AuthenticateAsync(string email, string password);
    Task<Utilisateur?> RegisterAsync(string nom, string prenom, string email, string password, string? telephone);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    Task<Utilisateur?> GetUserByIdAsync(int userId);
    Task<Utilisateur?> GetUserByEmailAsync(string email);
}
