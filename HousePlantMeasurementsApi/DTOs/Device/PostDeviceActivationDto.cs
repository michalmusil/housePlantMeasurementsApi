using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Device
{
    public class PostDeviceActivationDto
    {
        [Required]
        public int DeviceId { get; set; }
        [Required]
        public bool IsActive { get; set; }

    }
}

