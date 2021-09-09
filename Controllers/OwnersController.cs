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

namespace Garage3.Controllers
{
    public class OwnersController : Controller
    {
        private readonly Garage3Context db;

        public OwnersController(Garage3Context context)
        {
            db = context;
        }

        // GET: Owners
        public async Task<IActionResult> Index()
        {
            return View(await db.Owner.ToListAsync());
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await db.Owner
                .FirstOrDefaultAsync(m => m.SocialSecurityNumber == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool memberIsRegistered = await db.Owner.AnyAsync(v => v.SocialSecurityNumber == model.SocialSecurityNumber);
                if (!memberIsRegistered)
                {

                    var member = new Owner
                    {
                        SocialSecurityNumber = model.SocialSecurityNumber,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    };
                    db.Add(member);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Register", "Vehicles", new { ssn = member.SocialSecurityNumber });
                }
                else
                {
                    var existingMember = await db.Owner.FirstOrDefaultAsync(v => v.SocialSecurityNumber.Contains(model.SocialSecurityNumber));
                    TempData["SSNMessage"] = "A member with this social security number already exists!";
                }
            }
            return View(model);
        }

        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await db.Owner.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SocialSecurityNumber,FirstName,LastName")] Owner owner)
        {
            if (id != owner.SocialSecurityNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(owner);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.SocialSecurityNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(owner);
        }

        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await db.Owner
                .FirstOrDefaultAsync(m => m.SocialSecurityNumber == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var owner = await db.Owner.FindAsync(id);
            db.Owner.Remove(owner);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Member(string ssn)
        {
            if (ssn == null)
            {
                return NotFound();
            }
            var vehicles = await db.Vehicle
                .Where(v => v.Owner.SocialSecurityNumber == ssn)
                .Join(db.Owner, v => v.Owner.SocialSecurityNumber, o => o.SocialSecurityNumber, (v, o) => new { v, o })
                .Join(db.VehicleType, vo => vo.v.VehicleType.VehicleTypeId, vt => vt.VehicleTypeId, (vo, vt) => new { vo, vt })
                .Select(m => new OwnerDetailsViewModel
                {
                    SocialSecurityNumber = m.vo.o.SocialSecurityNumber,
                    FullName = m.vo.o.FirstName + " " + m.vo.o.LastName,
                    Type = m.vo.v.VehicleType.Type,
                    RegistrationNumber = m.vo.v.RegistrationNumber,
                    Model = m.vo.v.VehicleModel

            try
            {
                var vehicle = await db.Vehicle
                        .Join( db.Owner,
                         v => v.Owner.SocialSecurityNumber, m => m.SocialSecurityNumber,
                         (v,m) => new { Vehi = v, Memb = m })
                         .Where(m => m.Memb.SocialSecurityNumber == ssn)
                         .Join(db.VehicleType, 
                         vh => vh.Vehi.VehicleType.VehicleTypeId,
                         t => t.VehicleTypeId,
                         (vh,t) => new {ve = vh, type = t })
                     
                         .Select(o => new OwnerDetailsViewModel
                         {
                             SocialSecurityNumber = o.ve.Memb.SocialSecurityNumber,
                             FullName = o.ve.Memb.FirstName + " " + o.ve.Memb.LastName,
                             RegistrationNumber = o.ve.Vehi.RegistrationNumber,
                             Brand =  o.ve.Vehi.Brand,
                             VehicleModel = o.ve.Vehi.VehicleModel,
                             VehicleType = o.type.Type
                              
                         }).ToListAsync();

                if (vehicle == null)
                {
                    return NotFound();
                }
                return View(vehicle);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool OwnerExists(string id)
        {
            return db.Owner.Any(e => e.SocialSecurityNumber == id);
        }


        [ActionName("Overview")]
        public async Task<IActionResult> Overview()
        {
            var listWithEmpty = (from p in db.Owner
                                 join f in db.Vehicle
                                 on p.SocialSecurityNumber equals f.Owner.SocialSecurityNumber into ThisList
                                 from f in ThisList.DefaultIfEmpty()

                                 group p by new
                                 {
                                     p.FirstName,
                                     p.LastName,
                                     p.SocialSecurityNumber
                                 } into gcs
                                 select new
                                 {
                                     FirstName = gcs.Key.FirstName,
                                     LastName = gcs.Key.LastName,
                                     SocialSecurityNumber = gcs.Key.SocialSecurityNumber,
                                     NumberOfVehicles = gcs.Select(x => x).Distinct().Count(),
                                 })
                               .ToList()
                                .Select(x => new Models.ViewModels.MemberDetailsViewModel()
                                {
                                    FirstName = x.FirstName,
                                    LastName = x.LastName,
                                    FullName = x.FirstName + " " + x.LastName,
                                    SocialSecurityNumber = x.SocialSecurityNumber,
                                    NumberOfVehicles = x.NumberOfVehicles
                                });

            var sortedMemberList = listWithEmpty.OrderBy(x => x.FirstName.Substring(0, 1), StringComparer.Ordinal);

            return View(sortedMemberList);
        }
    }
}
