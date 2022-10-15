﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("Users")]
    public class User: BaseEntity
    {
        [Required]
        public string Email { get; set; }
        /*
        [Required]
        public string Password { get; set; }
        */

        List<Plant> Plants { get; set; }
        List<Device> Devices { get; set; }

    }
}

