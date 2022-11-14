using System;
using HousePlantMeasurementsApi.Data.Entities;

namespace HousePlantMeasurementsApi.Repositories.Plants
{
    public interface IPlantsRepository
    {
        public Task<IEnumerable<Plant>> GetAllPlants();

        public Task<Plant?> GetById(int id);

        public Task<IEnumerable<Plant>> GetByUserId(int userId);

        public Task<bool> AddPlant(Plant plant);

        public Task<bool> UpdatePlant(Plant plant);

        public Task<bool> DeletePlant(Plant plant);
    }
}

