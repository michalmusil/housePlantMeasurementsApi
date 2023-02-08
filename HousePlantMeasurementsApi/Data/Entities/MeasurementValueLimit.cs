using System;
using HousePlantMeasurementsApi.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("MeasurementValueLimits")]
    public class MeasurementValueLimit : BaseEntity
    {
        [Required]
        public MeasurementType Type { get; set; }

        [Required]
        public double LowerLimit { get; set; }

        [Required]
        public double UpperLimit { get; set; }

        [Required]
        public int PlantId { get; set; }
        [Required]
        public Plant Plant { get; set; }
    }
}

