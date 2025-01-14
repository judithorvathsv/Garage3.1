﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garage3.Models
{
    public class VehicleType
    {
        public int VehicleTypeId { get; set; }
    
        [Display(Name = "Vehicle type")]
        [Required(ErrorMessage = "Please choose type!")]
        [MaxLength(35)]
        [MinLength(3)]
        public string Type { get; set; }
        public int Size { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
