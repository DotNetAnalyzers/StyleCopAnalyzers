// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.HelperTests
{
    using System;
    using StyleCop.Analyzers.Helpers;
    using Xunit;

    public class RequiresTests
    {
        [Fact]
        public void TestNotNull()
        {
            string parameterName = nameof(parameterName);
            Requires.NotNull(string.Empty, parameterName);
        }

        [Fact]
        public void TestNull()
        {
            string parameterName = nameof(parameterName);
            Assert.Throws<ArgumentNullException>(parameterName, () => Requires.NotNull(default(string), parameterName));
        }
    }
}
