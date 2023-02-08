using System;
using System.ComponentModel.DataAnnotations;
using HousePlantMeasurementsApi.Data.Enums;

namespace HousePlantMeasurementsApi.DTOs.MeasurementValue
{
    public class GetMeasurementValueDto
    {
        [Required]
        public MeasurementType Type { get; set; }
        [Required]
        public double Value { get; set; }
    }
}

