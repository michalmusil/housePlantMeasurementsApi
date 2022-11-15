using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Plant;
using HousePlantMeasurementsApi.Repositories.Plants;
using HousePlantMeasurementsApi.Repositories.Users;
using HousePlantMeasurementsApi.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HousePlantMeasurementsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/v1/measurements")]
    public class MeasurementsController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IPlantsRepository plantsRepository;
        private readonly IUsersRepository usersRepository;
        private readonly IAuthService authService;

        public MeasurementsController(
            ILogger<PlantsController> logger,
            IMapper mapper,
            IPlantsRepository plantsRepository,
            IUsersRepository usersRepository,
            IAuthService authService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.plantsRepository = plantsRepository;
            this.usersRepository = usersRepository;
            this.authService = authService;
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GetPlantDto>>> GetAllPlantsOfUser(int userId)
        {
            var ownerOfPlants = await usersRepository.GetById(userId);

            if (ownerOfPlants == null)
            {
                return NotFound();
            }

            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, userId);

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }

            var plants = await plantsRepository.GetByUserId(userId);
            return Ok(mapper.Map<IEnumerable<GetPlantDto>>(plants));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetPlantDto>> GetById(int id)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var foundPlant = await plantsRepository.GetById(id);

            if (foundPlant == null)
            {
                return NotFound();
            }

            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, foundPlant.UserId);

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }

            return Ok(mapper.Map<GetPlantDto>(foundPlant));
        }

        [HttpPost]
        public async Task<ActionResult<GetPlantDto>> AddNewPlant(PostPlantDto plantPost)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, plantPost.UserId);

            var plantOwner = await usersRepository.GetById(plantPost.UserId);

            if (plantOwner == null)
            {
                return NotFound(new { message = $"User with id: {plantPost.UserId} was not found" });
            }

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }

            Plant? newPlant = null;
            try
            {
                newPlant = mapper.Map<Plant>(plantPost);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Adding plant not successfull: {ex.ToString()}");
                return BadRequest();
            }

            var savedPlant = await plantsRepository.AddPlant(newPlant);

            return Ok(mapper.Map<GetPlantDto>(newPlant));
        }

    }
}

