﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gnu.Licensing.Validation;


namespace Gnu.Licensing.Sample_Console
{
    class Program
    {
        /// <summary>
        /// NOTE: This one is to lock the license to particular machine or software installation. Create unique Id when the software is installed or first time started.
        /// </summary>
        const string AppId = "ae8cdf5f-26b3-4e2f-8e68-6ecc2e73720f";

        static void Main(string[] args)
        {
            var p = new Program();
            p.Sample();

            Console.WriteLine("Done...");
            Console.Read();
        }

        public async void Sample()
        {
            /* 
                Validate license on the client device|machine
            */

            var directory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var licensePath = Path.Combine(directory, "data", "test.license.xml");
            
            using(var license = File.OpenRead(licensePath))
            {
                Console.WriteLine("Validating license.");

                var results = await ValidateAsync(license);

                if (results.Any())
                {
                    foreach (var r in results)
                    {
                        Console.WriteLine(r.Message);
                        Console.WriteLine(r.HowToResolve);
                    }
                }
                else
                {
                    Console.WriteLine("Device is licensed. (Full)");
                }
            }
        }

        public Task<IEnumerable<IValidationFailure>> ValidateAsync(Stream license)
        {
            if (license == null)
            {
                throw new ArgumentNullException(nameof(license));
            }

            var task = Task.Run(() =>
            {
                var results = new List<IValidationFailure>();

                try
                {
                    var actual = License.Load(license);

                    var failure = FailureStrings.Get(FailureStrings.VAL04Code);

                    var validationFailures = actual.Validate()
                                                   .ExpirationDate()
                                                   .When(lic => lic.Type == LicenseType.Standard)
                                                   .And()
                                                   .Signature()
                                                   .And()
                                                   .AssertThat(x => string.Equals(AppId, x.AdditionalAttributes.Get(LicenseGlobals.AppId), StringComparison.OrdinalIgnoreCase), failure)
                                                   .AssertValidLicense().ToList();

                    foreach (var f in validationFailures)
                    {
                        results.Add(f);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    var failure = FailureStrings.Get(FailureStrings.VAL01Code);
                    results.Add(failure);
                }
                return results.AsEnumerable();
            });
            return task;
        }

    }
    
}