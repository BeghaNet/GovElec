using System.ComponentModel.DataAnnotations;

namespace GovElec.Api.Models;

public class RefreshToken
{
	[Key]
	[Required]
	[MaxLength(100)]
	public string Token { get; set; }=string.Empty;
	[Required]
	public string  Username { get; set; }=string.Empty;
	public DateTime ExpiresUtc { get; set; }
	public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
	public bool Enabled { get; set; }

}
