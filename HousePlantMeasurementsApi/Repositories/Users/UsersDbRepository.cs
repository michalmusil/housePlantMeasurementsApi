using System;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HousePlantMeasurementsApi.Repositories.Users
{
    public class UsersDbRepository : IUsersRepository
    {
        protected readonly PlantMeasurementsDbContext dbContext;

        public UsersDbRepository(PlantMeasurementsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }



        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await dbContext.Users.ToListAsync();

            return users;
        }

        public async Task<User?> GetById(int id)
        {
            var user = await dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            string emailLower = (email ?? "").ToLower();
            var user = await dbContext.Users.Where(user => user.Email.ToLower() == emailLower).FirstOrDefaultAsync();

            return user;
        }

        public async Task<User?> AddUser(User user)
        {
            dbContext.Add(user);
            var savedSuccessfully = await dbContext.SaveChangesAsync() > 0;
            if (savedSuccessfully)
            {
                return user;
            }
            return null;
        }

        public async Task<bool> UpdateUser(User user)
        {
            user.Updated = DateTime.UtcNow;
            dbContext.Update(user);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUser(User user)
        {
            var assignedDevices = await dbContext.Devices
                .Where(d => d.UserId == user.Id)
                .ToListAsync();
            if (assignedDevices != null)
            {
                assignedDevices.ForEach(d => d.UserId = null);
            }
            dbContext.Remove(user);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}

