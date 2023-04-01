using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("Plants")]
    public class Plant : BaseEntity
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Name { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Species { get; set; }

        [MaxLength(5000)]
        public string? Description { get; set; }

        public string? TitleImagePath { get; set; }

        [Required]
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }

        public List<PlantNote> PlantNotes { get; set; }

        public List<Measurement> Measurements { get; set; }

        public List<MeasurementValueLimit> MeasurementValueLimits { get; set; }
    }
}

