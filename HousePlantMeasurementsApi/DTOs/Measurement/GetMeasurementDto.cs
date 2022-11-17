using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.Measurement
{
    public class GetMeasurementDto
    {
        public int Id { get; set; }
        public double? Moisture { get; set; }
        public double? Temperature { get; set; }
        public double? LightIntensity { get; set; }
        public DateTime Taken { get; set; }
        public int PlantId { get; set; }
        public int DeviceId { get; set; }
    }
}

