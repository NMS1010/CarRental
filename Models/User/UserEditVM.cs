using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.User
{
	public class UserEditVM
	{
		[Required]
		public long Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Phone { get; set; }

		[Required]
		public string Address { get; set; }

		public IFormFile Image { get; set; }
	}
}