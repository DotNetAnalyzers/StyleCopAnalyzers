// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains tests for <see cref="SA1301ElementMustBeginWithLowerCaseLetter"/>.
    /// </summary>
    public class SA1301UnitTests : DiagnosticVerifier
    {
        [Fact]
        public void TestDisabledByDefaultAndNotConfigurable()
        {
            var analyzer = this.GetCSharpDiagnosticAnalyzers().Single();
            Assert.Equal(1, analyzer.SupportedDiagnostics.Length);
            Assert.False(analyzer.SupportedDiagnostics[0].IsEnabledByDefault);
            Assert.Contains(WellKnownDiagnosticTags.NotConfigurable, analyzer.SupportedDiagnostics[0].CustomTags);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1301ElementMustBeginWithLowerCaseLetter();
        }
    }
}
