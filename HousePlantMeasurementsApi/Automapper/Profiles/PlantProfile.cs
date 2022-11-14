using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.Plant;

namespace HousePlantMeasurementsApi.Automapper.Profiles
{
    public class PlantProfile: Profile
    {
        public PlantProfile()
        {
            CreateMap<Plant, GetPlantDto>();
            CreateMap<PostPlantDto, Plant>();
        }
    }
}

