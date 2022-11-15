using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Device
{
    public class PostRegisterDeviceDto
    {
        [Required]
        [MinLength(30)]
        public string UUID { get; set; }
    }
}

