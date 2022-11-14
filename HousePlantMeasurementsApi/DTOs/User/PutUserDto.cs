using System;
using System.ComponentModel.DataAnnotations;
using HousePlantMeasurementsApi.Data.Enums;

namespace HousePlantMeasurementsApi.DTOs.User
{
	public class PutUserDto
	{
        [Required]
        public int Id { get; set; }

        [EmailAddress]
        [MaxLength(500)]
        public string? Email { get; set; }

        public UserRole? Role { get; set; }

        [MinLength(7)]
        public string? Password { get; set; }
    }
}

