using System;
using System.Security.Claims;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Auth;
using HousePlantMeasurementsApi.DTOs.User;

namespace HousePlantMeasurementsApi.Services.AuthService
{
    public interface IAuthService
    {

        public Task<User> GetUserWithCredentials(PostUserLoginDto loginDto);

        public Task<GetAuthDto> LogUserIn(User user);

        public Task<int?> GetUserIdFromClaims(IEnumerable<Claim> claims);

        public Task<UserRole?> GetUserRole(int userId);
    }
}

