using System;
namespace HousePlantMeasurementsApi.Services.HashService
{
	public interface IHashService
	{
        public string HashUserPassword(string userPassword);
        public bool VerifyUserPassword(string userPassword, string passwordHash);

        public string HashCommunicationIdentifier(string communicationIdentifier);
        public bool VerifyCommunicationIdentifier(string communicationIdentifier, string identifierHash);

        public string HashMacAddress(string macAddress);
        public bool VerifyMacAddress(string macAddress, string macHash);
    }
}

