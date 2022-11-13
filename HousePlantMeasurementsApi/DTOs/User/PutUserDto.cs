using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.User
{
	public class PutUserDto
	{
        [Required]
        public int Id { get; set; }

        [EmailAddress]
        [MaxLength(500)]
        public string? Email { get; set; }

        [MinLength(7)]
        public string? Password { get; set; }
    }
}

