namespace GovElec.Shared;

public record LoginRequest(string Username,string Password);
public record LogoutRequest(string Token);
public record TokenResponse(string Token,string RefreshToken,DateTime ExpiresAtUtc);
