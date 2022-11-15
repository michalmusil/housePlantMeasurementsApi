using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Device
{
    public class PostDeviceDto
    {
        [Required]
        [MinLength(30)]
        public string UUID { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public int? UserId { get; set; }
        public int? PlantId { get; set; }
    }
}

