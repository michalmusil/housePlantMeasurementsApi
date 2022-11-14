using System;
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


        public async Task<int?> GetUserIdFromClaims(IEnumerable<Claim> claims)
        {
            var signedUserClaim = claims.Where(c => c.Type == "userId").FirstOrDefault();
            if (signedUserClaim == null)
            {
                return null;
            }

            try
            {
                return Int32.Parse(signedUserClaim.Value);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<UserRole?> GetUserRole(int userId)
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

