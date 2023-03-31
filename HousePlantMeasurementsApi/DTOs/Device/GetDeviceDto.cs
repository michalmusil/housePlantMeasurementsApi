﻿using System;
namespace HousePlantMeasurementsApi.DTOs.Device
{
    public class GetDeviceDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int? UserId { get; set; }
        public int? PlantId { get; set; }
    }
}

