using FrontOffice.Application.Common.Interfaces;

namespace FrontOffice.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    // Méthode pour hacher un mot de passe
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Méthode pour vérifier un mot de passe 
    public bool Verify(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}