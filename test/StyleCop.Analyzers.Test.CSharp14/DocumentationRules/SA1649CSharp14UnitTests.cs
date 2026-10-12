// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName>;

    public partial class SA1649CSharp14UnitTests : SA1649CSharp13UnitTests
    {
        [Fact]
        public async Task VerifyExtensionBlockInWrongFileNameAsync()
        {
            var testCode = @"public static class {|#0:TestExtensions|}
{
    extension(object o)
    {
        public bool IsNull => o is null;
    }
}
";
            var fixedCode = @"public static class TestExtensions
{
    extension(object o)
    {
        public bool IsNull => o is null;
    }
}
";

            var test = new StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName, StyleCop.Analyzers.DocumentationRules.SA1649CodeFixProvider>.CSharpTest()
            {
                TestSources = { ("WrongFileName.cs", testCode) },
                FixedSources = { ("TestExtensions.cs", fixedCode) },
            };

            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a top-level extension block (a compile error, but common while typing) does not crash the
        /// analyzer or produce a diagnostic, because it has no type name for the file name to match.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyTopLevelExtensionBlockIsIgnoredAsync()
        {
            var testCode = @"extension(object o)
{
}
";

            var test = new StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName, StyleCop.Analyzers.DocumentationRules.SA1649CodeFixProvider>.CSharpTest()
            {
                TestSources = { ("WrongFileName.cs", testCode) },
                CompilerDiagnostics = CompilerDiagnostics.None,
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyTopLevelExtensionBlockBeforeClassInWrongFileNameAsync()
        {
            var testCode = @"extension(object o)
{
}

public class {|#0:TestClass|}
{
}
";
            var fixedCode = @"extension(object o)
{
}

public class TestClass
{
}
";

            var test = new StyleCopCodeFixVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName, StyleCop.Analyzers.DocumentationRules.SA1649CodeFixProvider>.CSharpTest()
            {
                TestSources = { ("WrongFileName.cs", testCode) },
                FixedSources = { ("TestClass.cs", fixedCode) },
                CompilerDiagnostics = CompilerDiagnostics.None,
            };

            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
