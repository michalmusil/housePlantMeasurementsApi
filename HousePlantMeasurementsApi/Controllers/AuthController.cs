using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace HousePlantMeasurementsApi.Controllers
{
    [ApiController]
    [Route("/api/v1/Auth")]
    public class AuthController: ControllerBase
    {
        public IConfiguration AppConfiguration { get; set; }
        private readonly ILogger Logger;
        private readonly IMapper Mapper;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger, IMapper mapper)
        {
            this.AppConfiguration = configuration;
            this.Logger = logger;
            this.Mapper = mapper;
        }

        [HttpGet(Name = "Authenticate")]
        public async Task<ActionResult> Authenticate()
        {
            return Ok("Odpoved probehla");
        }

    }
}

