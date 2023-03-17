using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Device
{
    public class PostRegisterDeviceDto
    {
        [Required]
        [MinLength(15)]
        [MaxLength(15)]
        public string CommunicationIdentifier { get; set; }
        [Required]
        [MinLength(17)]
        [MaxLength(17)]
        public string MacAddress { get; set; }
    }
}

