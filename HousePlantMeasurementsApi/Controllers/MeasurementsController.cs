using System;
using System.Diagnostics.Metrics;
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
using HousePlantMeasurementsApi.Services.ValidationHelperService;
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
        private readonly IMeasurementValidator measurementValidator;

        public MeasurementsController(
            ILogger<PlantsController> logger,
            IMapper mapper,
            IMeasurementsRepository measurementsRepository,
            IDevicesRepository devicesRepository,
            IPlantsRepository plantsRepository,
            IAuthService authService,
            IMeasurementValidator measurementValidator)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.measurementsRepository = measurementsRepository;
            this.devicesRepository = devicesRepository;
            this.plantsRepository = plantsRepository;
            this.authService = authService;
            this.measurementValidator = measurementValidator;
        }

        [HttpGet("plant/{plantId}")]
        public async Task<ActionResult<IEnumerable<GetMeasurementDto>>> GetAllMeasurementsOfPlant(int plantId)
        {
            var plant = await plantsRepository.GetById(plantId);

            if (plant == null)
            {
                return NotFound();
            }

            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
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
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);

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

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<GetMeasurementTypeDto>>> GetMeasurementTypes()
        {
            var measurementTypes = Enum.GetValues(typeof(MeasurementType));

            return Ok(mapper.Map<IEnumerable<GetMeasurementTypeDto>>(measurementTypes));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<GetPlantDto>> PostNewMeasurement(PostMeasurementDto measurementPost)
        {
            //Response is always 200 OK - so that malicious user can't get feedback on brute force guessing
            var foundDevice = await devicesRepository.GetByUUID(measurementPost.DeviceUUID);

            if (foundDevice == null)
            {
                //No device with this UUID was found
                return Ok();
            }

            var deviceAuth = authService.GetDeviceAuthHashBase(measurementPost.DeviceMacAddress);
            var isAuthenticated = BCrypt.Net.BCrypt.Verify(deviceAuth, foundDevice.AuthHash);

            if (!isAuthenticated)
            {
                //MAC address of the request is not valid - device not authenticated
                return Ok();
            }

            if (!foundDevice.IsActive)
            {
                //Measurement accepted but not saved - sender measuring device is deactivated
                return Ok();
            }

            var plant = await plantsRepository.GetById(foundDevice.PlantId ?? -1);

            if (foundDevice.UserId == null || plant == null)
            {
                //Device is not assigned to an existing plant
                return Ok();
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
                //Measurement has not been parsed properly
                return Ok();
            }

            if (!measurementValidator.IsMeasurementValid(newMeasurement, plant))
            {
                //Measurement accepted but not saved - measurement is invalid
                return Ok();
            }

            var saved = await measurementsRepository.AddMeasurement(newMeasurement);
            return Ok(mapper.Map<GetMeasurementDto>(newMeasurement));
        }

    }
}

