using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("Devices")]
    public class Device: BaseEntity
    {
        [Required]
        public string UUID { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
    }
}

