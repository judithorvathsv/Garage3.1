﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Garage3.Models;

namespace Garage3.Data
{
    public class Garage3Context : DbContext
    {
        public Garage3Context (DbContextOptions<Garage3Context> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicle { get; set; }
        public DbSet<ParkingPlace> ParkingPlace { get; set; }
        public DbSet<Owner> Owner { get; set; }
        public DbSet<VehicleType> VehicleType { get; set; }
        public DbSet<ParkingEvent> ParkingEvent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.ParkingPlaces)
                .WithMany(pp => pp.Vehicles)
                .UsingEntity<ParkingEvent>(
                pe => pe.HasOne(pe => pe.ParkingPlace).WithMany(v => v.ParkingEvents),
                pe => pe.HasOne(pe => pe.Vehicle).WithMany(pp => pp.ParkingEvents));

            modelBuilder.Entity<ParkingEvent>().HasKey(x => new { x.ParkingPlaceId, x.VehicleId });
        }
    }
}
