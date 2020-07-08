﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Warthog.Api.Services
{
    public class UtilsTest
    {
        [Fact]
        public void Constants()
        {
            Assert.Equal("sha256", Utils.ChecksumType);
        }

        [Fact]
        public void GetSha256HashFromString()
        {
            var str = "test";
            var result = Utils.GetSha256HashFromString(str);

            Assert.NotNull(result);
        }
    }
}