using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HousePlantMeasurementsApi.Data.Enums;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("MeasurementValues")]
    public class MeasurementValue: BaseEntity
    {
        [Required]
        public MeasurementType Type { get; set; }
        [Required]
        public double Value { get; set; }

        [Required]
        public int MeasurementId { get; set; }
        [Required]
        public Measurement Measurement{ get; set; }
    }
}

