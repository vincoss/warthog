﻿using Microsoft.Extensions.Logging;
using Gnu.Licensing.Svr.Data;
using Gnu.Licensing.Svr.Interface;
using Gnu.Licensing.Svr.ViewModels;
using System;
using System.Threading.Tasks;


namespace Gnu.Licensing.Svr.Services
{
    public class LicenseRegistrationService : ILicenseRegistrationService
    {
        private readonly EfDbContext _context;
        private readonly ILogger<LicenseRegistrationService> _logger;

        public LicenseRegistrationService(EfDbContext context, ILogger<LicenseRegistrationService> logger)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Create(LicenseRegistrationViewModel model, string createdByUser)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(createdByUser)) throw new ArgumentNullException(nameof(createdByUser));

            var registration = new LicenseRegistration
            {
                ProductUuid = model.ProductUuid,
                LicenseName = model.LicenseName,
                LicenseEmail = model.LicenseEmail,
                LicenseType = model.LicenseType,
                Quantity = model.Quantity,
                CreatedByUser = createdByUser
            };

            _context.Add(registration);
            await _context.SaveChangesAsync();

            return registration.LicenseUuid;
        }
    }
}