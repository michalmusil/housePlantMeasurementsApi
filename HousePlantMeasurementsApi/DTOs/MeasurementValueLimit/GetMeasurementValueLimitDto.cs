using System;
using HousePlantMeasurementsApi.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.MeasurementValueLimit
{
    public class GetMeasurementValueLimitDto
    {
        [Required]
        public MeasurementType Type { get; set; }

        [Required]
        public double LowerLimit { get; set; }

        [Required]
        public double UpperLimit { get; set; }
    }
}

