using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("Measurements")]
    public class Measurement: BaseEntity
    {
        [Required]
        public double Moisture { get; set; }
        [Required]
        public double Temperature { get; set; }
        [Required]
        public double LightIntensity { get; set; }
        // Will probably correspond with Created, but speciffically here for future updates (for example if the measuring device would cache the data)
        [Required]
        public DateTime Taken { get; set; }

        [Required]
        public int PlantId { get; set; }
        [Required]
        public Plant Plant { get; set; }

        [Required]
        public int DeviceId { get; set; }
        [Required]
        public Device Device { get; set; }
    }
}

