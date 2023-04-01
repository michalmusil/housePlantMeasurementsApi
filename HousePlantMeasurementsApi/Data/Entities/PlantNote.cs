using System;
using System.ComponentModel.DataAnnotations;

namespace HousePlantMeasurementsApi.Data.Entities
{
	public class PlantNote: BaseEntity
	{
		public string Text { get; set; }

		[Required]
		public int PlantId { get; set; }
		[Required]
		public Plant Plant { get; set; }
	}
}

