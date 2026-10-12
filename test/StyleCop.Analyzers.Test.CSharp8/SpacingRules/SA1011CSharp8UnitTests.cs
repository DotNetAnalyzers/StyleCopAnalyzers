// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1011ClosingSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1011CSharp8UnitTests : SA1011CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task VerifyNullableArrayAnnotationsAsync()
        {
            var testCode = @"#nullable enable
namespace TestNamespace
{
    public class TestClass
    {
        private string[]? field1;
        private string[]?[]? field2;
        private string?[][]? field3;

        public string[]? Property1 => field1;

        public string[]?[]? Property2
        {
            get
            {
                return new string[]?[] { field1, field2?[0] };
            }
        }

        public string?[] Method(string?[]? values, string[]?[]? other)
        {
            return values ?? new string?[] { null, other?[0]?[0] };
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task VerifyNullForgivingAfterElementAccessAndArrayCreationAsync()
        {
            var testCode = @"#nullable enable
namespace TestNamespace
{
    public class TestClass
    {
        public string GetValue(string[]? values)
        {
            return values![0]!;
        }

        public int GetLength(string?[] items)
        {
            return new string?[0]!.Length + items[0]! .Length;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that declaring a null reference type works for arrays.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2927, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2927")]
        public async Task VerifyNullableContextWithArraysAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            byte[]? data = null;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2900, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2900")]
        public async Task VerifyNullableContextWithArrayReturnsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public byte[]? TestMethod()
        {
            return null;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3052, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3052")]
        public async Task TestClosingSquareBracketFollowedByExclamationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(object?[] arguments)
        {
            object o1 = arguments[0] !;
            object o2 = arguments[0]! ;
            object o3 = arguments[0] ! ;
            string s1 = arguments[0] !.ToString();
            string s2 = arguments[0]! .ToString();
            string s3 = arguments[0] ! .ToString();
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(object?[] arguments)
        {
            object o1 = arguments[0]!;
            object o2 = arguments[0]! ;
            object o3 = arguments[0]! ;
            string s1 = arguments[0]!.ToString();
            string s2 = arguments[0]! .ToString();
            string s3 = arguments[0]! .ToString();
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "followed").WithLocation(7, 36),
                Diagnostic().WithArguments(" not", "followed").WithLocation(9, 36),
                Diagnostic().WithArguments(" not", "followed").WithLocation(10, 36),
                Diagnostic().WithArguments(" not", "followed").WithLocation(12, 36),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3708, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3708")]
        public async Task TestClosingSquareBracketFollowedByRangeAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int[] arg)
        {
            _ = arg[0]..;
            _ = arg[0] ..;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3008, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3008")]
        public async Task TestIndexAndRangeClosingBracketSpacingAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int[] values)
        {
            _ = values[^1 {|#0:]|};
            _ = values[1..^2 {|#1:]|};
            _ = values[.. {|#2:]|};
            _ = values[^1{|#3:]|}^1;
            _ = values[^1] ^1;
        }
    }
}
";
            var fixedCode = @"
namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int[] values)
        {
            _ = values[^1];
            _ = values[1..^2];
            _ = values[..];
            _ = values[^1] ^1;
            _ = values[^1] ^1;
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(0),
                Diagnostic().WithArguments(" not", "preceded").WithLocation(1),
                Diagnostic().WithArguments(" not", "preceded").WithLocation(2),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(3),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
