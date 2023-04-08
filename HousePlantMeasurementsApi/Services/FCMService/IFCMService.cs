using System;
using HousePlantMeasurementsApi.Data.Entities;

namespace HousePlantMeasurementsApi.Services.FCMService
{
	public interface IFCMService
	{
        public Task<bool> SendPlantNotification(string notificationToken, Plant plant, Measurement measurement);
    }
}

