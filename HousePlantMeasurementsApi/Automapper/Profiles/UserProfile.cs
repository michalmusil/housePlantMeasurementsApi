using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.User;

namespace HousePlantMeasurementsApi.Automapper.Profiles
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserDto>();
            CreateMap<PostUserDto, User>();        
        }
    }
}

