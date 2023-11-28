﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Entities
{
    public class FollowVehicle : BaseEntity
    {
        [ForeignKey("UserId")]
        public long? UserId { get; set; }

        [ForeignKey("PostVehicle")]
        public long? PostVehicleId { get; set; }

        public User User { get; set; }
        public PostVehicle PostVehicle { get; set; }

        public bool Status { get; set; }
    }
}