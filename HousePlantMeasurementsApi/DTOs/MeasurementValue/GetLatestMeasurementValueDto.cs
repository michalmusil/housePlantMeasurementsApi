using System;
using HousePlantMeasurementsApi.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.MeasurementValue
{
	public class GetLatestMeasurementValueDto
	{
        [Required]
        public MeasurementType Type { get; set; }
        [Required]
        public double Value { get; set; }
        [Required]
        public int MeasurementId { get; set; }
        [Required]
        public DateTime Taken { get; set; }
    }
}

