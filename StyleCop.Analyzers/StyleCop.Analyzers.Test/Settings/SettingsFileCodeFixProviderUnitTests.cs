// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Settings
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Settings;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SettingsFileCodeFixProvider"/>.
    /// </summary>
    public class SettingsFileCodeFixProviderUnitTests : CodeFixVerifier
    {
        private const string TestCode = @"
namespace NamespaceName
{
}
";

        private bool createSettingsFile;
        private string settingsFileName = SettingsHelper.SettingsFileName;

        /// <summary>
        /// Verifies that a file without a header, but with leading trivia will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMissingFileHeaderWithLeadingTriviaAsync()
        {
            this.createSettingsFile = false;

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(TestCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);

            // verify that the code fix does not alter the document
            await this.VerifyCSharpFixAsync(TestCode, TestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a code fix will be offered if the settings file does not exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSettingsFileDoesNotExistAsync()
        {
            this.createSettingsFile = false;

            var offeredFixes = await this.GetOfferedCSharpFixesAsync(TestCode).ConfigureAwait(false);
            Assert.Single(offeredFixes);
        }

        /// <summary>
        /// Verifies that a code fix will not be offered if the settings file is already present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSettingsFileAlreadyExistsAsync()
        {
            this.createSettingsFile = true;
            this.settingsFileName = SettingsHelper.SettingsFileName;

            var offeredFixes = await this.GetOfferedCSharpFixesAsync(TestCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        /// <summary>
        /// Verifies that a code fix will not be offered if the settings file is already present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDotPrefixedSettingsFileAlreadyExistsAsync()
        {
            this.createSettingsFile = true;
            this.settingsFileName = SettingsHelper.AltSettingsFileName;

            var offeredFixes = await this.GetOfferedCSharpFixesAsync(TestCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            if (this.createSettingsFile)
            {
                return "{}";
            }

            return null;
        }

        /// <inheritdoc/>
        protected override string GetSettingsFileName()
        {
            return this.settingsFileName;
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new FileHeaderAnalyzers();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SettingsFileCodeFixProvider();
        }
    }
}
