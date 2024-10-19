﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Helpers.LanguageVersionTestExtensions;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1013ClosingBracesMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1013CSharp7UnitTests : SA1013UnitTests
    {
        /// <summary>
        /// Verifies spacing around a <c>}</c> character in tuple expressions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1001CSharp7UnitTests.TestSpacingAroundClosingBraceInTupleExpressionsAsync"/>
        /// <seealso cref="SA1009CSharp7UnitTests.TestSpacingAroundClosingBraceInTupleExpressionsAsync"/>
        [Fact]
        public async Task TestSpacingAroundClosingBraceInTupleExpressionsAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        var values = (new[] { 3{|#0:}|} , new[] { 3{|#1:}|} );
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        var values = (new[] { 3 } , new[] { 3 } );
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(1).WithArguments(string.Empty, "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc int[] { 1 , 1 } ;
            int* data2 = stackalloc int[] { 1 , 1 };
            int* data3 = stackalloc int[] { 1 , 1{|#0:}|} ;
            int* data4 = stackalloc int[] { 1 , 1{|#1:}|};
            int* data5 = stackalloc int[] { 1 , 1
};
            int* data6 = stackalloc int[]
            { 1 , 1{|#2:}|};
            int* data7 = stackalloc int[]
            {
                1 , 1 } ;
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc int[] { 1 , 1 } ;
            int* data2 = stackalloc int[] { 1 , 1 };
            int* data3 = stackalloc int[] { 1 , 1 } ;
            int* data4 = stackalloc int[] { 1 , 1 };
            int* data5 = stackalloc int[] { 1 , 1
};
            int* data6 = stackalloc int[]
            { 1 , 1 };
            int* data7 = stackalloc int[]
            {
                1 , 1 } ;
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(1).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(2).WithArguments(string.Empty, "preceded"),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3.OrLaterDefault(), testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestImplicitStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc[] { 1 , 1 } ;
            int* data2 = stackalloc[] { 1 , 1 };
            int* data3 = stackalloc[] { 1 , 1{|#0:}|} ;
            int* data4 = stackalloc[] { 1 , 1{|#1:}|};
            int* data5 = stackalloc[] { 1 , 1
};
            int* data6 = stackalloc[]
            { 1 , 1{|#2:}|};
            int* data7 = stackalloc[]
            {
                1 , 1 } ;
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc[] { 1 , 1 } ;
            int* data2 = stackalloc[] { 1 , 1 };
            int* data3 = stackalloc[] { 1 , 1 } ;
            int* data4 = stackalloc[] { 1 , 1 };
            int* data5 = stackalloc[] { 1 , 1
};
            int* data6 = stackalloc[]
            { 1 , 1 };
            int* data7 = stackalloc[]
            {
                1 , 1 } ;
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(1).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(2).WithArguments(string.Empty, "preceded"),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3.OrLaterDefault(), testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
