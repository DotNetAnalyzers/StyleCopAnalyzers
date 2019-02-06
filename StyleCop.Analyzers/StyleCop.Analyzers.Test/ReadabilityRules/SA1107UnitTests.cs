// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
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

        [Fact]
        public async Task TestWrongCodeAsync()
        {
            string testCode = @"
using System;
class ClassName
{
    public static void Foo(string a, string b)
    {
        int i = 5; int j = 6, k = 3; if(true)
        {
            i++;
        }
        else
        {
            j++;
        } Foo(""a"", ""b"");

        Func<int, int, int> g = (c, d) => { c++; return c + d; };
    }
}
";
            var expected = new[]
            {
                Diagnostic().WithLocation(7, 20),
                Diagnostic().WithLocation(7, 38),
                Diagnostic().WithLocation(14, 11),
                Diagnostic().WithLocation(16, 50),
            };

            string fixedCode = @"
using System;
class ClassName
{
    public static void Foo(string a, string b)
    {
        int i = 5;
        int j = 6, k = 3;
        if (true)
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
";

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
    }
}
