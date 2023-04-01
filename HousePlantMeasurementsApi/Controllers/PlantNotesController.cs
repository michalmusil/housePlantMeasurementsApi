using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Plant;
using HousePlantMeasurementsApi.DTOs.PlantNotes;
using HousePlantMeasurementsApi.Repositories.PlantNotes;
using HousePlantMeasurementsApi.Repositories.Plants;
using HousePlantMeasurementsApi.Services.AuthService;
using HousePlantMeasurementsApi.Services.ImageService;
using HousePlantMeasurementsApi.Services.ValidationHelperService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HousePlantMeasurementsApi.Controllers
{
	[Authorize]
	[ApiController]
	[Route("/api/v1/plants/notes")]
	public class PlantNotesController: ControllerBase
	{
		private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IAuthService authService;
		private readonly IPlantNotesRepository plantNotesRepository;
        private readonly IPlantsRepository plantsRepository;

        public PlantNotesController(
            ILogger<PlantNotesController> logger,
            IMapper mapper,
			IAuthService authService,
            IPlantNotesRepository plantNotesRepository,
            IPlantsRepository plantsRepository)
		{
			this.logger = logger;
			this.mapper = mapper;
			this.authService = authService;
			this.plantNotesRepository = plantNotesRepository;
			this.plantsRepository = plantsRepository;
		}

		[HttpGet("plant/{plantId}")]
		public async Task<ActionResult<IEnumerable<GetPlantNoteDto>>> GetNotesOfPlant(int plantId)
		{
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var foundPlant = await plantsRepository.GetById(plantId);

			if(foundPlant == null)
			{
				return NotFound("No plant with this id");
			}

			var asksForHimself = await authService.SignedUserHasId(HttpContext.User, foundPlant.UserId);

			if(!isAdmin && !asksForHimself)
			{
				return Forbid();
            }

			var plantNotes = await plantNotesRepository.GetByPlantId(foundPlant.Id);

			return Ok(mapper.Map<IEnumerable<GetPlantNoteDto>>(plantNotes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetPlantNoteDto>> GetById(int id)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var foundPlantNote = await plantNotesRepository.GetById(id);

            if (foundPlantNote == null)
            {
                return NotFound("No plant note with this id");
            }

            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, foundPlantNote.Plant.UserId);

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }

            return Ok(mapper.Map<GetPlantNoteDto>(foundPlantNote));
        }

        [HttpPost]
        public async Task<ActionResult<GetPlantNoteDto>> AddNewPlantNote(PostPlantNoteDto plantNotePost)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var foundPlant = await plantsRepository.GetById(plantNotePost.PlantId);

            if (foundPlant == null)
            {
                return NotFound("No plant with this id");
            }

            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, foundPlant.UserId);

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }

            PlantNote? newPlantNote = null;
            try
            {
                newPlantNote = mapper.Map<PlantNote>(plantNotePost);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Creating a plant note not successfull: {ex.ToString()}");
                return BadRequest();
            }

            var savedPlantNote = await plantNotesRepository.AddPlantNote(newPlantNote);

            if (savedPlantNote == null)
            {
                logger.LogInformation($"Saving a new plant note has failed");
                return BadRequest();
            }

            return Ok(mapper.Map<GetPlantNoteDto>(savedPlantNote));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlantNote(int id)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var plantNoteToDelete = await plantNotesRepository.GetById(id);

            if (plantNoteToDelete == null)
            {
                return NotFound();
            }

            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, plantNoteToDelete.Plant.UserId);

            if (!asksForHimself && !isAdmin)
            {
                return Forbid();
            }

            var deleted = await plantNotesRepository.DeletePlantNote(plantNoteToDelete);

            return Ok();
        }
    }
}

