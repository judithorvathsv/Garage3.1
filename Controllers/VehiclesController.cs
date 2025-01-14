﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage3.Data;
using Garage3.Models;
using Garage3.Models.ViewModels;
using System.Data;

namespace Garage3.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Garage3Context db;

        public VehiclesController(Garage3Context context)
        {
            db = context;
        }

        //Start page where search on reg nr is done
        public IActionResult Index()
        {
            return View();
        }

        // Search for Vehicle
        public async Task<IActionResult> Search(string searchText)
        {
            var exists = db.Vehicle.Any(v => v.RegistrationNumber == searchText);

            if (exists)
            {
                var model = await db.Vehicle.FirstOrDefaultAsync(v => v.RegistrationNumber == searchText);

                return RedirectToAction("Details", new { id = model.VehicleId });
            }
            else
            {
                TempData["Regnumber"] = searchText.ToUpper();

                return View(nameof(Register));
            }
        }


        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await db.Vehicle
                .Include(t => t.VehicleType)
                .Select(v => new DetailsViewModel
                { 
                    Id = v.VehicleId,
                    VehicleType = v.VehicleType.Type,
                    RegistrationNumber = v.RegistrationNumber,
                    Brand = v.Brand,
                    VehicleModel = v.VehicleModel,
                })
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        public IActionResult Overview()
        {
            var model = new OverviewListViewModel();
            model.Overview = GetOverviewViewModelAsEnumerable();

            return View(nameof(Overview), model);
        }

        private async Task<IEnumerable<SelectListItem>> GetVehicleTypesAsync()
        {
            return await db.Vehicle
                        .Select(t => t.VehicleType)
                        .Distinct()
                        .Select(g => new SelectListItem
                        {
                            Text = g.ToString(),
                            Value = g.ToString()
                        })
                        .ToListAsync();
        }
        private async Task<IEnumerable<SelectListItem>> GetAllVehicleTypesAsync()
        {
            return await db.VehicleType.Select(vt => new SelectListItem
            {
                Text = vt.Type,
                Value = vt.VehicleTypeId.ToString(),
            }).ToListAsync();
        }

        //For vehicle Overview sorting
        private IEnumerable<OverviewViewModel> GetOverviewViewModelAsEnumerable()
        {
            var list = (from f in db.Vehicle
                        join p in db.Owner on f.OwnerId equals p.OwnerId                       //owner db
                        join t in db.ParkingEvent on f.VehicleId equals t.VehicleId            //parkingevent db
                        join ft in db.VehicleType on f.VehicleTypeId equals ft.VehicleTypeId   //vehicletype db
                       
                        select new OverviewViewModel
                        {
                            VehicleId = f.VehicleId,
                            FullName = p.FirstName + " " + p.LastName,
                            VehicleRegistrationNumber = f.RegistrationNumber,
                            VehicleArrivalTime = t.TimeOfArrival,
                            VehicleParkDuration = t.TimeOfArrival - DateTime.Now,
                            VehicleType = ft.Type,
                            VehicleTypeId = ft.VehicleTypeId,                           
                            Id = p.OwnerId

                        });
            return list.AsEnumerable();         
        } 


        //For vehicle Overview filtering regnumber, type
        public IActionResult Filter(OverviewListViewModel viewModel)
        {        
            var vehicleAndOwner = GetOverviewViewModelAsEnumerable();         
            var result =  vehicleAndOwner;

            //type: empty 
            if (viewModel.VehicleTypeId == 0)
            {
                result = viewModel.Regnumber == null ? 
                    vehicleAndOwner : vehicleAndOwner.Where(m => m.VehicleRegistrationNumber.StartsWith(viewModel.Regnumber.ToUpper()));
            }

            //type: selected, regnr: empty 
            if (viewModel.Regnumber == null && viewModel.VehicleTypeId != 0)
                result = vehicleAndOwner.Where(m => m.VehicleTypeId == viewModel.VehicleTypeId);


            // tpe: selected, regnr: selected
            if (viewModel.VehicleTypeId != 0 && viewModel.Regnumber != null)
            {
                result = vehicleAndOwner.Where(
                                            m => m.VehicleRegistrationNumber.StartsWith(viewModel.Regnumber.ToUpper())
                                            &&                                       
                                            m.VehicleTypeId == viewModel.VehicleTypeId); 
            }               

            var model = new OverviewListViewModel();
            model.Overview = result.Select(v => new OverviewViewModel
            {
                VehicleId = v.VehicleId,
                FullName = v.FullName,
                VehicleRegistrationNumber = v.VehicleRegistrationNumber,
                VehicleArrivalTime = v.VehicleArrivalTime,
                VehicleParkDuration = v.VehicleArrivalTime - DateTime.Now,
                VehicleType = v.VehicleType,              
            });
            
            return View("Overview", model);
        }

        [HttpGet, ActionName("OverviewSort")]
        public IActionResult OverviewSort(string sortingVehicle)
        {
            ViewData["VehicleTypeSorting"] = string.IsNullOrEmpty(sortingVehicle) ? "VehicleTypeSortingDescending" : "";
            ViewData["RegistrationNumberSorting"] = sortingVehicle == "RegistrationNumberSortingAscending" ? "RegistrationNumberSortingDescending" : "RegistrationNumberSortingAscending";
            ViewData["OwnerSorting"] = sortingVehicle == "OwnerSortingAscending" ? "OwnerSortingDescending" : "OwnerSortingAscending";
            ViewData["ArrivalTimeSorting"] = sortingVehicle == "ArrivalTimeSortingAscending" ? "ArrivalTimeSortingDescending" : "ArrivalTimeSortingAscending";
            ViewData["DurationParkedSorting"] = sortingVehicle == "DurationParkedSortingAscending" ? "DurationParkedSortingDescending" : "DurationParkedSortingAscending";

            var model = new OverviewListViewModel();

            var vehicleAndMember = GetOverviewViewModelAsEnumerable();

            switch (sortingVehicle)
            {
                case "VehicleTypeSortingAscending":
                    vehicleAndMember = vehicleAndMember.OrderBy(x => x.VehicleType);
                    break;
                case "VehicleTypeSortingDescending":
                    vehicleAndMember = vehicleAndMember.OrderByDescending(x => x.VehicleType);
                    break;
                case "OwnerSortingAscending":
                    vehicleAndMember = vehicleAndMember.OrderBy(x => x.FullName);
                    break;
                case "OwnerSortingDescending":
                    vehicleAndMember = vehicleAndMember.OrderByDescending(x => x.FullName);
                    break;
                case "RegistrationNumberSortingAscending":
                    vehicleAndMember = vehicleAndMember.OrderBy(x => x.VehicleRegistrationNumber);
                    break;
                case "RegistrationNumberSortingDescending":
                    vehicleAndMember = vehicleAndMember.OrderByDescending(x => x.VehicleRegistrationNumber);
                    break;
                case "ArrivalTimeSortingAscending":
                    vehicleAndMember = vehicleAndMember.OrderBy(x => x.VehicleArrivalTime);
                    break;
                case "ArrivalTimeSortingDescending":
                    vehicleAndMember = vehicleAndMember.OrderByDescending(x => x.VehicleArrivalTime);
                    break;
                case "DurationParkedSortingAscending":
                    vehicleAndMember = vehicleAndMember.OrderBy(x => x.VehicleParkDuration.Days).ThenBy(x=>x.VehicleParkDuration.Hours).ThenBy(x => x.VehicleParkDuration.Minutes).ThenBy(x => x.VehicleParkDuration.Seconds);
                    break;
                case "DurationParkedSortingDescending":
                    vehicleAndMember = vehicleAndMember.OrderByDescending(x => x.VehicleParkDuration.Days).ThenByDescending(x => x.VehicleParkDuration.Hours).ThenByDescending(x => x.VehicleParkDuration.Minutes).ThenByDescending(x => x.VehicleParkDuration.Seconds);
                    break;
                default:
                    vehicleAndMember = vehicleAndMember.OrderBy(x => x.VehicleType);
                    break;
            }

            model.Overview = vehicleAndMember;
            return PartialView(nameof(Overview), model);
        }

        [HttpGet]
        public async Task<IActionResult> Register(int id)
        {
            var owner = await db.Owner.FindAsync(id);
            if (owner == null) return NotFound();
            else
            {
                var model = new RegisterVehicleViewModel
                {
                    VehicleTypes = await GetAllVehicleTypesAsync(),
                    Id = id,
                    FullName = $"{owner.FirstName} {owner.LastName}"
                };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVehicleViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool vehicleIsRegistered = await db.Vehicle.AnyAsync(v => v.RegistrationNumber == model.RegistrationNumber);

                var vehicle = new Vehicle
                {
                    RegistrationNumber = model.RegistrationNumber.ToUpper(),
                    VehicleTypeId = model.VehicleTypeId,
                    Brand = model.Brand,
                    VehicleModel = model.VehicleModel,
                    OwnerId = model.Id,

                };

                db.Add(vehicle);
                await db.SaveChangesAsync();
                TempData["RegMessage"] = "";
                return RedirectToAction("Details", new { id = vehicle.VehicleId });
            }
            else
            {
                return View(model);
            }

        }
        [HttpGet]
        public async Task<IActionResult> ParkRegisteredVehicle(int? id)
        {
            var vehicle = await db.Vehicle.FirstOrDefaultAsync(x => x.VehicleId == id);

            try
            {
                db.Update(vehicle);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (vehicle.VehicleId != id)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", new { id = vehicle.VehicleId });
        }
        public async Task<IActionResult> UnPark(int? vehicleid)
        {
            var vehicle = await db.Vehicle.FirstOrDefaultAsync(x => x.VehicleId == vehicleid);
            var departureTime = DateTime.Now;
            var parkingEvent = await db.ParkingEvent
                .Where(pe => pe.VehicleId == vehicleid)
                .FirstOrDefaultAsync();

            var parkingPlace = await db.ParkingEvent
                .Where(pe => pe.VehicleId == vehicleid)
                .Include(pp => pp.ParkingPlace)
                .Select(pp => pp.ParkingPlace)
                .FirstOrDefaultAsync();

            parkingPlace.IsOccupied = false;
            var arrivalTime = parkingEvent.TimeOfArrival;
            db.Update(parkingPlace);
            db.Remove(parkingEvent);
            db.SaveChanges();

            return RedirectToAction("UnparkResponse", new { vehicleid, departureTime, arrivalTime });
        }


        public async Task<IActionResult> Change(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var vehicle = await db.Vehicle.FindAsync(Id);
            var model = new ChangeVehicleViewModel
            {
                RegistrationNumber = vehicle.RegistrationNumber,
                VehicleTypes = await GetAllVehicleTypesAsync(),
                VehicleTypeId = vehicle.VehicleTypeId,
                Id = vehicle.VehicleId,
                Brand = vehicle.Brand,
                VehicleModel = vehicle.VehicleModel,
                OwnerId = vehicle.OwnerId

            };

            if (vehicle == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public bool Equals(Vehicle b1, Vehicle b2)
        {
            if (b1.RegistrationNumber == b2.RegistrationNumber
                && b1.Brand == b2.Brand
                && b1.VehicleModel == b2.VehicleModel 
                && b1.VehicleTypeId == b2.VehicleTypeId)
                return true;
            else
                return false;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(int Id, ChangeVehicleViewModel vehicle)
        {
            //från databasen
            var v1 = await db.Vehicle.AsNoTracking().FirstOrDefaultAsync(v => v.VehicleId == Id);

            var vehicleobject = new Vehicle
            {
                VehicleId = v1.VehicleId,
                RegistrationNumber = vehicle.RegistrationNumber,
                Brand = vehicle.Brand,
                VehicleModel = vehicle.VehicleModel,
                VehicleTypeId = vehicle.VehicleTypeId,
                OwnerId = vehicle.OwnerId
            };

            if (!Equals(v1, vehicleobject))
            {

                if (Id != vehicleobject.VehicleId)
                {
                    return NotFound();
                }

                vehicleobject.RegistrationNumber = v1.RegistrationNumber;

                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Update(vehicleobject);
                        await db.SaveChangesAsync();
                        TempData["ChangedVehicle"] = "The vehicle is changed!";
                        return RedirectToAction("Details", new { id = vehicleobject.VehicleId });
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!VehicleExists(vehicleobject.VehicleId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            return RedirectToAction("Details", new { id= vehicleobject.VehicleId });
        }


        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await db.Vehicle
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await db.Vehicle.FindAsync(id);
            db.Vehicle.Remove(vehicle);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return db.Vehicle.Any(e => e.VehicleId == id);
        }
        public async Task<IActionResult> UnParkResponse(int vehicleId, DateTime departureTime, DateTime arrivalTime)
        {
            var model = await db.Vehicle
                .Select(v => new UnParkResponseViewModel
                {
                    VehicleId = v.VehicleId,
                    VehicleRegistrationNumber = v.RegistrationNumber,
                    VehicleArrivalTime = arrivalTime,
                    VehicleDepartureTime = departureTime,
                })
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            return View("UnParkResponse", model);
        }

        public async Task<IActionResult> Receipt(int vehicleId, DateTime departureTime, DateTime arrivalTime)
        {
            var vehicle = await db.Vehicle
                .Include(v => v.ParkingEvents)
                .Include(v => v.Owner)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            var model = new ReceiptViewModel
            {
                VehicleRegistrationNumber = vehicle.RegistrationNumber,
                VehicleArrivalTime = arrivalTime,
                VehicleDepartureTime = departureTime,
                VehicleParkDuration = arrivalTime - departureTime,
                VehicleParkPrice = (departureTime - arrivalTime).TotalHours * 100,

                MemberFullName = $"{vehicle.Owner.LastName}, {vehicle.Owner.FirstName}",
                MemberSSN = vehicle.Owner.SocialSecurityNumber
            };

            return View(model);
        }



        //public async Task<IActionResult> Statistics()
        //{
        //    var vehicles = await db.Vehicle.ToListAsync();

        //    var model = new StatisticsViewModel
        //    {
        //        VehicleTypesData = Enum.GetValues(typeof(VehicleTypes))
        //                               .Cast<VehicleTypes>()
        //                               .ToDictionary(type => type.ToString(), type => vehicles
        //                                                                                .Where(v => v.VehicleType == type && v.IsParked)
        //                                                                                .Count()),

        //        NumberOfWheels = vehicles
        //                            .Where(v => v.IsParked)
        //                            .Select(v => v.NumberOfWheels)
        //                            .Sum(),

        //        GeneratedRevenue = vehicles
        //                            .Where(v => v.IsParked)
        //                            .Select(v =>
        //                                (DateTime.Now - v.TimeOfArrival).TotalHours
        //                              + (DateTime.Now - v.TimeOfArrival).TotalDays * 24
        //                                )
        //                            .Sum() * 100
        //    };
        //    return View(model);
        //}
        public async Task<IActionResult> CheckUnique(string registrationnumber)
        {
            bool regExists = await db.Vehicle.AnyAsync(v => v.RegistrationNumber == registrationnumber);

            if (regExists) return Json("A vehicle with this registration number aldready exists.");

            return Json(true);
        }
    }
}