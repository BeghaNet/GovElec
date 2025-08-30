using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.IdentityModel.Tokens;
using Microsoft.CodeAnalysis.Options;

namespace GovElec.Api.Services;

public class TokenOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SignInKey { get; set; } = string.Empty;
    public bool SignInKeyIsBase64 { get; set; }
}
public interface ITokenService
{
    Task<(string token,string refreshToken, DateTime expiresUtc)> CreateTokenAsync(string username, TimeSpan? lifetime = null);
	Task<bool> IsRefreshTokenValidAsync(string refreshToken);
     Task RemoveTokensForUser(string username);
     Task DisableRefreshTokenAsync(string refreshToken);
     Task<bool> IsValidTokenIsValid(string token);
     Task<string> GetUserFromRefreshTokenAsync(string refreshToken);
	ClaimsPrincipal? ValidateToken(string token);
}
public class TokenService(AppDbContext context, IConfiguration configuration) : ITokenService
{
    public async Task< (string token,string refreshToken, DateTime expiresUtc)> CreateTokenAsync(string username, TimeSpan? lifetime = null)
    {
          var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
          if (user == null)
            return (new("Erreur",string.Empty, DateTime.UtcNow));

        var now = DateTime.UtcNow;
        var expires = now.Add(lifetime ?? TimeSpan.FromHours(2));

        var secretInKey = configuration["Jwt:SigninKey"!];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretInKey!));
	   var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
	   var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.GivenName,$"{user!.FullName}"),
            new Claim(ClaimTypes.Role, $"{user.Role}"),
            new Claim("Team",$"{user.Equipe}"),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim("Phone",user.Phone), // ex: "read:weather write:weather"
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        
        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
	   var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
		var refreshTokenEntity = new Models.RefreshToken
		{
			Token = refreshToken,
			Username = user.UserName,
			ExpiresUtc = now.AddDays(7),
			Enabled = true
		};
          await RemoveTokensForUser(username);
		context.RefreshTokens.Add(refreshTokenEntity);
		await context.SaveChangesAsync();

		return (accessToken,refreshToken, expires);
    }
     public async Task RemoveTokensForUser(string username)
     {
		if (await UserExists(username))
		{
			var existingTokens = context.RefreshTokens.Where(t => t.Username == username && t.Enabled);
			context.RefreshTokens.RemoveRange(existingTokens);
		}
	}
     public async Task DisableRefreshTokenAsync(string refreshToken)
     {
		var token = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken && t.Enabled);
		if (token != null)
		{
			token.Enabled = false;
			await context.SaveChangesAsync();
		}
	}
     public async Task<bool> IsValidTokenIsValid(string token)
     {
          var result=await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token && t.Enabled);
		if (result == null || result.ExpiresUtc < DateTime.UtcNow|| result.Enabled==false)
			return false;
		return true;
	}
     public async Task<string> GetUserFromRefreshTokenAsync(string refreshToken)
     {
		var token = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken && t.Enabled);
          return token?.Username ?? string.Empty;
	}
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var signInKey = configuration["Jwt:SigninKey"!];
        byte[] bytes = Encoding.UTF8.GetBytes(signInKey!);
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(bytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5)
        };
        
        try
        {
            return handler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }

     public async Task<bool> IsRefreshTokenValidAsync(string refreshToken)
     {
		var token = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken && t.Enabled);
		if (token == null || token.ExpiresUtc < DateTime.UtcNow)
			return false;
		return true;
	}

     private Task<bool> UserExists(string username) =>
	   context.RefreshTokens.AnyAsync(u => u.Username == username);

}
