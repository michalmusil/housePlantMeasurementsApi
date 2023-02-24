using System;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HouseDeviceMeasurementsApi.Repositories.Devices
{
    public class DevicesDbRepository: IDevicesRepository
    {
        private readonly PlantMeasurementsDbContext dbContext;

        public DevicesDbRepository(PlantMeasurementsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Device>> GetAllDevices(bool? registered = null)
        {
            var devices = dbContext.Devices.AsNoTracking();

            if (registered == true)
            {
                devices = devices.Where(d => d.UserId != null);
            }
            else if (registered == false)
            {
                devices = devices.Where(d => d.UserId == null);
            }

            return await devices.ToListAsync();
        }

        public async Task<Device?> GetById(int id)
        {
            return await dbContext.Devices.Where(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Device?> GetByUUID(string uuid)
        {
            return await dbContext.Devices.Where(d => d.UUID == uuid).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Device>> GetByUserId(int userId)
        {
            var devices = await dbContext.Devices.Where(d => d.UserId == userId).ToListAsync();
            return devices;
        }

        public async Task<Device?> AddDevice(Device device)
        {
            dbContext.Add(device);
            var savedSuccessfully = await dbContext.SaveChangesAsync() > 0;
            if (savedSuccessfully)
            {
                return device;
            }
            return null;
        }

        public async Task<bool> UpdateDevice(Device device)
        {
            device.Updated = DateTime.UtcNow;
            dbContext.Update(device);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteDevice(Device device)
        {
            dbContext.Remove(device);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}

