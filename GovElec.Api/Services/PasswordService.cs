
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GovElec.Api.Services;

public interface IPasswordService
{
    Task CreateAsync(string username,string password);
    Task<bool> VerifyAsync(string username, string password);
}
public class PasswordService(AppDbContext context) : IPasswordService
{
    private const int SaltSize = 16;          // 128 bits
    private const int HashSize = 32;          // 256 bits
    private const int Iterations = 100_000;


    public async Task CreateAsync(string username, string password)
    {


        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("userName est requis.", nameof(username));
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("password est requis.", nameof(password));

        var user = await context.Users.SingleOrDefaultAsync(u => u.UserName.ToUpper() == username.ToUpper());
        if (user is null)
            throw new InvalidOperationException($"Utilisateur '{username}' introuvable.");

        // Génère un sel aléatoire sécurisé
        var salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        // Calcule le hash PBKDF2
        var hash = Pbkdf2(password, salt, Iterations, HashSize);

        // Stocke dans l'utilisateur
        user.PasswordSalt = salt;
        user.PasswordHash = hash;
        user.PasswordIterations = Iterations;
        user.PasswordHashSize = HashSize;

        await context.SaveChangesAsync();

    }

    public async Task<bool> VerifyAsync(string username,string password)
    {
        // Implementation for verifying a password
        if (string.IsNullOrWhiteSpace(username) || password is null)
            return false;

        var user = await context.Users.SingleOrDefaultAsync(u => u.UserName.ToUpper() == username.ToUpper());
        if (user is null || user.PasswordSalt.Length == 0 || user.PasswordHash.Length == 0)
            return false;

        // Utilise les paramètres stockés (permet d’évoluer les itérations/taille plus tard)
        var expectedSize = user.PasswordHashSize > 0 ? user.PasswordHashSize : HashSize;
        var iterations = user.PasswordIterations > 0 ? user.PasswordIterations : Iterations;

        var computed = Pbkdf2(password, user.PasswordSalt, iterations, expectedSize);

        // Comparaison en temps constant pour éviter les attaques de timing
        return CryptographicOperations.FixedTimeEquals(computed, user.PasswordHash);
    }
    private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int size)
        {
            // Rfc2898DeriveBytes.Pbkdf2 (API moderne) – disponible sur .NET 6+
            return Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: size
            );
        }
}
