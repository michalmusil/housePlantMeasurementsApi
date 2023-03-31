using System;
using System.Data.Common;

namespace HousePlantMeasurementsApi.Services.HashService
{
	public class HashService: IHashService
	{
        private readonly int numberOfSaltChars = 22;
        private readonly int workFactor = 13;
        private readonly IConfiguration appConfiguration;

        public HashService(IConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }

        public string HashCommunicationIdentifier(string communicationIdentifier)
        {
            var communicationIdentifierSection = appConfiguration.GetSection("CommunicationIdentifier");
            var salt = communicationIdentifierSection.GetValue<string>("Salt");

            var hashed = BCrypt.Net.BCrypt.HashPassword(communicationIdentifier, salt);

            var rawHash = BCrypt.Net.BCrypt.InterrogateHash(hashed).RawHash;

            var withoutSalt = rawHash.Substring(numberOfSaltChars - 1);

            return withoutSalt;
        }

        public bool VerifyCommunicationIdentifier(string communicationIdentifier, string identifierHash)
        {          
            var checkedForHash = HashCommunicationIdentifier(communicationIdentifier);
            return checkedForHash == identifierHash;
        }

        public string HashMacAddress(string macAddress)
        {
            var hashed = BCrypt.Net.BCrypt.HashPassword(macAddress, workFactor);
            return hashed;
        }

        public bool VerifyMacAddress(string macAddress, string macHash)
        {
            return BCrypt.Net.BCrypt.Verify(macAddress, macHash);
        }

        public string HashUserPassword(string userPassword)
        {
            var hashed = BCrypt.Net.BCrypt.HashPassword(userPassword, workFactor);
            return hashed;
        }

        public bool VerifyUserPassword(string userPassword, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(userPassword, passwordHash);
        }
    }
}

