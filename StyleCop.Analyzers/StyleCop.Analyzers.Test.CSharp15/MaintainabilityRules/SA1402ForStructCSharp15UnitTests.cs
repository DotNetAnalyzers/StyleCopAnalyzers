// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.MaintainabilityRules;
    using StyleCop.Analyzers.Test.MaintainabilityRules;
    using Xunit;

    public partial class SA1402ForStructCSharp15UnitTests : SA1402ForStructCSharp14UnitTests
    {
        /// <summary>
        /// Verifies that a union declaration counts as a struct when structs are configured as top-level types, and
        /// that the code fix moves it to its own file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionCountsAsStructAsync()
        {
            var testCode = @"public struct Foo
{
}
public union {|#0:Bar|}(int, string);";

            var fixedSources = new[]
            {
                ("/0/Test0.cs", @"public struct Foo
{
}
"),
                ("Bar.cs", @"public union Bar(int, string);"),
            };

            var settings = SA1402SettingsConfiguration.ConfigureAsTopLevelType.GetSettings("struct");
            var expected = this.Diagnostic().WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, settings, expected, fixedSources, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a union declaration is not counted when structs are not configured as top-level types.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionNotCountedWhenStructIsNotTopLevelTypeAsync()
        {
            var testCode = @"public class Foo
{
}
public union Bar(int, string);";

            var settings = SA1402SettingsConfiguration.ConfigureAsNonTopLevelType.GetSettings("struct");
            await this.VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix does not copy a union declaration into the file extracted for another type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestCodeFixDoesNotCopyUnionAsync()
        {
            var testCode = @"public class Foo
{
}
public union Baz(int, string);
public class {|#0:Bar|}
{
}";

            var fixedSources = new[]
            {
                ("/0/Test0.cs", @"public class Foo
{
}
public union Baz(int, string);
"),
                ("Bar.cs", @"public class Bar
{
}"),
            };

            var settings = SA1402SettingsConfiguration.ConfigureAsNonTopLevelType.GetSettings("struct");
            var expected = this.Diagnostic().WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, settings, expected, fixedSources, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
