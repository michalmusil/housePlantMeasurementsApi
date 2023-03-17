using System;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.MeasurementValue;
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

        public async Task<IEnumerable<GetLatestMeasurementValueDto>> GetMostRecentValuesOfPlant(int plantId)
        {
            var values = new List<GetLatestMeasurementValueDto>();

            var measurementTypes = Enum.GetValues(typeof(MeasurementType));
            foreach(var measurementType in measurementTypes)
            {
                var castedType = (MeasurementType)measurementType;
                var result = await dbContext.Measurements.Join(
                    dbContext.MeasurementValues,
                    m => m.Id,
                    v => v.MeasurementId,
                    (m, v) => new { m = m, v = v })
                    .Where(x => x.m.PlantId == plantId && x.v.Type == castedType)
                    .OrderByDescending(x => x.m.Taken)
                    .FirstOrDefaultAsync();

                if(result != null)
                {
                    var latestValue = new GetLatestMeasurementValueDto()
                    {
                        Type = result.v.Type,
                        Value = result.v.Value,
                        MeasurementId = result.m.Id,
                        Taken = result.m.Taken
                    };
                    values.Add(latestValue);
                }
            }

            return values;
        }

        public async Task<Measurement?> GetById(int id)
        {
            return await dbContext.Measurements.Where(m => m.Id == id)
                .Include(m => m.MeasurementValues)
                .FirstOrDefaultAsync();
        }

        public async Task<Measurement?> AddMeasurement(Measurement measurement)
        {
            measurement.Taken = DateTime.UtcNow;
            dbContext.Add(measurement);
            var savedSuccessfully = await dbContext.SaveChangesAsync() > 0;
            if (savedSuccessfully)
            {
                return measurement;
            }
            return null;
        }

        public async Task<bool> DeleteMeasurement(Measurement measurement)
        {
            dbContext.Remove(measurement);
            return await dbContext.SaveChangesAsync() > 0;
        }     
    }
}

