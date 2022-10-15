using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("PlantImages")]
    public class PlantImage: BaseEntity
    {
        [Required]
        public string ImageName { get; set; }

        [Required]
        public int PlantId { get; set; }
        [Required]
        public Plant Plant { get; set; }
    }
}

