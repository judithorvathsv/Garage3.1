﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Garage3.Models.ViewModels
{
    public class OwnerDetailsViewModel
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }

        [Display(Name = "Social Security Number")]
        public string SocialSecurityNumber { get; set; }

        [Display(Name = "Firstname")]
        public string FirstName { get; set; }


        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Display(Name = "Name")]
        public string FullName { get; set; }

        public bool IsParked { get; set; }

        public IEnumerable<VehicleViewModel> Vehicles { get; set; }
    }
}