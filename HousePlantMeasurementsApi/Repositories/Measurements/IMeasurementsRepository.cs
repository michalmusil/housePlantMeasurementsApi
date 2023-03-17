using System;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.MeasurementValue;

namespace HousePlantMeasurementsApi.Repositories.Measurements
{
    public interface IMeasurementsRepository
    {

        public Task<IEnumerable<Measurement>> GetByPlantId(int plantId, DateTime? from, DateTime? to);

        public Task<IEnumerable<Measurement>> GetByDeviceId(int deviceId, DateTime? from, DateTime? to);

        public Task<Measurement?> GetById(int id);

        public Task<Measurement?> GetMostRecentByMeasurementType(int plantId, MeasurementType measurementType);

        public Task<IEnumerable<GetLatestMeasurementValueDto>> GetMostRecentValuesOfPlant(int plantId);

        public Task<Measurement?> AddMeasurement(Measurement measurement);

        public Task<bool> DeleteMeasurement(Measurement measurement);
    }
}

