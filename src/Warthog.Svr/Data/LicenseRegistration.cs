﻿using System;
using System.ComponentModel.DataAnnotations;


namespace Warthog.Api.Data
{
    public class LicenseRegistration
    {
        [Key]
        public int LicenseRegistrationId { get; set; }

        [Required]
        public Guid LicenseUuid { get; set; }

        [Required]
        public Guid ProductUuid { get; set; }

        [Required]
        public string LicenseName { get; set; }

        [Required]
        public string LicenseEmail { get; set; }

        [Required]
        public LicenseType LicenseType { get; set; }

        public bool IsActive { get; set; }  // Blacklist

        public string Comment { get; set; } // Reasons for blacklist

        public int Quantity { get; set; }

        public DateTime? Expire { get; set; }  

        [Required]
        public DateTime CreatedDateTimeUtc { get; set; }

        [Required]
        public string CreatedByUser { get; set; }
    }
}