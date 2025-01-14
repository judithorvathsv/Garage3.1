﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Garage3.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        [Display(Name = "Registration number")]
        [Required(ErrorMessage = "Please enter registration number!")]
        [MaxLength(10)]
        [MinLength(3)]
        [RegularExpression("^([-a-zA-Z-0-9-]+)$", ErrorMessage = "Invalid registration number!")]
        public string RegistrationNumber { get; set; }

        [RegularExpression("^[a-zA-Z0-9åäö][a-zA-Z0-9-\\såäö]+", ErrorMessage = "Invalid brand!")]
        [Required(ErrorMessage = "Please enter brand!")]
        [MaxLength(20)]
        [MinLength(3, ErrorMessage = "The brand name must be longer!")]
        public string Brand { get; set; }

        [Display(Name = "Vehicle model")]
        [RegularExpression("^[a-zA-Z0-9åäö][a-zA-Z0-9-\\såäö]+", ErrorMessage = "Invalid model!")]
        [Required(ErrorMessage = "Please enter model!")]   
        [MaxLength(20)]
        [MinLength(1, ErrorMessage = "The model name must be longer!")]
        public string VehicleModel { get; set; }

        // FK
        public int OwnerId { get; set; }
        public int VehicleTypeId { get; set; }

        //navigation
        public VehicleType VehicleType { get; set; }
        public Owner Owner { get; set; }

        // M:M NAV
        public ICollection<ParkingPlace> ParkingPlaces { get; set; }
        public ICollection<ParkingEvent> ParkingEvents { get; set; }


    }
}
