using System;
using AutoMapper;
using HousePlantMeasurementsApi.Repositories.Plants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HousePlantMeasurementsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("plants")]
    public class PlantsController: ControllerBase
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IPlantsRepository plantsRepository;

        public PlantsController(
            ILogger logger,
            IMapper mapper,
            IPlantsRepository plantsRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.plantsRepository = plantsRepository;
        }


        [HttpGet(Name ="List")]
        public async Task<IActionResult<IEnumerable<GetPlantDto>>> GetAllPlants()
        {

        }
    }
}

