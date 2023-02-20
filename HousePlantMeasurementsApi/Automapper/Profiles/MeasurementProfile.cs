using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Measurement;

namespace HousePlantMeasurementsApi.Automapper.Profiles
{
    public class MeasurementProfile: Profile
    {
        public MeasurementProfile()
        {
            CreateMap<PostMeasurementDto, Measurement>();
            CreateMap<Measurement,GetMeasurementDto>();

            CreateMap<MeasurementType, GetMeasurementTypeDto>()
                .ForMember(dto => dto.Number, opt => opt.MapFrom(measurementType => (int)measurementType))
                .ForMember(dto => dto.Name, opt => opt.MapFrom(measurementType => measurementType.ToString()));
        }
    }
}

