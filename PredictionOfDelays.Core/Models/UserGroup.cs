﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PredictionOfDelays.Core.Models
{
    public class UserGroup
    {
        [Key,Column(Order = 0)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Key, Column(Order = 1)]
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
