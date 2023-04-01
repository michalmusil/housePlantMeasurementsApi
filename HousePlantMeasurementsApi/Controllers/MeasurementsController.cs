using System;
using System.Diagnostics.Metrics;
using AutoMapper;
using HouseDeviceMeasurementsApi.Repositories.Devices;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Measurement;
using HousePlantMeasurementsApi.DTOs.MeasurementValue;
using HousePlantMeasurementsApi.DTOs.Plant;
using HousePlantMeasurementsApi.Repositories.Measurements;
using HousePlantMeasurementsApi.Repositories.Plants;
using HousePlantMeasurementsApi.Repositories.Users;
using HousePlantMeasurementsApi.Services.AuthService;
using HousePlantMeasurementsApi.Services.FCMService;
using HousePlantMeasurementsApi.Services.HashService;
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
        private readonly IUsersRepository usersRepository;
        private readonly IAuthService authService;
        private readonly IHashService hashService;
        private readonly IMeasurementValidator measurementValidator;
        private readonly IFCMService fcmService;

        public MeasurementsController(
            ILogger<PlantsController> logger,
            IMapper mapper,
            IMeasurementsRepository measurementsRepository,
            IDevicesRepository devicesRepository,
            IPlantsRepository plantsRepository,
            IUsersRepository usersRepository,
            IAuthService authService,
            IHashService hashService,
            IMeasurementValidator measurementValidator,
            IFCMService fCMService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.measurementsRepository = measurementsRepository;
            this.devicesRepository = devicesRepository;
            this.plantsRepository = plantsRepository;
            this.usersRepository = usersRepository;
            this.authService = authService;
            this.hashService = hashService;
            this.measurementValidator = measurementValidator;
            this.fcmService = fCMService;
        }

        [HttpGet("plant/{plantId}")]
        public async Task<ActionResult<IEnumerable<GetMeasurementDto>>> GetAllMeasurementsOfPlant(
            int plantId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
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

            var measurements = await measurementsRepository.GetByPlantId(plant.Id, from, to);

            return Ok(mapper.Map<IEnumerable<GetMeasurementDto>>(measurements));
        }

        [HttpGet("plant/latestMeasurement/{plantId}")]
        public async Task<ActionResult<GetMeasurementDto>> GetLatestMeasurementOfPlant(int plantId, [FromQuery] MeasurementType measurementType)
        {
            var plant = await plantsRepository.GetById(plantId);
            var measurementTypeValid = measurementValidator.IsMeasurementTypeValid(measurementType);
           
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

            if (!measurementTypeValid)
            {
                return BadRequest("Measurement type not valid");
            }

            var measurement = await measurementsRepository.GetMostRecentByMeasurementType(plantId, measurementType);

            if (measurement == null)
            {
                return NotFound("No measurement found");
            }

            return Ok(mapper.Map<GetMeasurementDto>(measurement));
        }

        [HttpGet("plant/latestValues/{plantId}")]
        public async Task<ActionResult<IEnumerable<GetLatestMeasurementValueDto>>> GetLatestMeasurementValuesOfPlant(int plantId)
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

            var mostRecentValues = await measurementsRepository.GetMostRecentValuesOfPlant(plantId);

            return Ok(mostRecentValues);
        }

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<GetMeasurementDto>>> GetAllMeasurementsOfDevice(
            int deviceId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
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

            var measurements = await measurementsRepository.GetByDeviceId(device.Id, from, to);

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
            var communicationIdentifierHash = hashService.HashCommunicationIdentifier(measurementPost.DeviceCommunicationIdentifier);
            var foundDevice = await devicesRepository.GetByCommunicationIdentifierHash(communicationIdentifierHash);

            if (foundDevice == null)
            {
                //No device with this CommunicationIdentifier was found
                return Ok();
            }

            var macAddressAuthentic = hashService.VerifyMacAddress(measurementPost.DeviceMacAddress, foundDevice.MacAddress);

            if (!macAddressAuthentic)
            {
                //MAC address of the request is not valid - device not authenticated
                return Ok();
            }  

            var plant = await plantsRepository.GetById(foundDevice.PlantId ?? -1);

            Measurement? newMeasurement = null;
            try
            { 
                newMeasurement = mapper.Map<Measurement>(measurementPost);
                newMeasurement.PlantId = plant.Id;
                newMeasurement.DeviceId = foundDevice.Id;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Creating a new measurement failed: {ex.ToString()}");
                //Measurement has not been parsed properly
                return Ok();
            }

            if (!measurementValidator.IsMeasurementValid(measurement: newMeasurement, plant: plant, device: foundDevice))
            {
                //Measurement accepted but not saved - measurement is invalid
                return Ok();
            }

            var savedMeasurement = await measurementsRepository.AddMeasurement(newMeasurement);

            if(savedMeasurement == null)
            {
                logger.LogInformation($"Saving a new measurement failed");
                return BadRequest();
            }

            var ownerOfMeasurement = await usersRepository.GetById(plant.UserId);

            if(ownerOfMeasurement != null &&
               ownerOfMeasurement.NotificationToken != null &&
               ownerOfMeasurement.NotificationToken.Length > 0 &&
               !measurementValidator.IsMeasurementWithinLimits(savedMeasurement, plant.MeasurementValueLimits))
            {
                var notificationToken = ownerOfMeasurement.NotificationToken;
                var title = "Plant is not feeling well";
                var message = plant.Name + " has surpassed set measurement limits.";

                var sucessfulNotification = await fcmService.SendNotification(
                    notificationToken: notificationToken,
                    title: title,
                    message: message,
                    plantName: plant.Name);
            }

            return Ok(mapper.Map<GetMeasurementDto>(savedMeasurement));
        }

    }
}

