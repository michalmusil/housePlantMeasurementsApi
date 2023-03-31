using System;
using HousePlantMeasurementsApi.Data.Entities;

namespace HousePlantMeasurementsApi.Repositories.PlantNotes
{
	public interface IPlantNotesRepository
	{
        public Task<IEnumerable<PlantNote>> GetByPlantId(int plantId);

        public Task<PlantNote?> GetById(int id);

        public Task<PlantNote?> AddPlantNote(PlantNote plantNote);

        public Task<bool> DeletePlantNote(PlantNote plantNote);
    }
}

