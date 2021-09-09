﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Garage3.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SocialSecurityNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => x.OwnerId);
                });

            migrationBuilder.CreateTable(
                name: "ParkingPlace",
                columns: table => new
                {
                    ParkingPlaceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsOccupied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingPlace", x => x.ParkingPlaceId);
                });

            migrationBuilder.CreateTable(
                name: "VehicleType",
                columns: table => new
                {
                    VehicleTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleType", x => x.VehicleTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VehicleModel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    VehicleTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicle_Owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owner",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vehicle_VehicleType_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "VehicleType",
                        principalColumn: "VehicleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParkingEvent",
                columns: table => new
                {
                    ParkingPlaceId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    TimeOfArrival = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingEvent", x => new { x.ParkingPlaceId, x.VehicleId });
                    table.ForeignKey(
                        name: "FK_ParkingEvent_ParkingPlace_ParkingPlaceId",
                        column: x => x.ParkingPlaceId,
                        principalTable: "ParkingPlace",
                        principalColumn: "ParkingPlaceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParkingEvent_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Owner",
                columns: new[] { "OwnerId", "FirstName", "LastName", "SocialSecurityNumber" },
                values: new object[,]
                {
                    { 1, "Isaac", "Newton", "600102-1478" },
                    { 16, "Josef", "Jacobsson", "987654-3210" },
                    { 15, "Joel", "Abelin", "345678-9874" },
                    { 14, "Joel", "Josefsson", "234567-1234" },
                    { 12, "James", "Jones", "123456-7891" },
                    { 11, "Adam", "Abelin", "123456-1234" },
                    { 10, "Thomas", "Edison", "690102-7535" },
                    { 9, "Alexander", "Fleming", "680102-1595" },
                    { 13, "joel", "Viklund", "134679-2587" },
                    { 7, "Nicolaus", "Copernicus", "660102-4568" },
                    { 6, "Charles", "Darwin", "650102-1235" },
                    { 5, "Galileo", "Galilei", "640102-4561" },
                    { 4, "Marie", "Curie", "630102-7894" },
                    { 3, "Stephen", "Hawking", "620102-4567" },
                    { 2, "Albert", "Einstein", "610102-1234" },
                    { 8, "Louis", "Pasteur", "670102-7895" }
                });

            migrationBuilder.InsertData(
                table: "VehicleType",
                columns: new[] { "VehicleTypeId", "Size", "Type" },
                values: new object[,]
                {
                    { 8, 1, "Kayak" },
                    { 7, 1, "Canoe" },
                    { 6, 9, "Boat" },
                    { 5, 6, "Van" },
                    { 2, 6, "Truck" },
                    { 3, 6, "Bus" },
                    { 1, 3, "Car" },
                    { 9, 9, "Airplane" },
                    { 4, 1, "Motorcycle" },
                    { 10, 9, "Helicopter" }
                });

            migrationBuilder.InsertData(
                table: "Vehicle",
                columns: new[] { "VehicleId", "Brand", "OwnerId", "RegistrationNumber", "VehicleModel", "VehicleTypeId" },
                values: new object[,]
                {
                    { 1, "Chevrolet", 1, "ABC-123", "Silverado", 1 },
                    { 2, "Toyota", 2, "BCD-123", "RAV4", 1 },
                    { 3, "Honda", 3, "CDE-456", "Accord", 1 },
                    { 4, "Ford", 4, "DEF-456", "Explorer", 1 },
                    { 5, "Subaru", 5, "EFG-456", "Impreza", 1 },
                    { 7, "Kia", 6, "FGH-789", "Stinger", 1 },
                    { 8, "Hyundai", 7, "GHI-9512", "Veloster", 1 },
                    { 9, "Nissan", 8, "HIJ-7532", "Versa", 1 },
                    { 10, "Volvo", 9, "IJK-456", "XC40", 1 },
                    { 11, "BMW", 10, "JKL-654", "X5", 1 },
                    { 12, "BMW", 11, "KLM-864", "i3", 1 },
                    { 13, "Honda", 1, "LMN-246", "Civic", 1 },
                    { 14, "Saab", 2, "MNO-931", "AreoX", 1 },
                    { 16, "Yamaha", 4, "AAB-123", "VMAX", 4 },
                    { 15, "Boeing", 3, "N12345", "777", 9 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingEvent_VehicleId",
                table: "ParkingEvent",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_OwnerId",
                table: "Vehicle",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_VehicleTypeId",
                table: "Vehicle",
                column: "VehicleTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParkingEvent");

            migrationBuilder.DropTable(
                name: "ParkingPlace");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.DropTable(
                name: "VehicleType");
        }
    }
}
