using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GovElec.Shared;
public class PasswordDto
{
	public string Username { get; set; } = string.Empty;
	public string OldPassword { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string ConfirmPassword { get; set; } = string.Empty;
}
