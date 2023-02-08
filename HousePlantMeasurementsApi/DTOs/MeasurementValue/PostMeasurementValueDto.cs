using System;
using HousePlantMeasurementsApi.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.MeasurementValue
{
    public class PostMeasurementValueDto
    {
        [Required]
        public MeasurementType Type { get; set; }
        [Required]
        public double Value { get; set; }
    }
}

