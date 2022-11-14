using System;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs.Auth;
using HousePlantMeasurementsApi.DTOs.User;

namespace HousePlantMeasurementsApi.Services.AuthService
{
    public interface IAuthService
    {

        public Task<User> GetUserWithCredentials(PostUserLoginDto loginDto);

        public Task<GetAuthDto> LogUserIn(User user);
    }
}

