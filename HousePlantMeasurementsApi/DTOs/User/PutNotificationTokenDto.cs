using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.User
{
	public class PutNotificationTokenDto
	{
        [Required]
        public string NotificationToken { get; set; }
    }
}

