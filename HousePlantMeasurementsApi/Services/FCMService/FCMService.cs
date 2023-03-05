using System;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace HousePlantMeasurementsApi.Services.FCMService
{
	public class FCMService: IFCMService
	{
        private readonly string pathToCredentials = "FCM/Credentials/";
        private readonly string credentialsFileName = "plant-monitor-mobile-app-firebase-adminsdk-xdfd6-bb86cfc257.json";

        private readonly string titleKey = "title";
        private readonly string messageKey = "message";
        private readonly string plantNameKey = "plant_name";

        public FCMService()
		{
            if (FirebaseMessaging.DefaultInstance == null)
            {
                FirebaseApp.Create(options: new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(pathToCredentials + credentialsFileName)
                });
            }
		}


        public async Task<bool> SendNotification(string notificationToken, string title, string message, string plantName)
        {
            var notificationToSend = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { titleKey, title },
                    { messageKey, message },
                    { plantNameKey, plantName }
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

