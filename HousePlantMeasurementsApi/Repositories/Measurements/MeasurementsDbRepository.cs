using System;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HousePlantMeasurementsApi.Repositories.Measurements
{
    public class MeasurementsDbRepository: IMeasurementsRepository
    {
        private readonly PlantMeasurementsDbContext dbContext;
        public MeasurementsDbRepository(PlantMeasurementsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Measurement>> GetByPlantId(int plantId)
        {
            return await dbContext.Measurements.Where(m => m.PlantId == plantId)
                .Include(m => m.MeasurementValues)
                .ToListAsync();
        }

        public async Task<IEnumerable<Measurement>> GetByDeviceId(int deviceId)
        {
            return await dbContext.Measurements.Where(m => m.DeviceId == deviceId)
                .Include(m => m.MeasurementValues)
                .ToListAsync();
        }

        public async Task<Measurement?> GetById(int id)
        {
            return await dbContext.Measurements.Where(m => m.Id == id)
                .Include(m => m.MeasurementValues)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddMeasurement(Measurement measurement)
        {
            measurement.Taken = DateTime.UtcNow;
            dbContext.Add(measurement);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMeasurement(Measurement measurement)
        {
            dbContext.Remove(measurement);
            return await dbContext.SaveChangesAsync() > 0;
        }


    }
}

