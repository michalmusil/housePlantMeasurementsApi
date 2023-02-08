using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.MeasurementValueLimit;

namespace HousePlantMeasurementsApi.Automapper.Profiles
{
    public class MeasurementValueLimitProfile: Profile
    {
        public MeasurementValueLimitProfile()
        {
            CreateMap<MeasurementValueLimit, GetMeasurementValueLimitDto>();
            CreateMap<PostMeasurementValueLimitDto, MeasurementValueLimit>();
            CreateMap<PutMeasurementValueLimitDto, MeasurementValueLimit>();
        }
    }
}

