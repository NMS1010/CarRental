﻿using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.User
{
    public class ChangePasswordVM
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; }
    }
}