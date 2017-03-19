// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.ReadabilityRules;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for SA1140.
    /// </summary>
    /// <seealso cref="SA1140MaximumLineLength" />
    public class SA1140UnitTests : DiagnosticVerifier
    {
        private string settings = string.Empty;

        [Fact]
        public async Task TestLineLengthAsync()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        double line6length35 = 0.0;
        double line7length40 = 0.000000;
        double line8length41 = 0.0000000;
        double line9length42 = 0.00000000;
    }
}";

            var diagnosticLine1 = this.CSharpDiagnostic().WithLocation(6, 1);
            var diagnosticLine2 = this.CSharpDiagnostic().WithLocation(7, 1);
            var diagnosticLine3 = this.CSharpDiagnostic().WithLocation(8, 1);
            var diagnosticLine4 = this.CSharpDiagnostic().WithLocation(9, 1);

            this.SetLineLengthSettings(34);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { diagnosticLine1, diagnosticLine2, diagnosticLine3, diagnosticLine4 }, CancellationToken.None).ConfigureAwait(false);

            this.SetLineLengthSettings(35);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { diagnosticLine2, diagnosticLine3, diagnosticLine4 }, CancellationToken.None).ConfigureAwait(false);

            this.SetLineLengthSettings(41);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { diagnosticLine4 }, CancellationToken.None).ConfigureAwait(false);

            this.SetLineLengthSettings(42);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] {}, CancellationToken.None).ConfigureAwait(false);

        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            return this.settings;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1140MaximumLineLength();
        }

        private void SetLineLengthSettings(int maxLineLength)
        {
            this.settings = @"
{
    ""settings"": {
        ""readabilityRules"": {
            ""lineLength"": " + maxLineLength.ToString() + @"
        }
    }
}
";
        }
    }
}
