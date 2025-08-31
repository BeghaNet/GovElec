namespace GovElec.Shared;
public class UserForCreateCommand
{
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Equipe { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Default role
}
public class UserForUpdateCommand
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Equipe { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Default role
    public string OldPassword { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
}
public class ChangePasswordCommand
{
    public string UserName { get; set; } = string.Empty;
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
public class UserForDeleteCommand
{
    public Guid Id { get; set; }
    public bool Undelete { get; set; } = false; // Si vrai, on récupère le compte utilisateur, sinon on le marque comme supprimé (soft-delete)
}
public class UserForLoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class UserForReadQuery
{
    public Guid Id { get; set; }
}
public class UserForReadResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Equipe { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Default role
    public bool IsDeleted { get; set; } = false;
}
public class UserForListResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Equipe { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Default role
    public bool IsDeleted { get; set; } = false;
}
