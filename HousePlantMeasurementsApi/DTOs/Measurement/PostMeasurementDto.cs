using System;
using System.ComponentModel.DataAnnotations;
using HousePlantMeasurementsApi.Data.Entities;

namespace HousePlantMeasurementsApi.DTOs.Measurement
{
    public class PostMeasurementDto
    {
        [Required]
        public string DeviceUUID { get; set; }

        [Required]
        public string DeviceMacAddress { get; set; }

        [Required]
        public List<PostMeasurementDto> MeasurementValues { get; set; }
    }
}

