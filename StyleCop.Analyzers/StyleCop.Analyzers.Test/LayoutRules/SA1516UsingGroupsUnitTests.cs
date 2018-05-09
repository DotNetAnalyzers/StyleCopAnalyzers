// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Verifies the using directives functionality of <see cref="SA1516ElementsMustBeSeparatedByBlankLine"/>.
    /// </summary>
    public class SA1516UsingGroupsUnitTests : CodeFixVerifier
    {
        private bool? systemUsingDirectivesFirst;
        private OptionSetting? blankLinesBetweenUsingGroups;

        /// <summary>
        /// Verifies the allow scenario is handled properly.
        /// </summary>
        /// <param name="namespaceName">The namespace name to use when wrapping. Null indicates no wrapping.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null)]
        [InlineData("Test")]
        public async Task TestAllowForCompilationUnitAsync(string namespaceName)
        {
            this.blankLinesBetweenUsingGroups = OptionSetting.Allow;

            var testCode = @"
using System;

using TestNamespace;
using static System.Math;

using Factory = System.Activator;
";

            testCode = WrapWithNamespace(testCode, namespaceName);

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        //// TODO: Add namespace checks to the remaining code using WrapWithNamespace

        /// <summary>
        /// Verifies the omit scenario with system first is handled properly.
        /// </summary>
        /// <param name="namespaceName">The namespace name to use when wrapping. Null indicates no wrapping.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null)]
        [InlineData("Test")]
        public async Task TestOmitWithSystemFirstAsync(string namespaceName)
        {
            this.systemUsingDirectivesFirst = true;
            this.blankLinesBetweenUsingGroups = OptionSetting.Omit;

            var testCode = @"
using System;

using TestNamespace;

using static System.Math;

using Factory = System.Activator;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"
using System;
using TestNamespace;
using static System.Math;
using Factory = System.Activator;

namespace TestNamespace
{
}
";

            testCode = WrapWithNamespace(testCode, namespaceName);
            fixedTestCode = WrapWithNamespace(fixedTestCode, namespaceName);
            var offset = GetLineOffset(namespaceName);

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(4 + offset, 1),
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(6 + offset, 1),
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(8 + offset, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the omit scenario without system first is handled properly.
        /// </summary>
        /// <param name="namespaceName">The namespace name to use when wrapping. Null indicates no wrapping.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null)]
        [InlineData("Test")]
        public async Task TestOmitWithoutSystemFirstAsync(string namespaceName)
        {
            this.systemUsingDirectivesFirst = false;
            this.blankLinesBetweenUsingGroups = OptionSetting.Omit;

            var testCode = @"
using System;
using TestNamespace;

using static System.Math;

using Factory = System.Activator;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"
using System;
using TestNamespace;
using static System.Math;
using Factory = System.Activator;

namespace TestNamespace
{
}
";

            testCode = WrapWithNamespace(testCode, namespaceName);
            fixedTestCode = WrapWithNamespace(fixedTestCode, namespaceName);
            var offset = GetLineOffset(namespaceName);

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(5 + offset, 1),
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(7 + offset, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the omit scenario with comments separating instead of blank lines.
        /// </summary>
        /// <param name="namespaceName">The namespace name to use when wrapping. Null indicates no wrapping.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null)]
        [InlineData("Test")]
        public async Task TestOmitWithSeparingCommentsAsync(string namespaceName)
        {
            this.systemUsingDirectivesFirst = true;
            this.blankLinesBetweenUsingGroups = OptionSetting.Omit;

            var testCode = @"
using System;
// regular group
using TestNamespace;
// static group
using static System.Math;
/* alias group */
using Factory = System.Activator;

namespace TestNamespace
{
}
";

            testCode = WrapWithNamespace(testCode, namespaceName);

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the require scenario with system first is handled properly.
        /// </summary>
        /// <param name="namespaceName">The namespace name to use when wrapping. Null indicates no wrapping.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null)]
        [InlineData("Test")]
        public async Task TestRequireWithSystemFirstAsync(string namespaceName)
        {
            this.systemUsingDirectivesFirst = true;
            this.blankLinesBetweenUsingGroups = OptionSetting.Require;

            var testCode = @"
using System;
using TestNamespace;
using static System.Math;
using Factory = System.Activator;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"
using System;

using TestNamespace;

using static System.Math;

using Factory = System.Activator;

namespace TestNamespace
{
}
";

            testCode = WrapWithNamespace(testCode, namespaceName);
            fixedTestCode = WrapWithNamespace(fixedTestCode, namespaceName);
            var offset = GetLineOffset(namespaceName);

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(3 + offset, 1),
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(4 + offset, 1),
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(5 + offset, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the require scenario without system first is handled properly.
        /// </summary>
        /// <param name="namespaceName">The namespace name to use when wrapping. Null indicates no wrapping.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null)]
        [InlineData("Test")]
        public async Task TestRequireWithoutSystemFirstAsync(string namespaceName)
        {
            this.systemUsingDirectivesFirst = false;
            this.blankLinesBetweenUsingGroups = OptionSetting.Require;

            var testCode = @"
using System;
using TestNamespace;
using static System.Math;
using Factory = System.Activator;

namespace TestNamespace
{
}
";

            var fixedTestCode = @"
using System;
using TestNamespace;

using static System.Math;

using Factory = System.Activator;

namespace TestNamespace
{
}
";

            testCode = WrapWithNamespace(testCode, namespaceName);
            fixedTestCode = WrapWithNamespace(fixedTestCode, namespaceName);
            var offset = GetLineOffset(namespaceName);

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(4 + offset, 1),
                this.CSharpDiagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(5 + offset, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            var useBlankLinesBetweenUsingGroups = this.blankLinesBetweenUsingGroups ?? OptionSetting.Allow;
            var systemUsingDirectivesFirst = this.systemUsingDirectivesFirst ?? true;

            return $@"
{{
    ""settings"": {{
        ""orderingRules"": {{
            ""systemUsingDirectivesFirst"" : {systemUsingDirectivesFirst.ToString().ToLowerInvariant()},
            ""blankLinesBetweenUsingGroups"": ""{this.blankLinesBetweenUsingGroups.ToString().ToLowerInvariant()}""
        }}
    }}
}}
";
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1516ElementsMustBeSeparatedByBlankLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1516CodeFixProvider();
        }

        private static string WrapWithNamespace(string testCode, string namespaceName)
        {
            var builder = new StringBuilder(testCode);

            if (!string.IsNullOrEmpty(namespaceName))
            {
                builder.Insert(0, $"namespace {namespaceName}\r\n{{\r\n");
                builder.Append("}\r\n");
            }

            builder.Append(@"
namespace TestNamespace { }
");

            return builder.ToString();
        }

        private static int GetLineOffset(string namespaceName)
        {
            return string.IsNullOrEmpty(namespaceName) ? 0 : 2;
        }
    }
}
