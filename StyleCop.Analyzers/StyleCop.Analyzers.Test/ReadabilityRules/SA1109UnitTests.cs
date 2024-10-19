// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1109BlockStatementsMustNotContainEmbeddedRegions"/>.
    /// </summary>
    public class SA1109UnitTests
    {
        [Fact]
        public void TestDisabledByDefaultAndNotConfigurable()
        {
            var analyzer = new SA1109BlockStatementsMustNotContainEmbeddedRegions();
            Assert.Single(analyzer.SupportedDiagnostics);
            Assert.False(analyzer.SupportedDiagnostics[0].IsEnabledByDefault);
            Assert.Contains(WellKnownDiagnosticTags.NotConfigurable, analyzer.SupportedDiagnostics[0].CustomTags);
        }
    }
}
