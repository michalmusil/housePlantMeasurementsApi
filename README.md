# Prototype of an application for monitoring houseplant environment - REST API
This is a 1/3 of a complete project solution for a bachelor's thesis. The thesis (and the underlying project) deals with designing and implementing multitier application for continuous monitoring of houseplant's environment (ambient temperature, light intensity and soil moisture). The application is meant to be used by multiple users with multiple plants and measuring devices.

# The functional parts of the projects
1. REST API with database for application logic (ASP.NET Core with Entity Framework Core): https://github.com/michalmusil/housePlantMeasurementsApi
2. Android navive app for presenting the data to users (Kotlin, Jetpack Compose): https://github.com/michalmusil/plant_monitor
3. ESP 8266 code for the device measuring the houseplant environment (C++ in Arduino IDE): https://github.com/michalmusil/housePlantMeasuringDevice

To make the project work as a whole, you have to get all of the parts working together.

## REST API
The REST API is implemented in the ASP.NET Core framework. Database is implemented using Entity Framework Core code-first approach and is meant to run on Microsoft SQL Server. All of the endpoints are documented using swagger, the documentation is available at /swagger/index.html after running in development environment. REST API is meant to receive measurements of plants environment from the measuring devices and to serve the users requests comming from the Android mobile app. The API also uses Firebase Cloud Messaging to send user push notifications to individual instances of user Android apps.

## How to make it work
To make the API work, you need to:
1. Create and host a Microsoft SQL Server database.
2. Set up project in Firebase for the application (if you already haven't while setting up the Android App, in that case, use that one). Generate and download private key in JSON format from your project's Firebase Console and store it in HousePlantMeasurementsApi/FCM/Credentials. In FCMService.cs change the fileName for your private key accordingly.
3. Adjust the appsettings.json for your configuration. Change the server address (optional), JWT and CommunicationIdentifier secrets (optional) and most importantly, add your SQL Server database's connection string(required).
4. Update your database with the migrations of the solution using Entity Framework (from CLI: dotnet ef database update)
5. Run the solution in development environment on your localhost, or publish and run it on a server.
6. Regular users can register on the /user/register endpoint without authentication. For you to add new measuring device instances to be used (at /devices POST), you have to be an Admin user. As a starting point, register as a regular user and then add yourself the admin role manually in the DB (set Role attribute to 1).

Helpers:
* Using Firebase Cloud Messaging with ASP.NET Core: https://medium.com/@tamashudak/building-net-core-app-server-to-send-firebase-cloud-messaging-message-requests-641f3c6c90ae