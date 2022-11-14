using System;
using AutoMapper;
using HousePlantMeasurementsApi.DTOs.Auth;
using HousePlantMeasurementsApi.DTOs.User;
using HousePlantMeasurementsApi.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace HousePlantMeasurementsApi.Controllers
{
    [ApiController]
    [Route("/api/v1/auth")]
    public class AuthController: ControllerBase
    {
        public IConfiguration appConfiguration { get; set; }
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IAuthService authService;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger, IMapper mapper, IAuthService authService)
        {
            this.appConfiguration = configuration;
            this.logger = logger;
            this.mapper = mapper;
            this.authService = authService;
        }


        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<GetAuthDto>> LogIn(PostUserLoginDto loginDto)
        {
            var user = await authService.GetUserWithCredentials(loginDto);
            if (user == null)
            {
                return NotFound();
            }
            var token = await authService.LogUserIn(user);
            return Ok(token);
        }

    }
}

