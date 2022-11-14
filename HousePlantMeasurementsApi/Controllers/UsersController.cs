using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs;
using HousePlantMeasurementsApi.DTOs.User;
using HousePlantMeasurementsApi.Repositories.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HousePlantMeasurementsApi.Controllers
{
    [ApiController]
    [Route("/api/v1/Users")]
    public class UsersController : ControllerBase
    {
        public IConfiguration appConfiguration { get; set; }
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly IUsersRepository usersRepository;

        public UsersController(IConfiguration configuration, ILogger<AuthController> logger, IMapper mapper, IUsersRepository usersRepository)
        {
            this.appConfiguration = configuration;
            this.logger = logger;
            this.mapper = mapper;
            this.usersRepository = usersRepository;
        }





        [Authorize]
        [HttpGet(Name = "List")]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> UsersList()
        {
            var users = await usersRepository.GetAllUsers();

            return Ok(mapper.Map<IEnumerable<GetUserDto>>(users));
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetById")]
        public async Task<ActionResult<GetUserDto>> GetById(int id)
        {
            var user = await usersRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<GetUserDto>(user));
        }

        [HttpPost("Register", Name = "Register")]
        public async Task<ActionResult<GetUserDto>> Register(PostUserDto userPost)
        {
            var alreadyExistingUser = await usersRepository.GetByEmail(userPost.Email);

            if (alreadyExistingUser != null)
            {
                return Conflict();
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

        [Authorize]
        [HttpPut(Name = "Update")]
        public async Task<ActionResult<GetUserDto>> UpdateUser(PutUserDto userPut)
        {
            User? userToUpdate = await usersRepository.GetById(userPut.Id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            if (userPut.Email != null && userPut.Email.Length > 0)
            {
                userToUpdate.Email = userPut.Email;
            }
            if (userPut.Password != null && userPut.Password.Length > 0)
            {
                userToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(userPut.Password);
            }

            var updated = await usersRepository.UpdateUser(userToUpdate);

            return Ok(mapper.Map<GetUserDto>(userToUpdate));

        }

        [Authorize]
        [HttpDelete("{id}", Name = "Delete")]
        public async Task<ActionResult> DeleteUser(int id)
        {
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

