using System;
using HousePlantMeasurementsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace HousePlantMeasurementsApi.Data
{
    public class PlantMeasurementsDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<MeasurementValueLimit> MeasurementValueLimits { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<MeasurementValue> MeasurementValues { get; set; }

        public PlantMeasurementsDbContext(DbContextOptions<PlantMeasurementsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MeasurementValue>()
                .HasOne(v => v.Measurement)
                .WithMany(m => m.MeasurementValues)
                .HasForeignKey(v => v.MeasurementId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MeasurementValueLimit>()
                .HasOne(l => l.Plant)
                .WithMany(p => p.MeasurementValueLimits)
                .HasForeignKey(l => l.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Measurement>()
                .HasOne(m => m.Plant)
                .WithMany(p => p.Measurements)
                .HasForeignKey(m => m.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Plant>()
                .HasOne(p => p.User)
                .WithMany(u => u.Plants)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

