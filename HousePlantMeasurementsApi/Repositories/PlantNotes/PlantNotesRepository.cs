using System;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HousePlantMeasurementsApi.Repositories.PlantNotes
{
    public class PlantNotesRepository : IPlantNotesRepository
    {
        private readonly PlantMeasurementsDbContext dbContext;

        public PlantNotesRepository(
            PlantMeasurementsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<PlantNote>> GetByPlantId(int plantId)
        {
            var foundNotes = await dbContext.PlantNotes
                .Where(n => n.PlantId == plantId)
                .Include(n => n.Plant)
                .ToListAsync();

            return foundNotes;
        }

        public async Task<PlantNote?> GetById(int id)
        {
            var foundNote = await dbContext.PlantNotes
                .Where(n => n.Id == id)
                .Include(n => n.Plant)
                .FirstOrDefaultAsync();
            return foundNote;
        }

        public async Task<PlantNote?> AddPlantNote(PlantNote plantNote)
        {
            dbContext.Add(plantNote);
            var saved = await dbContext.SaveChangesAsync() > 0;
            if (saved)
            {
                return plantNote;
            }
            return null;
        }

        public async Task<bool> DeletePlantNote(PlantNote plantNote)
        {
            dbContext.Remove(plantNote);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}

