using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.Measurement;

namespace HousePlantMeasurementsApi.Automapper.Profiles
{
    public class MeasurementProfile: Profile
    {
        public MeasurementProfile()
        {
            CreateMap<PostMeasurementDto, Measurement>();
            CreateMap<Measurement,GetMeasurementDto>();
        }
    }
}

