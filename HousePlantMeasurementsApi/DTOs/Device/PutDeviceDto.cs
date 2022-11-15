using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Device
{
    public class PutDeviceDto
    {
        [Required]
        public int Id { get; set; }
        public string? UUID { get; set; }
        public bool? IsActive { get; set; }
        public int? UserId { get; set; }
        public int? PlantId { get; set; }
    }
}

