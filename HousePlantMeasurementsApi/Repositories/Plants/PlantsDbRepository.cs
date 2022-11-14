using System;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HousePlantMeasurementsApi.Repositories.Plants
{
    public class PlantsDbRepository: IPlantsRepository
    {
        private readonly PlantMeasurementsDbContext dbContext;
        public PlantsDbRepository(PlantMeasurementsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Plant>> GetAllPlants()
        {
            return await dbContext.Plants.ToListAsync();
        }

        public async Task<Plant?> GetById(int id)
        {
            return await dbContext.Plants.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Plant>> GetByUserId(int userId)
        {
            var plants = await dbContext.Plants.Where(p => p.UserId == userId).ToListAsync();
            return plants;
        }

        public async Task<bool> AddPlant(Plant plant)
        {
            dbContext.Add(plant);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePlant(Plant plant)
        {
            dbContext.Update(plant);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePlant(Plant plant)
        {
            dbContext.Remove(plant);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}

