using System;
using HousePlantMeasurementsApi.Data.Entities;

namespace HouseDeviceMeasurementsApi.Repositories.Devices
{
    public interface IDevicesRepository
    {
        public Task<IEnumerable<Device>> GetAllDevices(bool? registered);

        public Task<Device?> GetById(int id);

        public Task<Device?> GetByCommunicationIdentifier(string communicationIdentifier);

        public Task<IEnumerable<Device>> GetByUserId(int userId);

        public Task<Device?> AddDevice(Device device);

        public Task<bool> UpdateDevice(Device device);
    }
}

