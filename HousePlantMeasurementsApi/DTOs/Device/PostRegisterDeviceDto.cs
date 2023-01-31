using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Device
{
    public class PostRegisterDeviceDto
    {
        [Required]
        [MinLength(30)]
        public string UUID { get; set; }

        [Required]
        [MinLength(17)]
        [MaxLength(17)]
        public string MacAddress { get; set; }
    }
}

