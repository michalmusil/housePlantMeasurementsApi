using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HousePlantMeasurementsApi.Data.Entities
{
    [Table("Devices")]
    public class Device: BaseEntity
    {
        [Required]
        [MinLength(30)]
        public string UUID { get; set; }

        // The hash of devices MAC address - ensures dependency on physical device
        [Required]
        public string AuthHash { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsDeleted { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }



        public Device()
        {
            this.IsDeleted = false;
        }
    }
}

