using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Plant
{
    public class PostPlantDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Species { get; set; }
        [Required]
        public int UserId { get; set; }

        public string? Description { get; set; }

        public double? MoistureLowLimit { get; set; }
        public double? MoistureHighLimit { get; set; }

        public double? TemperatureLowLimit { get; set; }
        public double? TemperatureHighLimit { get; set; }

        public double? LightIntensityLowLimit { get; set; }
        public double? LightIntensityHighLimit { get; set; }
    }
}

