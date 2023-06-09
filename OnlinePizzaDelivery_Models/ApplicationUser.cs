﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizzaDelivery_Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        [NotMapped]
        public string StreetAddress { get; set; }
        [NotMapped]
        public string City { get; set; }
        [NotMapped]
        public string HouseNumber { get; set; }
        [NotMapped]
        public int Entrance { get; set; }
        [NotMapped]
        public int Apartment { get; set; }
    }
}
