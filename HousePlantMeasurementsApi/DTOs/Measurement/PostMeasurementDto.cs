using System;
using System.ComponentModel.DataAnnotations;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.MeasurementValue;

namespace HousePlantMeasurementsApi.DTOs.Measurement
{
    public class PostMeasurementDto
    {
        [Required]
        public string DeviceCommunicationIdentifier { get; set; }

        [Required]
        public string DeviceMacAddress { get; set; }

        [Required]
        public List<PostMeasurementValueDto> MeasurementValues { get; set; }
    }
}

