using System;
using System.ComponentModel.DataAnnotations;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.MeasurementValueLimit;

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

        public List<PostMeasurementValueLimitDto>? MeasurementValueLimits { get; set; }
    }
}

