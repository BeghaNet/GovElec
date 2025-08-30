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
    (string token,string refreshToken, DateTime expiresUtc) CreateToken(User user, TimeSpan? lifetime = null);
    ClaimsPrincipal? ValidateToken(string token);
}
public class TokenService(IConfiguration configuration) : ITokenService
{
    public (string token,string refreshToken, DateTime expiresUtc) CreateToken(User user, TimeSpan? lifetime = null)
    {
        if (user == null)
            return (new(string.Empty,string.Empty, DateTime.UtcNow));

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
	   return (accessToken,refreshToken, expires);
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

}
