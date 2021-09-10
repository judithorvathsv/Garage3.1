﻿using Garage3.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garage3.Services
{
    public class VehicleTypeSelectService : IVehicleTypeSelectService
    {
        private readonly Garage3Context db;

        public VehicleTypeSelectService(Garage3Context db)
        {
            this.db = db;
        }


        public async Task<IEnumerable<SelectListItem>> GetVehicleTypesAsync()
        {
            var typesList = await db.VehicleType
                            .Select(type => new SelectListItem()
                            {
                                Text = type.Type,
                                Value = type.VehicleTypeId.ToString(),
                            }).ToListAsync();

            //typesList.Insert(0, new SelectListItem()
            //{
            //    Text = "----Select----",
            //    Value = string.Empty
            //});

            return typesList;


        }
    }
}
