﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Auth;
using HousePlantMeasurementsApi.DTOs.User;
using HousePlantMeasurementsApi.Repositories.Users;
using Microsoft.IdentityModel.Tokens;

namespace HousePlantMeasurementsApi.Services.AuthService
{
    public class AuthService: IAuthService
    {

        private readonly IConfiguration appConfiguration;
        private readonly IUsersRepository usersRepository;

        public AuthService(IUsersRepository usersRepository, IConfiguration appConfiguration)
        {
            this.usersRepository = usersRepository;
            this.appConfiguration = appConfiguration;
        }




        public async Task<User> GetUserWithCredentials (PostUserLoginDto loginDto)
        {
            var user = await usersRepository.GetByEmail(loginDto.Email);
            if (user != null)
            {
                var passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
                if (passwordValid)
                {
                    return user;
                }
            }
            return null;
        }

        public async Task<GetAuthDto> LogUserIn(User user)
        {
            var appJwtSection = appConfiguration.GetSection("Jwt");

            var audience = appJwtSection.GetValue<string>("Audience");
            var issuer = appJwtSection.GetValue<string>("Issuer");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appJwtSection.GetValue<string>("Secret")));
            var validUntil = DateTime.Now.AddHours(appJwtSection.GetValue<int>("ExpirationInHours"));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>() { new Claim("userId", user.Id.ToString()) };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: validUntil,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new GetAuthDto() { Id = user.Id, Token = tokenString };

        }

        public async Task<int?> GetSignedUserId(ClaimsPrincipal? user)
        {
            var signedUserClaim = user?.Claims.Where(c => c.Type == "userId").FirstOrDefault();
            if (signedUserClaim == null)
            {
                return null;
            }
            try
            {
                return Int32.Parse(signedUserClaim.Value);
            }
            catch
            {
                return null;
            }
        }


        public async Task<bool> SignedUserHasRole(ClaimsPrincipal? user, UserRole role)
        {
            var signedUserId = await GetSignedUserId(user);
            if (signedUserId.HasValue)
            {
                var signedUserRole = await GetUserRole(signedUserId.Value);
                return signedUserRole == role;
            }

            return false;
        }

        public async Task<bool> SignedUserHasId(ClaimsPrincipal? user, int id)
        {
            return await GetSignedUserId(user) == id;
        }

        public string GetDeviceAuthHashBase(string macAddress)
        {
            return macAddress;
        }

        public string? GetDeviceAuthHash(string macAddress)
        {
            var baseString = GetDeviceAuthHashBase(macAddress);
            var hashed = BCrypt.Net.BCrypt.HashPassword(baseString);
            return hashed;
        }





        private async Task<UserRole?> GetUserRole(int userId)
        {
            var foundUser = await usersRepository.GetById(userId);
            if (foundUser == null)
            {
                return null;
            }
            return foundUser.Role;
        }
    }
}

