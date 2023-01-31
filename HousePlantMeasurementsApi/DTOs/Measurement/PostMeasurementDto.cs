using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Measurement
{
    public class PostMeasurementDto
    {
        [Required]
        public string DeviceUUID { get; set; }

        [Required]
        public string DeviceMacAddress { get; set; }

        [Required]
        public double Moisture { get; set; }
        [Required]
        public double Temperature { get; set; }
        [Required]
        public double LightIntensity { get; set; }
    }
}

