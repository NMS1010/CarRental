﻿using CarRental.Common.Enums;

namespace CarRental.Entities
{
	public class ApprovalApplication : BaseEntity
	{
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string Identity { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public long UserId { get; set; }
		public bool IsApprove { get; set; } = false;
		public APPLICATION_TYPE Type { get; set; }
		public User User { get; set; }
	}
}