using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.Device;

namespace HousePlantMeasurementsApi.Automapper.Profiles
{
    public class DeviceProfile: Profile
    {
        public DeviceProfile()
        {
            CreateMap<Device, GetDeviceDto>();
            CreateMap<PostDeviceDto, Device>();
        }
    }
}

