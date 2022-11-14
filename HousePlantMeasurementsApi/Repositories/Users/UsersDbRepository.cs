using System;
using HousePlantMeasurementsApi.Data;
using HousePlantMeasurementsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HousePlantMeasurementsApi.Repositories.Users
{
    public class UsersDbRepository : IUsersRepository
    {
        protected readonly PlantMeasurementsDbContext DbContext;

        public UsersDbRepository(PlantMeasurementsDbContext dbContext)
        {
            this.DbContext = dbContext;
        }



        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await DbContext.Users.ToListAsync();

            return users;
        }

        public async Task<User?> GetById(int id)
        {
            var user = await DbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            string emailLower = (email ?? "").ToLower();
            var user = await DbContext.Users.Where(user => user.Email.ToLower() == emailLower).FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> AddUser(User user)
        {
            DbContext.Add(user);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUser(User user)
        {
            user.Updated = DateTime.UtcNow;
            DbContext.Update(user);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUser(User user)
        {
            DbContext.Remove(user);
            return await DbContext.SaveChangesAsync() > 0;
        }
    }
}

