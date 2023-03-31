using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.DTOs.PlantNotes
{
	public class PostPlantNoteDto
	{
        [Required]
        public string Text { get; set; }

        [Required]
        public int PlantId { get; set; }
    }
}

