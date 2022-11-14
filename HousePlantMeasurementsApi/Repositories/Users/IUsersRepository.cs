using System;
using HousePlantMeasurementsApi.Data.Entities;

namespace HousePlantMeasurementsApi.Repositories.Users
{
    public interface IUsersRepository
    {
        public Task<IEnumerable<User>> GetAllUsers();

        public Task<User> GetById(int id);

        public Task<User> GetByEmail(string email);

        public Task<bool> AddUser(User user);

        public Task<bool> UpdateUser(User user);

        public Task<bool> DeleteUser(User user);
    }
}

