using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Device
{
    public class PostAssignToPlantDto
    {
        [Required]
        public int DeviceId { get; set; }
        [Required]
        public int PlantId { get; set; }
    }
}

