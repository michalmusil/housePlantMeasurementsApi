using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Plant;
using HousePlantMeasurementsApi.DTOs.User;
using HousePlantMeasurementsApi.Repositories.Plants;
using HousePlantMeasurementsApi.Repositories.Users;
using HousePlantMeasurementsApi.Services.AuthService;
using HousePlantMeasurementsApi.Services.ImageService;
using HousePlantMeasurementsApi.Services.ValidationHelperService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;

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
        private readonly IImageService imageService;
        private readonly IMeasurementValidator measurementValidator;

        public PlantsController(
            ILogger<PlantsController> logger,
            IMapper mapper,
            IPlantsRepository plantsRepository,
            IUsersRepository usersRepository,
            IAuthService authService,
            IImageService imageService,
            IMeasurementValidator measurementValidator)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.plantsRepository = plantsRepository;
            this.usersRepository = usersRepository;
            this.authService = authService;
            this.imageService = imageService;
            this.measurementValidator = measurementValidator;
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GetPlantDto>>> GetAllPlantsOfUser(int userId)
        {
            var ownerOfPlants = await usersRepository.GetById(userId);

            if (ownerOfPlants == null)
            {
                return NotFound();
            }

            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
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
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
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
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, plantPost.UserId);

            var plantOwner = await usersRepository.GetById(plantPost.UserId);

            if (plantOwner == null)
            {
                return NotFound(new { message = $"User with specified id was not found" });
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
                logger.LogInformation($"Creating a plant not successfull: {ex.ToString()}");
                return BadRequest();
            }

            if(newPlant.MeasurementValueLimits.Count > 0 &&
                !measurementValidator.AreMeasurementLimitsValid(newPlant.MeasurementValueLimits))
            {
                return BadRequest();
            }

            var savedPlant = await plantsRepository.AddPlant(newPlant);

            if(savedPlant == null)
            {
                logger.LogInformation($"Saving a new plant has failed");
                return BadRequest();
            }

            return Ok(mapper.Map<GetPlantDto>(savedPlant));
        }

        [HttpPut]
        public async Task<ActionResult<GetPlantDto>> UpdatePlant(PutPlantDto plantPut)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
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


            // Updating measurement value limits of this plant
            var mappedPut = mapper.Map<Plant>(plantPut);
            if( mappedPut != null &&
                mappedPut.MeasurementValueLimits != null)
            {
                if (!measurementValidator.AreMeasurementLimitsValid(mappedPut.MeasurementValueLimits))
                {
                    return BadRequest("Measurement limits are not valid");
                }
                var limitsRemoved =  await plantsRepository.RemoveLimitsOfPlant(plantToUpdate);
                plantToUpdate.MeasurementValueLimits = mappedPut.MeasurementValueLimits;
            }

            var updated = await plantsRepository.UpdatePlant(plantToUpdate);

            return Ok(mapper.Map<GetPlantDto>(plantToUpdate));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlant(int id)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var plantToDelete = await plantsRepository.GetById(id);
            var plantImageName = plantToDelete.TitleImagePath;

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

            if(deleted && plantImageName != null)
            {
                var imageDeleted = await imageService.RemoveImageFromFileSystem(plantImageName);
            }

            return Ok();
        }





        [HttpGet("images/{plantId}", Name = "GetPlantImage")]
        public async Task<ActionResult> GetPlantImage(int plantId)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var plant = await plantsRepository.GetById(plantId);

            if (plant == null)
            {
                return NotFound();
            }

            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, plant.UserId);

            if (!asksForHimself && !isAdmin)
            {
                return Forbid();
            }

            if (plant.TitleImagePath == null)
            {
                return NotFound();
            }

            var image = await imageService.GetImageFromPath(plant.TitleImagePath);
            var contentType = await imageService.GetImageContentType(plant.TitleImagePath);

            if (image == null || contentType == null)
            {
                return NotFound();
            }

            return File(image, contentType);
        }

        [HttpPut("images")]
        public async Task<ActionResult> SetPlantTitleImage(int plantId, IFormFile image)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var plant = await plantsRepository.GetById(plantId);

            if (plant == null)
            {
                return NotFound();
            }

            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, plant.UserId);
            var oldImageName = plant.TitleImagePath;

            if (!asksForHimself && !isAdmin)
            {
                return Forbid();
            }

            try
            {
                var imageName = await imageService.SaveImageToFileSystem(image);
                plant.TitleImagePath = imageName;
                var updated = await plantsRepository.UpdatePlant(plant);

                if(oldImageName != null)
                {
                    var oldDeleted = imageService.RemoveImageFromFileSystem(oldImageName);
                }

                return Ok(mapper.Map<GetPlantDto>(plant));
            }
            catch(Exception ex)
            {
                return BadRequest("Could not save image");
            }
        }
    }
}

