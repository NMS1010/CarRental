using System.ComponentModel.DataAnnotations;

namespace CarRental.Entities
{
	public class BaseEntity
	{
		[Key]
		public long Id { get; set; }
	}
}