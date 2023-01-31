using System;
using HousePlantMeasurementsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HousePlantMeasurementsApi.Data
{
    public class PlantMeasurementsDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Measurement> Measurements { get; set; }

        public PlantMeasurementsDbContext(DbContextOptions<PlantMeasurementsDbContext> options)
            : base(options)
        {
        }


    }
}

