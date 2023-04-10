using System;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Services.ValidationHelperService;

namespace HousePlantMeasurementsApi.Services.FCMService
{
	public class FCMService: IFCMService
	{
        private readonly IMeasurementValidator measurementValidator;

        private readonly string pathToCredentials = "FCM/Credentials/";
        private readonly string credentialsFileName = "plant-monitor-mobile-app-firebase-adminsdk-xdfd6-bb86cfc257.json";
        private readonly string titleKey = "title";
        private readonly string messageKey = "message";
        private readonly string plantNameKey = "plant_name";

        public FCMService(IMeasurementValidator measurementValidator)
		{
            this.measurementValidator = measurementValidator;
            if (FirebaseMessaging.DefaultInstance == null)
            {
                FirebaseApp.Create(options: new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(pathToCredentials + credentialsFileName)
                });
            }
		}


        public async Task<bool> SendPlantNotification(string notificationToken, Plant plant, Measurement measurement)
        {
            var title = plant.Name + "❗️";
            var message = "";

            var invalidTypes = measurementValidator.GetInvalidMeasurementTypes(measurement, plant.MeasurementValueLimits);

            foreach(var invalidType in invalidTypes)
            {
                switch (invalidType)
                {
                    case Data.Enums.MeasurementType.Temperature:
                        message += "🌡";
                        break;
                    case Data.Enums.MeasurementType.LightIntensity:
                        message += "☀️";
                        break;
                    case Data.Enums.MeasurementType.SoilMoisture:
                        message += "💧";
                        break;

                }
            }

            var notificationToSend = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { titleKey, title },
                    { messageKey, message },
                    { plantNameKey, plant.Name }
                },
                Token = notificationToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = message
                }
            };

            var result = await FirebaseMessaging.DefaultInstance.SendAsync(notificationToSend);

            return result != null && result.Length > 0;
        }
    }
}

