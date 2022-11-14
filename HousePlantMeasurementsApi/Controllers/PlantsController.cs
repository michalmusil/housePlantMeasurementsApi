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
                return Unauthorized();
            }

            var plants = await plantsRepository.GetByUserId(userId);
            return Ok(mapper.Map<IEnumerable<GetPlantDto>>(plants));
        }

        [HttpPost]
        public async Task<ActionResult<GetPlantDto>> AddNewPlant(PostPlantDto plantPost)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, plantPost.UserId);

            User? newUser = null;
            try
            {
                newUser = mapper.Map<User>(userPost);
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"User registration not successfull: {ex.ToString}");
                return BadRequest();
            }

            var saved = await usersRepository.AddUser(newUser);

            return Ok(mapper.Map<GetUserDto>(newUser));
        }

    }
}

