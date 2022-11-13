using System;
using AutoMapper;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.DTOs;
using HousePlantMeasurementsApi.DTOs.User;
using HousePlantMeasurementsApi.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HousePlantMeasurementsApi.Controllers
{
    [ApiController]
    [Route("/api/v1/Users")]
    public class UsersController : ControllerBase
    {
        public IConfiguration AppConfiguration { get; set; }
        private readonly ILogger Logger;
        private readonly IMapper Mapper;
        private readonly IUsersRepository UsersRepository;

        public UsersController(IConfiguration configuration, ILogger<AuthController> logger, IMapper mapper, IUsersRepository usersRepository)
        {
            this.AppConfiguration = configuration;
            this.Logger = logger;
            this.Mapper = mapper;
            this.UsersRepository = usersRepository;
        }






        [HttpGet(Name = "List")]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> UsersList()
        {
            var users = await UsersRepository.GetAllUsers();

            return Ok(Mapper.Map<IEnumerable<GetUserDto>>(users));
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<ActionResult<GetUserDto>> GetById(int id)
        {
            var user = await UsersRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<GetUserDto>(user));
        }

        [HttpPost(Name = "Register")]
        public async Task<ActionResult<GetUserDto>> Register(PostUserDto userPost)
        {
            User? newUser = null;
            try
            {
                newUser = Mapper.Map<User>(userPost);
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"User registration not successfull: {ex.ToString}");
                return BadRequest();
            }

            var saved = await UsersRepository.AddUser(newUser);

            return Ok(Mapper.Map<GetUserDto>(newUser));
        }


        [HttpPut(Name = "Update")]
        public async Task<ActionResult<GetUserDto>> UpdateUser(PutUserDto userPut)
        {
            User? userToUpdate = await UsersRepository.GetById(userPut.Id);
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

            var updated = await UsersRepository.UpdateUser(userToUpdate);

            return Ok(Mapper.Map<GetUserDto>(userToUpdate));

        }


        [HttpDelete("{id}", Name = "Delete")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            User? userToDelete = await UsersRepository.GetById(id);
            if (userToDelete == null)
            {
                return NotFound();
            }

            var deleted = await UsersRepository.DeleteUser(userToDelete);

            return Ok();
        }
    }
}

