namespace GovElec.Api.Models;


public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Equipe { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    public string Role { get; set; } ="User"; // Default role
    public bool IsDeleted { get; set; } = false;
    //Configuration pour le password
    public int PasswordIterations { get; set; }
    public int PasswordHashSize { get; set; }

    //Les liens
    public ICollection<Demande> DemandesEmises { get; set; } = new List<Demande>();
    public ICollection<Demande> DemandesAssignees { get; set; } = new List<Demande>();
}
