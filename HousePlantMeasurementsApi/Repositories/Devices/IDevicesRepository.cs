using System;
using HousePlantMeasurementsApi.Data.Entities;

namespace HouseDeviceMeasurementsApi.Repositories.Devices
{
    public interface IDevicesRepository
    {
        public Task<IEnumerable<Device>> GetAllDevices(bool? registered);

        public Task<Device?> GetById(int id);

        public Task<Device?> GetByUUID(string uuid);

        public Task<IEnumerable<Device>> GetByUserId(int userId);

        public Task<bool> AddDevice(Device device);

        public Task<bool> UpdateDevice(Device device);

        public Task<bool> DeleteDevice(Device device);
    }
}

