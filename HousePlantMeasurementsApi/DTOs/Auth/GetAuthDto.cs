using System;
using HousePlantMeasurementsApi.Data.Enums;

namespace HousePlantMeasurementsApi.DTOs.Auth
{
    public class GetAuthDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string Token { get; set; }
    }
}

