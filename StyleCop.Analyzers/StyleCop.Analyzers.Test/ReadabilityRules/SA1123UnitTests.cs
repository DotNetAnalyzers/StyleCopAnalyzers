// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.ReadabilityRules.SA1123DoNotPlaceRegionsWithinElements,
        Analyzers.ReadabilityRules.RemoveRegionCodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1123DoNotPlaceRegionsWithinElements"/> and
    /// <see cref="RemoveRegionCodeFixProvider"/>.
    /// </summary>
    public class SA1123UnitTests
    {
        [Fact]
        public async Task TestRegionInMethodAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
#region Foo
        string test = """";
#endregion
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 1);

            string fixedCode = @"public class Foo
{
    public void Bar()
    {
        string test = """";
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionPartialyInMethodAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
#region Foo
        string test = """";
    }
#endregion
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionPartialyInMethod2Async()
        {
            var testCode = @"public class Foo
{
    public void Bar()
#region Foo
    {
        string test = """";
    }
#endregion
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionPartialyMultipleMethodsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
#region Foo
        string test = """";
    }
    public void FooBar()
    {
        string test = """";
#endregion
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEndRegionInMethodAsync()
        {
            var testCode = @"public class Foo
{
#region Foo
    public void Bar()
    {
        string test = """";
#endregion
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionOutsideMethodAsync()
        {
            var testCode = @"public class Foo
{
#region Foo
#endregion
    public void Bar()
    {
        string test = """";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionOutsideMethod2Async()
        {
            var testCode = @"public class Foo
{
#region Foo
    public void Bar()
    {
        string test = """";
    }
#endregion
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFixAllProviderAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        #region Foo
        #region Foo
        #region Foo
        #endregion
        #endregion
        #endregion
        #region Foo
        #region Foo
        #region Foo
        // Test
        #endregion
        #endregion
        #endregion
    }
}
";

            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        // Test
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 9),
                Diagnostic().WithLocation(7, 9),
                Diagnostic().WithLocation(8, 9),
                Diagnostic().WithLocation(12, 9),
                Diagnostic().WithLocation(13, 9),
                Diagnostic().WithLocation(14, 9),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
