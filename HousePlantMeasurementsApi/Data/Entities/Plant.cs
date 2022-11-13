using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("Plants")]
    public class Plant : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Species { get; set; }
        public string? Description { get; set; }

        public double? MoistureLowLimit { get; set; }
        public double? MoistureHighLimit { get; set; }

        public double? TemperatureLowLimit { get; set; }
        public double? TemperatureHighLimit { get; set; }

        public double? LightIntensityLowLimit { get; set; }
        public double? LightIntensityHighLimit { get; set; }

        [Required]
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }

        List<PlantImage> PlantImages { get; set; }
        List<Measurement> Measurements { get; set; }
    }
}

