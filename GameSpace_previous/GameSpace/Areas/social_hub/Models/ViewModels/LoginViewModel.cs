using System.ComponentModel.DataAnnotations;

namespace GameSpace.Areas.social_hub.Models
{
	public class LoginViewModel
	{
		[Required, Display(Name = "Account")]
		public string Account { get; set; } = string.Empty;

		[Required, DataType(DataType.Password), Display(Name = "Password")]
		public string Password { get; set; } = string.Empty;

		[Display(Name = "Remember Me")]
		public bool RememberMe { get; set; }

		public string? ReturnUrl { get; set; }
	}
}
