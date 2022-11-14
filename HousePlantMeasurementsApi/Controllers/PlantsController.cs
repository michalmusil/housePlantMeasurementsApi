using System;
using AutoMapper;
using HousePlantMeasurementsApi.DTOs.Plant;
using HousePlantMeasurementsApi.DTOs.User;
using HousePlantMeasurementsApi.Repositories.Plants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HousePlantMeasurementsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/v1/plants")]
    public class PlantsController: ControllerBase
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IPlantsRepository plantsRepository;

        public PlantsController(
            ILogger<PlantsController> logger,
            IMapper mapper,
            IPlantsRepository plantsRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.plantsRepository = plantsRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetPlantDto>>> GetAllPlants()
        {
            var plants = await plantsRepository.GetAllPlants();
            return Ok(mapper.Map<IEnumerable<GetPlantDto>>(plants));
        }

    }
}

