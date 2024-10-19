﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.LayoutRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1516ElementsMustBeSeparatedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1516CodeFixProvider>;

    /// <summary>
    /// Verifies the using directives functionality of <see cref="SA1516ElementsMustBeSeparatedByBlankLine"/>.
    /// </summary>
    public class SA1516UsingGroupsUnitTests
    {
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
            var testCode = @"
using System;

using TestNamespace;
using static System.Math;

using Factory = System.Activator;
";

            testCode = WrapWithNamespace(testCode, namespaceName);

            await new CSharpTest
            {
                TestCode = testCode,
                Settings = GetSettings(blankLinesBetweenUsingGroups: OptionSetting.Allow),
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(4 + offset, 1),
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(6 + offset, 1),
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(8 + offset, 1),
                },
                FixedCode = fixedTestCode,
                Settings = GetSettings(systemUsingDirectivesFirst: true, blankLinesBetweenUsingGroups: OptionSetting.Omit),
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(5 + offset, 1),
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorOmit).WithLocation(7 + offset, 1),
                },
                FixedCode = fixedTestCode,
                Settings = GetSettings(systemUsingDirectivesFirst: false, blankLinesBetweenUsingGroups: OptionSetting.Omit),
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await new CSharpTest
            {
                TestCode = testCode,
                Settings = GetSettings(systemUsingDirectivesFirst: true, blankLinesBetweenUsingGroups: OptionSetting.Omit),
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(3 + offset, 1),
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(4 + offset, 1),
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(5 + offset, 1),
                },
                FixedCode = fixedTestCode,
                Settings = GetSettings(systemUsingDirectivesFirst: true, blankLinesBetweenUsingGroups: OptionSetting.Require),
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(4 + offset, 1),
                    Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(5 + offset, 1),
                },
                FixedCode = fixedTestCode,
                Settings = GetSettings(systemUsingDirectivesFirst: false, blankLinesBetweenUsingGroups: OptionSetting.Require),
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        private static string GetSettings(bool systemUsingDirectivesFirst = true, OptionSetting blankLinesBetweenUsingGroups = OptionSetting.Allow)
        {
            return $@"
{{
    ""settings"": {{
        ""orderingRules"": {{
            ""systemUsingDirectivesFirst"" : {systemUsingDirectivesFirst.ToString().ToLowerInvariant()},
            ""blankLinesBetweenUsingGroups"": ""{blankLinesBetweenUsingGroups.ToString().ToLowerInvariant()}""
        }}
    }}
}}
";
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
