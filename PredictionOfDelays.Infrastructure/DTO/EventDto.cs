﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PredictionOfDelays.Core;
using PredictionOfDelays.Core.Models;

namespace PredictionOfDelays.Infrastructure.DTO
{
    public class EventDto
    {
        public int EventId { get; set; }
        public string OwnerUserId { get; set; }
        [Required]
        public LocalizationDto Localization { get; set; }
        [Required]
        [MaxLength(50), MinLength(5)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of event")]
        public DateTime EventDate { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }
        public List<UserEventDto> Users { get; set; }
    }
}