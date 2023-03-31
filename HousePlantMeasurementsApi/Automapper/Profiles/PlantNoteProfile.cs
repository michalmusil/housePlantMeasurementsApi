using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.PlantNotes;

namespace HousePlantMeasurementsApi.Automapper.Profiles
{
	public class PlantNoteProfile: Profile
	{
		public PlantNoteProfile()
		{
			CreateMap<PlantNote, GetPlantNoteDto>();
            CreateMap<PostPlantNoteDto, PlantNote>();
        }
	}
}

