using System;
using AutoMapper;
using HouseDeviceMeasurementsApi.Repositories.Devices;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Measurement;
using HousePlantMeasurementsApi.DTOs.Plant;
using HousePlantMeasurementsApi.Repositories.Measurements;
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
        private readonly IMeasurementsRepository measurementsRepository;
        private readonly IDevicesRepository devicesRepository;
        private readonly IPlantsRepository plantsRepository;
        private readonly IAuthService authService;

        public MeasurementsController(
            ILogger<PlantsController> logger,
            IMapper mapper,
            IMeasurementsRepository measurementsRepository,
            IDevicesRepository devicesRepository,
            IPlantsRepository plantsRepository,
            IAuthService authService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.measurementsRepository = measurementsRepository;
            this.devicesRepository = devicesRepository;
            this.plantsRepository = plantsRepository;
            this.authService = authService;
        }


        [HttpGet("plant/{plantId}")]
        public async Task<ActionResult<IEnumerable<GetMeasurementDto>>> GetAllMeasurementsOfPlant(int plantId)
        {
            var plant = await plantsRepository.GetById(plantId);

            if (plant == null)
            {
                return NotFound();
            }

            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, plant.UserId);

            if (!isAdmin && !asksForHimself)
            {
                return StatusCode(403, "Only owner of this plant or admin can view its measurements");
            }

            var measurements = await measurementsRepository.GetByPlantId(plant.Id);

            return Ok(mapper.Map<IEnumerable<GetMeasurementDto>>(measurements));
        }

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<GetMeasurementDto>>> GetAllMeasurementsOfDevice(int deviceId)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);

            if (!isAdmin)
            {
                return StatusCode(403, "This endpoint is restricted for admin users only");
            }

            var device = await devicesRepository.GetById(deviceId);

            if (device == null)
            {
                return NotFound();
            }

            var measurements = await measurementsRepository.GetByDeviceId(device.Id);

            return Ok(mapper.Map<IEnumerable<GetMeasurementDto>>(measurements));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<GetPlantDto>> PostNewMeasurement(PostMeasurementDto measurementPost)
        {
            var foundDevice = await devicesRepository.GetByUUID(measurementPost.DeviceUUID);

            if (foundDevice == null)
            {
                return NotFound("No device with this UUID was found");
            }

            var plant = await plantsRepository.GetById(foundDevice.PlantId ?? -1);

            if (foundDevice.UserId == null || plant == null)
            {
                return StatusCode(403, "Device is not assigned to an existing plant");
            }

            if (
                !(measurementPost.Temperature >= plant.TemperatureLowLimit && measurementPost.Temperature <= plant.TemperatureHighLimit &&
                measurementPost.Moisture >= plant.MoistureLowLimit && measurementPost.Moisture <= plant.MoistureHighLimit &&
                measurementPost.LightIntensity >= plant.LightIntensityLowLimit && measurementPost.LightIntensity <= plant.LightIntensityHighLimit)
                )
            {
                return Accepted(new { message = "Measurement accepted but not saved - values out of set boundries" });
            }


            Measurement? newMeasurement = null;
            try
            { 
                newMeasurement = mapper.Map<Measurement>(measurementPost);
                newMeasurement.PlantId = plant.Id;
                newMeasurement.DeviceId = foundDevice.Id;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Could not parse posted measurement: {ex.ToString()}");
                return BadRequest();
            }

            var saved = await measurementsRepository.AddMeasurement(newMeasurement);
            return Ok(mapper.Map<GetMeasurementDto>(newMeasurement));
        }

    }
}

