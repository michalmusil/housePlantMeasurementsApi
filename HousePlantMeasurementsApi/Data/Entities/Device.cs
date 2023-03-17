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
        [MinLength(15)]
        [MaxLength(15)]
        public string CommunicationIdentifier { get; set; }

        // The hash of devices MAC address - ensures dependency on physical device
        [Required]
        public string MacHash { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }

    }
}

