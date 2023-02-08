using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HousePlantMeasurementsApi.Data.Enums;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("Measurements")]
    public class Measurement: BaseEntity
    {
        // Will probably correspond with Created, but speciffically here for future updates (for example if the measuring device would cache the data)
        [Required]
        public DateTime Taken { get; set; }

        public List<MeasurementValue> MeasurementValues { get; set; }

        [Required]
        public int PlantId { get; set; }
        [Required]
        public Plant Plant { get; set; }

        [Required]
        public int DeviceId { get; set; }
        [Required]
        public Device Device { get; set; }



        public double? getValueByType(MeasurementType type)
        {
            var measurementValue =  this.MeasurementValues.Where(v => v.Type == type).FirstOrDefault();
            return measurementValue?.Value;
        }
    }
}

