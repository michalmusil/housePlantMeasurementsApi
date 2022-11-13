using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.User
{
    public class PostUserDto 
    {
        [Required]
        [EmailAddress]
        [MaxLength(500)]
        public string Email { get; set; }

        [Required]
        [MinLength(7)]
        public string Password { get; set; }

    }
}

