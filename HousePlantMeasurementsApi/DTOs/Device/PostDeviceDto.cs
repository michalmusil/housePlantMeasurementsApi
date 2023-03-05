using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Device
{
    public class PostDeviceDto
    {
        [Required]
        [MinLength(30)]
        public string CommunicationIdentifier { get; set; }

        [Required]
        [MinLength(17)]
        [MaxLength(17)]
        public string MacAddress { get; set; }

        [Required]
        public bool IsActive { get; set; } = false;
        public int? UserId { get; set; }
        public int? PlantId { get; set; }
    }
}

