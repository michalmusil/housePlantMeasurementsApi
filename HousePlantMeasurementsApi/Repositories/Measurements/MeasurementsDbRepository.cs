using System;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
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

        public async Task<IEnumerable<Measurement>> GetByPlantId(int plantId, DateTime? from, DateTime? to)
        {
            var measurements = dbContext.Measurements.Where(m => m.PlantId == plantId)
                .Include(m => m.MeasurementValues).AsNoTracking();

            if(from != null)
            {
                measurements = measurements.Where(m => m.Taken >= from);
            }

            if (to != null)
            {
                measurements = measurements.Where(m => m.Taken <= to);
            }

            return await measurements
                .OrderBy(m => m.Taken)
                .ToListAsync();
        }

        public async Task<IEnumerable<Measurement>> GetByDeviceId(int deviceId, DateTime? from, DateTime? to)
        {
            var measurements = dbContext.Measurements.Where(m => m.DeviceId == deviceId)
                .Include(m => m.MeasurementValues).AsNoTracking();

            if (from != null)
            {
                measurements = measurements.Where(m => m.Taken >= from);
            }

            if (to != null)
            {
                measurements = measurements.Where(m => m.Taken <= to);
            }

            return await measurements
                .OrderBy(m => m.Taken)
                .ToListAsync();
        }

        public async Task<Measurement?> GetMostRecentByMeasurementType(int plantId, MeasurementType measurementType)
        {
            var foundMeasurement = await dbContext.MeasurementValues
                .Where(v => v.Measurement.PlantId == plantId && v.Type == measurementType)
                .Select(v => v.Measurement)
                .OrderByDescending(m => m.Taken)
                .FirstOrDefaultAsync();

            if(foundMeasurement == null)
            {
                return null;
            }

            var measurementWithValues = await dbContext.Measurements
                .Where(m => m.Id == foundMeasurement.Id)
                .Include(m => m.MeasurementValues)
                .FirstOrDefaultAsync();

            return measurementWithValues;
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

