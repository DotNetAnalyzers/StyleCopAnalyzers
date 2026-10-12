// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1107CodeMustNotContainMultipleStatementsOnOneLine,
        StyleCop.Analyzers.ReadabilityRules.SA1107CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1107CodeMustNotContainMultipleStatementsOnOneLine"/> and
    /// <see cref="SA1107CodeFixProvider"/>.
    /// </summary>
    public class SA1107UnitTests
    {
        [Fact]
        public async Task TestCorrectCodeAsync()
        {
            string testCode = @"
using System;
class ClassName
{
    public static void Foo(string a, string b) 
    {
        int i = 5;
        int j = 6, k = 3;
        if(true)
        {
            i++;
        }
        else
        {
            j++;
        }
        Foo(""a"", ""b"");

        Func<int, int, int> f = (c, d) => c + d;
        Func<int, int, int> g = (c, d) => { return c + d; };
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestWrongCodeAsync(string lineEnding)
        {
            string testCode = @"
using System;
class ClassName
{
    public static void Foo(string a, string b)
    {
        int i = 5; {|#0:int j = 6, k = 3;|} {|#1:if(true)
        {
            i++;
        }
        else
        {
            j++;
        }|} {|#2:Foo(""a"", ""b"");|}

        Func<int, int, int> g = (c, d) => { c++; {|#3:return c + d;|} };
    }
}
".ReplaceLineEndings(lineEnding);
            var expected = new[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),
            };

            string fixedCode = @"
using System;
class ClassName
{
    public static void Foo(string a, string b)
    {
        int i = 5;
        int j = 6, k = 3;
        if(true)
        {
            i++;
        }
        else
        {
            j++;
        }
        Foo(""a"", ""b"");

        Func<int, int, int> g = (c, d) => { c++;
        return c + d; };
    }
}
".ReplaceLineEndings(lineEnding);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatAnalyzerDoesntCrashOnEmptyBlockAsync()
        {
            string testCode = @"
using System;
class ClassName
{
    public static void Foo(string a, string b)
    {
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatAnalyzerIgnoresStatementsWithMissingTokenAsync()
        {
            string testCode = @"
using System;
class ClassName
{
    public static void Foo(string a, string b)
    {
        int i
        if (true)
        {
            Console.WriteLine(""Bar"");
        }
    }
}
";
            DiagnosticResult expected = DiagnosticResult.CompilerError("CS1002").WithLocation(7, 14).WithMessage("; expected");
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2862, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2862")]
        public async Task TestEmptyStatementAfterBlockAsync()
        {
            string testCode = @"
class Program
{
    static void Main(string[] args)
    {
        {
        }[|;|]
    }
}
";
            string fixedCode = @"
class Program
{
    static void Main(string[] args)
    {
        {
        }
        ;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
