﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictionOfDelays.Core.Models
{
    public class Event : IEntity
    {
        public int EventId { get; set; }

        [Required]
        [MaxLength(50),MinLength(5)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of event")]
        [FutureDate(ErrorMessage = "Enter future date")]
        public DateTime EventDate { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }

        //todo add localizatio, administrators and restrictions

        public Event()
        {
           Users = new List<ApplicationUser>();
        }

        
    }
}
