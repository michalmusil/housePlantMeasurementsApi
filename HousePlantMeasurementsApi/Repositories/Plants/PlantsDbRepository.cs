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
            return await dbContext.Plants
                .Include(p => p.MeasurementValueLimits)
                .ToListAsync();
        }

        public async Task<Plant?> GetById(int id)
        {
            return await dbContext.Plants.Where(p => p.Id == id)
                .Include(p => p.MeasurementValueLimits)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Plant>> GetByUserId(int userId)
        {
            var plants = await dbContext.Plants.Where(p => p.UserId == userId)
                .Include(p => p.MeasurementValueLimits)
                .ToListAsync();
            return plants;
        }

        public async Task<Plant?> AddPlant(Plant plant)
        {
            dbContext.Add(plant);
            var savedSuccessfully = await dbContext.SaveChangesAsync() > 0;
            if (savedSuccessfully)
            {
                return plant;
            }
            return null;
        }

        public async Task<bool> UpdatePlant(Plant plant)
        {
            plant.Updated = DateTime.UtcNow;
            dbContext.Update(plant);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveLimitsOfPlant(Plant plant)
        {
            var limitsToRemove = dbContext.MeasurementValueLimits.Where(l => l.PlantId == plant.Id);
            dbContext.MeasurementValueLimits.RemoveRange(limitsToRemove);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePlant(Plant plant)
        {
            dbContext.Remove(plant);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}

