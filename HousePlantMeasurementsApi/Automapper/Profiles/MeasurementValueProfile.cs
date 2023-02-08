using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.MeasurementValue;

namespace HousePlantMeasurementsApi.Automapper.Profiles
{
    public class MeasurementValueProfile: Profile
    {
        public MeasurementValueProfile()
        {
            CreateMap<MeasurementValue, GetMeasurementValueDto>();
            CreateMap<PostMeasurementValueDto, MeasurementValue>();
        }
    }
}

