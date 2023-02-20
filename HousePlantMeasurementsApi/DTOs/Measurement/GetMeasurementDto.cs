using System;
using System.ComponentModel.DataAnnotations;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.MeasurementValue;

namespace HousePlantMeasurementsApi.DTOs.Measurement
{
    public class GetMeasurementDto
    {
        public int Id { get; set; }
        public DateTime Taken { get; set; }
        public int PlantId { get; set; }
        public int DeviceId { get; set; }

        public List<GetMeasurementValueDto> MeasurementValues { get; set; }
    }
}

