using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Plant;
using HousePlantMeasurementsApi.DTOs.User;
using HousePlantMeasurementsApi.Repositories.Plants;
using HousePlantMeasurementsApi.Repositories.Users;
using HousePlantMeasurementsApi.Services.AuthService;
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
        private readonly IUsersRepository usersRepository;
        private readonly IAuthService authService;

        public PlantsController(
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
        public async Task<ActionResult<IEnumerable<GetPlantDto>>> GetAllPlants(int userId)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, userId);
            var ownerOfPlants = await usersRepository.GetById(userId);

            if (ownerOfPlants == null)
            {
                return NotFound();
            }
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
            catch(Exception ex)
            {
                logger.LogInformation($"Adding plant not successfull: {ex.ToString()}");
                return BadRequest();
            }

            var savedPlant = await plantsRepository.AddPlant(newPlant);

            return Ok(mapper.Map<GetPlantDto>(newPlant));
        }

        [HttpPut]
        public async Task<ActionResult<GetPlantDto>> UpdatePlant(PutPlantDto plantPut)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var plantToUpdate = await plantsRepository.GetById(plantPut.Id);

            if (plantToUpdate == null)
            {
                return NotFound();
            }

            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, plantToUpdate.UserId);

            if (!asksForHimself && !isAdmin)
            {
                return Forbid();
            }

            plantToUpdate.Name = plantPut.Name ?? plantToUpdate.Name;
            plantToUpdate.Species = plantPut.Species ?? plantToUpdate.Species;
            plantToUpdate.Description = plantPut.Description ?? plantToUpdate.Description;
            plantToUpdate.Name = plantPut.Name ?? plantToUpdate.Name;

            plantToUpdate.MoistureLowLimit = plantPut.MoistureLowLimit ?? plantToUpdate.MoistureLowLimit;
            plantToUpdate.MoistureHighLimit = plantPut.MoistureHighLimit ?? plantToUpdate.MoistureHighLimit;
            plantToUpdate.TemperatureLowLimit = plantPut.TemperatureLowLimit ?? plantToUpdate.TemperatureLowLimit;
            plantToUpdate.TemperatureHighLimit = plantPut.TemperatureHighLimit ?? plantToUpdate.TemperatureHighLimit;
            plantToUpdate.LightIntensityLowLimit = plantPut.LightIntensityLowLimit ?? plantToUpdate.LightIntensityLowLimit;
            plantToUpdate.LightIntensityHighLimit = plantPut.LightIntensityHighLimit ?? plantToUpdate.LightIntensityHighLimit;

            var updated = await plantsRepository.UpdatePlant(plantToUpdate);

            return Ok(mapper.Map<GetPlantDto>(plantToUpdate));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlant(int id)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var plantToDelete = await plantsRepository.GetById(id);

            if (plantToDelete == null)
            {
                return NotFound();
            }

            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, plantToDelete.UserId);

            if (!asksForHimself && !isAdmin)
            {
                return Forbid();
            }

            var deleted = await plantsRepository.DeletePlant(plantToDelete);

            return Ok();
        }
    }
}

