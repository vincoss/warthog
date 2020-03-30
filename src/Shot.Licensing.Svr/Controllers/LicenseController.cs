﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using samplesl.Svr.Interface;
using samplesl.Svr.Models;


namespace samplesl.Svr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(ILicenseService licenseService)
        {
            if(licenseService ==  null)
            {
                throw new ArgumentNullException(nameof(licenseService));
            }
            _licenseService = licenseService;
        }

        // GET: api/license
        [HttpGet]
        public void Get()
        {
            // Used to check if server is available
        }

        // POST: api/license
        [HttpPost]
        public async Task<ActionResult<LicenseRegisterResult>> Post([FromBody] LicenseRegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            return await _licenseService.CreateAsync(request);
        }
    }
}
