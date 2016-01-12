// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpecialRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.Settings;
    using Analyzers.SpecialRules;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA0002InvalidSettingsFile"/>.
    /// </summary>
    public class SA0002UnitTests : DiagnosticVerifier
    {
        private const string TestCode = @"
namespace NamespaceName { }
";

        private string settings;

        [Fact]
        public async Task TestMissingSettingsAsync()
        {
           await this.VerifyCSharpDiagnosticAsync(TestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidSettingsAsync()
        {
            this.settings = SettingsFileCodeFixProvider.DefaultSettingsFileContent;

            await this.VerifyCSharpDiagnosticAsync(TestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingsAsync()
        {
            this.settings = @"
{
  ""$schema"": ""https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json""
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""ACME, Inc"",
      ""copyrightText"": ""Copyright 2015 {companyName}. All rights reserved.""
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            return this.settings;
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA0002InvalidSettingsFile();
        }
    }
}
