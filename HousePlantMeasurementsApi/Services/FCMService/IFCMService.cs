using System;
using HousePlantMeasurementsApi.Data.Entities;

namespace HousePlantMeasurementsApi.Services.FCMService
{
	public interface IFCMService
	{
        public Task<bool> SendNotification(string notificationToken, string title, string message, string plantName);
    }
}

