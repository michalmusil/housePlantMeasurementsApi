using System;
using HousePlantMeasurementsApi.Data.Entities;

namespace HousePlantMeasurementsApi.Repositories.Measurements
{
    public interface IMeasurementsRepository
    {

        public Task<IEnumerable<Measurement>> GetByPlantId(int plantId);

        public Task<IEnumerable<Measurement>> GetByDeviceId(int deviceId);

        public Task<Measurement?> GetById(int id);

        public Task<bool> AddMeasurement(Measurement measurement);

        public Task<bool> DeleteMeasurement(Measurement measurement);
    }
}

