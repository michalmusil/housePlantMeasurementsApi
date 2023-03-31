using System;
namespace HousePlantMeasurementsApi.DTOs.PlantNotes
{
	public class GetPlantNoteDto
	{
        public int Id { get; set; }

		public string Text { get; set; }

        public int PlantId { get; set; }

        public DateTime Created { get; set; }
    }
}

