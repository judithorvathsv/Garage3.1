﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garage3.Models
{
    public class Owner
    {
        public int OwnerId { get; set; }

        [Required(ErrorMessage = "Please enter your social security number!")]
        [Display(Name = "Social Security Number")]
        [MaxLength(13)]
        [MinLength(10)]
        [RegularExpression("[0-9]{6}-[0-9]{4}", ErrorMessage = "Invalid social security number!")]        
        public string SocialSecurityNumber { get; set; }

        [Required(ErrorMessage = "Please enter firstname!")]
        [Display(Name = "Firstname")]
        [MaxLength(35)]
        [MinLength(3)]
        [RegularExpression("[-a-zA-Z]+", ErrorMessage = "Invalid firstname!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter lastname!")]
        [Display(Name = "Lastname")]
        [MaxLength(35)]
        [MinLength(3)]
        [RegularExpression("[-a-zA-Z]+", ErrorMessage = "Invalid lastname!")]
        public string LastName { get; set; }



        // NAV
        public ICollection<Vehicle> Vehicles { get; set; }

    }
}
