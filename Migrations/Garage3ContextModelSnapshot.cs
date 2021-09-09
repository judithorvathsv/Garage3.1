﻿// <auto-generated />
using System;
using Garage3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Garage3.Migrations
{
    [DbContext(typeof(Garage3Context))]
    partial class Garage3ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Garage3.Models.Owner", b =>
                {
                    b.Property<int>("OwnerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.Property<string>("SocialSecurityNumber")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.HasKey("OwnerId");

                    b.ToTable("Owner");
                });

            modelBuilder.Entity("Garage3.Models.ParkingEvent", b =>
                {
                    b.Property<int>("ParkingPlaceId")
                        .HasColumnType("int");

                    b.Property<int>("VehicleId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeOfArrival")
                        .HasColumnType("datetime2");

                    b.HasKey("ParkingPlaceId", "VehicleId");

                    b.HasIndex("VehicleId");

                    b.ToTable("ParkingEvent");
                });

            modelBuilder.Entity("Garage3.Models.ParkingPlace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsOccupied")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("ParkingPlace");
                });

            modelBuilder.Entity("Garage3.Models.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<string>("RegistrationNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("VehicleModel")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("VehicleTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("VehicleTypeId");

                    b.ToTable("Vehicle");
                });

            modelBuilder.Entity("Garage3.Models.VehicleType", b =>
                {
                    b.Property<int>("VehicleTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.HasKey("VehicleTypeId");

                    b.ToTable("VehicleType");
                });

            modelBuilder.Entity("Garage3.Models.ParkingEvent", b =>
                {
                    b.HasOne("Garage3.Models.ParkingPlace", "ParkingPlace")
                        .WithMany("ParkingEvents")
                        .HasForeignKey("ParkingPlaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Garage3.Models.Vehicle", "Vehicle")
                        .WithMany("ParkingEvents")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParkingPlace");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("Garage3.Models.Vehicle", b =>
                {
                    b.HasOne("Garage3.Models.Owner", "Owner")
                        .WithMany("Vehicles")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Garage3.Models.VehicleType", "VehicleType")
                        .WithMany()
                        .HasForeignKey("VehicleTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");

                    b.Navigation("VehicleType");
                });

            modelBuilder.Entity("Garage3.Models.Owner", b =>
                {
                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("Garage3.Models.ParkingPlace", b =>
                {
                    b.Navigation("ParkingEvents");
                });

            modelBuilder.Entity("Garage3.Models.Vehicle", b =>
                {
                    b.Navigation("ParkingEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
