﻿using samplesl.Sample_XamarinForms.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace samplesl.Sample_XamarinForms.Test
{
    public class LicenseResultTest
    {
        [Fact]
        public void NotSuccessful()
        {
            var model = new LicenseResult(null, new Exception(), null);

            Assert.False(model.Successful);
        }

        [Fact]
        public void Successful()
        {
            var license = License.Load("<license></license>");
            var model = new LicenseResult(license, null, null);

            Assert.True(model.Successful);
        }
    }
}
