using System;
namespace HousePlantMeasurementsApi.DTOs.User
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
    }
}

