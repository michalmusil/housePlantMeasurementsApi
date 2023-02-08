using System;
using System.ComponentModel.DataAnnotations;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.MeasurementValueLimit;

namespace HousePlantMeasurementsApi.DTOs.Plant
{
    public class GetPlantDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string? Name { get; set; }
        public string? Species { get; set; }
        public string? Description { get; set; }
        public string? ImageName { get; set; }

        public List<GetMeasurementValueLimitDto> MeasurementValueLimits { get; set; }

    }
}

