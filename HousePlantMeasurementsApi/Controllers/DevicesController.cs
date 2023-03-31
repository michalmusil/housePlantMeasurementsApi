using System;
using AutoMapper;
using HouseDeviceMeasurementsApi.Repositories.Devices;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs.Device;
using HousePlantMeasurementsApi.DTOs.Plant;
using HousePlantMeasurementsApi.Repositories.Plants;
using HousePlantMeasurementsApi.Repositories.Users;
using HousePlantMeasurementsApi.Services.AuthService;
using HousePlantMeasurementsApi.Services.HashService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HousePlantMeasurementsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/v1/devices")]
    public class DevicesController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IUsersRepository usersRepository;
        private readonly IDevicesRepository devicesRepository;
        private readonly IPlantsRepository plantsRepository;
        private readonly IAuthService authService;
        private readonly IHashService hashService;

        public DevicesController(
            ILogger<DevicesController> logger,
            IMapper mapper,
            IPlantsRepository plantsRepository,
            IUsersRepository usersRepository,
            IDevicesRepository devicesRepository,
            IAuthService authService,
            IHashService hashService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.usersRepository = usersRepository;
            this.devicesRepository = devicesRepository;
            this.plantsRepository = plantsRepository;
            this.authService = authService;
            this.hashService = hashService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDeviceDto>>> GetAllDevices([FromQuery] bool? registered)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);

            if (!isAdmin)
            {
                return StatusCode(403, "This endpoint is restricted for admin users only");
            }

            var devices = await devicesRepository.GetAllDevices(registered);

            return Ok(mapper.Map<IEnumerable<GetDeviceDto>>(devices));
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GetDeviceDto>>> GetAllDevicesOfUser(int userId)
        {
            var ownerOfDevices = await usersRepository.GetById(userId);

            if (ownerOfDevices == null)
            {
                return NotFound();
            }

            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, userId);

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }

            var devices = await devicesRepository.GetByUserId(userId);

            return Ok(mapper.Map<IEnumerable<GetDeviceDto>>(devices));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetDeviceDto>> GetDeviceById(int id)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var foundDevice = await devicesRepository.GetById(id);

            if (foundDevice == null)
            {
                return NotFound();
            }

            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, foundDevice.UserId ?? -1);

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }   

            return Ok(mapper.Map<GetDeviceDto>(foundDevice));
        }

        [HttpPost]
        public async Task<ActionResult<GetDeviceDto>> PostNewDevice(PostDeviceDto devicePost)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);

            if (!isAdmin)
            {
                return StatusCode(403, "This endpoint is restricted for admin users only");
            }

            var communicationIdentifierHash = hashService.HashCommunicationIdentifier(devicePost.CommunicationIdentifier);

            var deviceWithNewCommunicationIdentifier = await devicesRepository.GetByCommunicationIdentifierHash(communicationIdentifierHash);

            if (deviceWithNewCommunicationIdentifier != null)
            {
                return Conflict("Device with this CommunicationIdentifier already exists");
            }

            var macHash = hashService.HashMacAddress(devicePost.MacAddress);

            Device newDevice = null;

            try
            {
                newDevice = mapper.Map<Device>(devicePost);
                newDevice.CommunicationIdentifier = communicationIdentifierHash;
                newDevice.MacAddress = macHash;
                var savedDevice = await devicesRepository.AddDevice(newDevice);

                if(savedDevice == null)
                {
                    logger.LogInformation($"Saving a new device has failed");
                    return BadRequest();
                }

                return Ok(mapper.Map<GetDeviceDto>(savedDevice));
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Creating a new device has failed: {ex.ToString()}");
                return BadRequest();
            }
        }

        [HttpPut("register")]
        public async Task<ActionResult<GetDeviceDto>> RegisterDevice(PostRegisterDeviceDto registerObject)
        {
            //Returning generic BadRequest for all failures to minimize feedback on attempts to brute force register devices
            var communicationIdentifierHash = hashService.HashCommunicationIdentifier(registerObject.CommunicationIdentifier);
            var foundDevice = await devicesRepository.GetByCommunicationIdentifierHash(communicationIdentifierHash);

            if (foundDevice == null)
            {
                //Device with this CommunicationIdentifier does not exist
                return BadRequest();
            }

            var macAddressAuthentic = hashService.VerifyMacAddress(registerObject.MacAddress, foundDevice.MacAddress);

            if (!macAddressAuthentic)
            {
                //MAC address of the request is not valid - device not authenticated
                return Ok();
            } 

            if (foundDevice.UserId != null)
            {
                //This device has already been registered
                return BadRequest();
            }

            var registeringUserId = await authService.GetSignedUserId(HttpContext.User);

            if (registeringUserId == null)
            {
                //Could not determine users identity
                return BadRequest();
            }

            foundDevice.UserId = registeringUserId;
            foundDevice.IsActive = true;
            foundDevice.PlantId = null;

            var updated = await devicesRepository.UpdateDevice(foundDevice);

            return Ok(mapper.Map<GetDeviceDto>(foundDevice));
        }


        [HttpPut("activation")]
        public async Task<ActionResult<GetDeviceDto>> DeviceActivation(PostDeviceActivationDto activationObject)
        {
            var foundDevice = await devicesRepository.GetById(activationObject.DeviceId);

            if (foundDevice == null)
            {
                return NotFound("Device with this id does not exist");
            }

            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, foundDevice.UserId ?? -1);

            if (!isAdmin && !asksForHimself)
            {
                return StatusCode(403, "This device can only be activated by its owner or admin user");
            }

            foundDevice.IsActive = activationObject.IsActive;

            var updated = await devicesRepository.UpdateDevice(foundDevice);

            return Ok(mapper.Map<GetDeviceDto>(foundDevice));
        }

        [HttpPut("assignToPlant")]
        public async Task<ActionResult<GetDeviceDto>> AssignDeviceToPlant(PostAssignToPlantDto assignObject)
        {
            var foundDevice = await devicesRepository.GetById(assignObject.DeviceId);

            if (foundDevice == null)
            {
                return NotFound("Device with this id does not exist");
            }

            var plantToAssign = await plantsRepository.GetById(assignObject.PlantId);

            if (plantToAssign == null)
            {
                return NotFound("Plant with this id does not exist");
            }

            if (foundDevice.UserId != plantToAssign.UserId)
            {
                return StatusCode(403, "Device must have same owner as the plant it's being assigned to");
            }

            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var isOwnerOfDevice = await authService.SignedUserHasId(HttpContext.User, foundDevice.UserId ?? -1);
            var isOwnerOfPlant = await authService.SignedUserHasId(HttpContext.User, plantToAssign.UserId);

            if (!isAdmin && (!isOwnerOfDevice || !isOwnerOfPlant))
            {
                return StatusCode(403, "You lack the ownership of either plant or device you are trying to assign");
            }

            foundDevice.PlantId = plantToAssign.Id;

            var updated = await devicesRepository.UpdateDevice(foundDevice);

            return Ok(mapper.Map<GetDeviceDto>(foundDevice));
        }

        [HttpPut("unregister/{id}")]
        public async Task<ActionResult> UnregisterDevice(int id)
        {
            var deviceToUnregister = await devicesRepository.GetById(id);
            if (deviceToUnregister == null)
            {
                return NotFound();
            }

            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.ADMIN);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, deviceToUnregister.UserId ?? -1);

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }

            deviceToUnregister.IsActive = false;
            deviceToUnregister.PlantId = null;
            deviceToUnregister.UserId = null;

            var updated = await devicesRepository.UpdateDevice(deviceToUnregister);

            return Ok();
        }

    }

}

