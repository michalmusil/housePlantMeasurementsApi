using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Measurement
{
    public class GetMeasurementTypeDto
    {
        [Required]
        public int Number { get; set; }
        [Required]
        public string Name { get; set; }
    }
}

