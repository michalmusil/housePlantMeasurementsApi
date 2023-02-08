using System;
using System.ComponentModel.DataAnnotations;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.MeasurementValueLimit;

namespace HousePlantMeasurementsApi.DTOs.Plant
{
    public class PutPlantDto
    {
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Species { get; set; }

        public string? Description { get; set; }

        public List<PutMeasurementValueLimitDto>? MeasurementValueLimits { get; set; }
    }
}

