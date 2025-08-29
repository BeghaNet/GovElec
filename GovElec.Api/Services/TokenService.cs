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
}
public interface ITokenService
{
    (string token, DateTime expiresUtc) CreateToken(User user, TimeSpan? lifetime = null);
    ClaimsPrincipal? ValidateToken(string token);
}
public class TokenService(IConfiguration configuration) : ITokenService
{
    // private readonly TokenOptions _options;
    // private readonly byte[] _signInKey;
    // public TokenService(IOptions<TokenOptions> options)
    // {
    //     _options = options.Value;
    //     _signInKey = Encoding.UTF8.GetBytes(_options.SignInKey);
    // }

    public (string token, DateTime expiresUtc) CreateToken(User user, TimeSpan? lifetime = null)
    {
        if (user == null)
            return (new(string.Empty, DateTime.UtcNow));

        var now = DateTime.UtcNow;
        var expires = now.Add(lifetime ?? TimeSpan.FromHours(2));
        var signInKey = configuration["Jwt:SigningKey"!];
        byte[] bytes = Encoding.UTF8.GetBytes(signInKey!);
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.GivenName,$"{user!.FullName}"),
            new Claim("Role", $"{user.Role}"),
            new Claim("Team",$"{user.Equipe}"),
            new Claim(JwtRegisteredClaimNames.Email,user.Email),
            new Claim(JwtRegisteredClaimNames.PhoneNumber,user.Phone), // ex: "read:weather write:weather"
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(bytes), SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expires);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var signInKey = configuration["Jwt:SigningKey"!];
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
