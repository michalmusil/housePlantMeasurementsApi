using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HousePlantMeasurementsApi.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("Users")]
    [Index(nameof(Email), IsUnique=true)]
    public class User: BaseEntity
    {
        [Required]
        [EmailAddress]
        [MaxLength(500)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserRole Role { get; set; }

        public string? NotificationToken { get; set; }

        public List<Plant> Plants { get; set; }
        public List<Device> Devices { get; set; }

    }
}

