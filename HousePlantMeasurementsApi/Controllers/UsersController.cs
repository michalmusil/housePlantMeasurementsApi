using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;
using HousePlantMeasurementsApi.DTOs;
using HousePlantMeasurementsApi.DTOs.User;
using HousePlantMeasurementsApi.Repositories.Users;
using HousePlantMeasurementsApi.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HousePlantMeasurementsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IUsersRepository usersRepository;
        private readonly IAuthService authService;

        public UsersController(
            ILogger<AuthController> logger,
            IMapper mapper,
            IUsersRepository usersRepository,
            IAuthService authService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.usersRepository = usersRepository;
            this.authService = authService;
        }





        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> UsersList()
        {
            if (!await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin))
            {
                return Unauthorized(new { message = "Endpoint accessible for admin users only" });
            }

            var users = await usersRepository.GetAllUsers();

            return Ok(mapper.Map<IEnumerable<GetUserDto>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDto>> GetById(int id)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, id);

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }

            var user = await usersRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<GetUserDto>(user));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<GetUserDto>> Register(PostUserDto userPost)
        {
            var alreadyExistingUser = await usersRepository.GetByEmail(userPost.Email);

            if (alreadyExistingUser != null)
            {
                return Conflict(new { message = "User with this email already exists" });
            }

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

        [HttpPut]
        public async Task<ActionResult<GetUserDto>> UpdateUser(PutUserDto userPut)
        {
            var userToUpdate = await usersRepository.GetById(userPut.Id);
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, userPut.Id);

            if (userToUpdate == null)
            {
                return NotFound();
            }
            if (!asksForHimself && !isAdmin)
            {
                return Forbid();
            }


            // Catching if non-admin user tries to change his own role - not allowed
            if (userPut.Role == UserRole.User || userPut.Role == UserRole.Admin)
            {
                if (!isAdmin)
                {
                    return Unauthorized(new { message = "Only admin can update users roles" });
                }

                userToUpdate.Role = userPut.Role ?? userToUpdate.Role;
            }

            if (userPut.Email != null && userPut.Email.Length > 0)
            {
                var existingUser = await usersRepository.GetByEmail(userPut.Email);
                if (existingUser != null)
                {
                    return Conflict(new { message = "User with this email already exists" });
                }

                userToUpdate.Email = userPut.Email;
            }
            if (userPut.Password != null && userPut.Password.Length > 0)
            {
                userToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(userPut.Password);
            }

            var updated = await usersRepository.UpdateUser(userToUpdate);

            return Ok(mapper.Map<GetUserDto>(userToUpdate));

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var isAdmin = await authService.SignedUserHasRole(HttpContext.User, UserRole.Admin);
            var asksForHimself = await authService.SignedUserHasId(HttpContext.User, id);

            if (!isAdmin && !asksForHimself)
            {
                return Forbid();
            }

            User? userToDelete = await usersRepository.GetById(id);
            if (userToDelete == null)
            {
                return NotFound();
            }

            var deleted = await usersRepository.DeleteUser(userToDelete);

            return Ok();
        }
    }

}