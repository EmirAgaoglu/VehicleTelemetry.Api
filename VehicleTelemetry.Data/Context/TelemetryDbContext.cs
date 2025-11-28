using Microsoft.EntityFrameworkCore;
using VehicleTelemetry.Domain.Entities;

namespace VehicleTelemetry.Data.Context
{
    public class TelemetryDbContext : DbContext
    {
        public TelemetryDbContext(DbContextOptions<TelemetryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<TelemetryRecord> TelemetryRecords => Set<TelemetryRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Vehicle konfigurasyonu
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.HasIndex(v => v.PlateNumber).IsUnique();

                entity.Property(v => v.PlateNumber)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(v => v.Manufacturer)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(v => v.Model)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(v => v.DriverName)
                    .HasMaxLength(128);
            });

            // TelemetryRecord konfigurasyonu
            modelBuilder.Entity<TelemetryRecord>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasOne(t => t.Vehicle)
                    .WithMany(v => v.TelemetryRecords)
                    .HasForeignKey(t => t.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(t => t.TimestampUtc)
                    .IsRequired();
            });
        }
    }
}
