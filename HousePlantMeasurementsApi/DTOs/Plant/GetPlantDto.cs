using System;
using System.ComponentModel.DataAnnotations;
using HousePlantMeasurementsApi.Data.Entities;

namespace HousePlantMeasurementsApi.DTOs.Plant
{
    public class GetPlantDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string? Name { get; set; }
        public string? Species { get; set; }
        public string? Description { get; set; }

        public double? MoistureLowLimit { get; set; }
        public double? MoistureHighLimit { get; set; }

        public double? TemperatureLowLimit { get; set; }
        public double? TemperatureHighLimit { get; set; }

        public double? LightIntensityLowLimit { get; set; }
        public double? LightIntensityHighLimit { get; set; }

    }
}

