// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.NamingRules;
    using Xunit;

    /// <summary>
    /// This class contains tests for <see cref="SA1301ElementMustBeginWithLowerCaseLetter"/>.
    /// </summary>
    public class SA1301UnitTests
    {
        [Fact]
        public void TestDisabledByDefaultAndNotConfigurable()
        {
            var analyzer = new SA1301ElementMustBeginWithLowerCaseLetter();
            Assert.Single(analyzer.SupportedDiagnostics);
            Assert.False(analyzer.SupportedDiagnostics[0].IsEnabledByDefault);
            Assert.Contains(WellKnownDiagnosticTags.NotConfigurable, analyzer.SupportedDiagnostics[0].CustomTags);
        }
    }
}
