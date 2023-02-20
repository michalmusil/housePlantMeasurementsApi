using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.Plant;
using HousePlantMeasurementsApi.Services.ImageService;

namespace HousePlantMeasurementsApi.Automapper.Profiles
{
    public class PlantProfile: Profile
    {
        public PlantProfile()
        {
            CreateMap<Plant, GetPlantDto>()
                .ForMember(dto => dto.HasTitleImage, opt => opt.MapFrom(plant => plant.TitleImagePath != null && plant.TitleImagePath.Length > 0));
            CreateMap<PostPlantDto, Plant>();
            CreateMap<PutPlantDto, Plant>();
        }
    }
}

