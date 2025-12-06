// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1300ElementMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public partial class SA1300CSharp10UnitTests : SA1300CSharp9UnitTests
    {
        [Fact]
        public async Task TestUpperCaseFileScopedNamespaceAsync()
        {
            var testCode = @"namespace Test;";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseFileScopedNamespaceAsync()
        {
            var testCode = @"namespace {|#0:test|};";

            var fixedCode = @"namespace Test;";

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAllowedLowerCaseFileScopedNamespaceIsNotReportedAsync()
        {
            var customTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowedNamespaceComponents"": [ ""eBay"" ]
    }
  }
}
";

            var testCode = @"namespace eBay;";

            await new CSharpTest
            {
                TestCode = testCode,
                Settings = customTestSettings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseComplicatedFileScopedNamespaceAsync()
        {
            var testCode = @"namespace {|#0:test|}.{|#1:foo|}.{|#2:bar|};";

            var fixedCode = @"namespace Test.Foo.Bar;";

            DiagnosticResult[] expected = new[]
            {
                Diagnostic().WithArguments("test").WithLocation(0),
                Diagnostic().WithArguments("foo").WithLocation(1),
                Diagnostic().WithArguments("bar").WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAllowedLowerCaseComplicatedFileScopedNamespaceIsNotReportedAsync()
        {
            var customTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowedNamespaceComponents"": [ ""iPod"" ]
    }
  }
}
";

            var testCode = @"namespace Apple.iPod.Library;";

            await new CSharpTest
            {
                TestCode = testCode,
                Settings = customTestSettings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3979, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3979")]
        public async Task TestRecordStructNameMustStartWithUpperCaseLetterAsync()
        {
            var testCode = @"
public record struct {|#0:r|}(int A)
{
    public {|#1:r|}(int a, int b)
        : this(A: a)
    {
    }
}
";

            var fixedCode = @"
public record struct R(int A)
{
    public R(int a, int b)
        : this(A: a)
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments("r"),
                Diagnostic().WithLocation(1).WithArguments("r"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
