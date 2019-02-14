// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.ReadabilityRules.SA1141UseTupleSyntax,
        Analyzers.ReadabilityRules.SA1141CodeFixProvider>;

    /// <summary>
    /// This class contains the CSharp 7.x unit tests for SA1141.
    /// </summary>
    /// <seealso cref="SA1141UseTupleSyntax"/>
    /// <seealso cref="SA1141CodeFixProvider"/>
    public class SA1141CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that member declarations containing ValueTuple will result in the proper diagnostics and fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateMemberDeclarationsWithValueTuplesAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public ValueTuple<int, int> TestMethod(ValueTuple<double, double> value)
    {
        throw new NotImplementedException();
    }

    public System.ValueTuple<(int, int), int> TestMethod2(int p1, ValueTuple<System.ValueTuple<long, long>, long> p2, (ValueTuple<string, string>, string) p3)
    {
        throw new NotImplementedException();
    }

    public System.ValueTuple<int, int> TestProperty1 { get; set; }

    public System.Collections.Generic.List<ValueTuple<int, int>> TestProperty2 { get; set; }

    public System.ValueTuple<int, long> this[int i] { get { return (1, 1l); } set { } }

    public static explicit operator TestClass(System.ValueTuple<int, int> p1)
    {
        throw new NotImplementedException();
    }

    public static implicit operator System.ValueTuple<int, int>(TestClass p1)
    {
        throw new NotImplementedException();
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public (int, int) TestMethod((double, double) value)
    {
        throw new NotImplementedException();
    }

    public ((int, int), int) TestMethod2(int p1, ((long, long), long) p2, ((string, string), string) p3)
    {
        throw new NotImplementedException();
    }

    public (int, int) TestProperty1 { get; set; }

    public System.Collections.Generic.List<(int, int)> TestProperty2 { get; set; }

    public (int, long) this[int i] { get { return (1, 1l); } set { } }

    public static explicit operator TestClass((int, int) p1)
    {
        throw new NotImplementedException();
    }

    public static implicit operator (int, int)(TestClass p1)
    {
        throw new NotImplementedException();
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(5, 12),
                Diagnostic().WithLocation(5, 44),
                Diagnostic().WithLocation(10, 12),
                Diagnostic().WithLocation(10, 67),
                Diagnostic().WithLocation(10, 120),
                Diagnostic().WithLocation(15, 12),
                Diagnostic().WithLocation(17, 44),
                Diagnostic().WithLocation(19, 12),
                Diagnostic().WithLocation(21, 47),
                Diagnostic().WithLocation(26, 37),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that ValueTuple object creation expressions will produce the expected diagnostics and fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateValueTupleObjectCreationAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var test1 = new ValueTuple<int, int>(1, 2);
        var test2 = new System.ValueTuple<int, int>(1, 2);
        var test3 = new ValueTuple<ValueTuple<int, int>, int>(new ValueTuple<int, int>(3, 4), 2);
        var test4 = new System.ValueTuple<int, System.ValueTuple<int, int>>(1, new System.ValueTuple<int, int>(2, 3));
        var test5 = (new ValueTuple<int, int>(3, 4), 2);
        var test6 = new System.ValueTuple<int, System.ValueTuple<int, int>>(1, (2, 3));
        var test7 = ValueTuple.Create(1, 2);
        var test8 = ValueTuple.Create<int, double>(1, 2);
        var test9 = System.ValueTuple.Create(1, new ValueTuple<int, double>(2, 3));
        var test10 = ValueTuple.Create(ValueTuple.Create(1, 2, 3), 4);
        var test11 = new ValueTuple<int, ValueTuple<int, int>>(1, ValueTuple.Create(2, 3));
        var test12 = new System.ValueTuple<byte, int>(1, 2);
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var test1 = (1, 2);
        var test2 = (1, 2);
        var test3 = ((3, 4), 2);
        var test4 = (1, (2, 3));
        var test5 = ((3, 4), 2);
        var test6 = (1, (2, 3));
        var test7 = (1, 2);
        var test8 = (1, (double)2);
        var test9 = (1, (2, (double)3));
        var test10 = ((1, 2, 3), 4);
        var test11 = (1, (2, 3));
        var test12 = ((byte)1, 2);
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(7, 21),
                Diagnostic().WithLocation(8, 21),
                Diagnostic().WithLocation(9, 21),
                Diagnostic().WithLocation(9, 63),
                Diagnostic().WithLocation(10, 21),
                Diagnostic().WithLocation(10, 80),
                Diagnostic().WithLocation(11, 22),
                Diagnostic().WithLocation(12, 21),
                Diagnostic().WithLocation(13, 21),
                Diagnostic().WithLocation(14, 21),
                Diagnostic().WithLocation(15, 21),
                Diagnostic().WithLocation(15, 49),
                Diagnostic().WithLocation(16, 22),
                Diagnostic().WithLocation(16, 40),
                Diagnostic().WithLocation(17, 22),
                Diagnostic().WithLocation(17, 67),
                Diagnostic().WithLocation(18, 22),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that the usage of <see cref="System.ValueTuple"/> within pattern matching will produce no diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateValueTupleUsageInPatternMatchingAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod(object x)
    {
        switch (x)
        {
        case System.ValueTuple<int, long> a:
            break;

        case ValueTuple<double, ValueTuple<int, int>> b:
            break;
        }

        if (x is ValueTuple<byte, byte> c)
        {
        }

        if (x is System.ValueTuple<System.ValueTuple<bool, bool, bool>, long> d)
        {
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3055, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3055")]
        public async Task ValidateSingleElementValueTupleUsageAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var test1 = default(ValueTuple<int>);
        var test2 = default(ValueTuple<ValueTuple<int>>);
        ValueTuple<int> test3 = ValueTuple.Create(3);
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that the usage of <see cref="System.ValueTuple"/> within exception filtering will produce no diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateValueTupleUsageInExceptionFiltersAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public string TestMethod(object x)
    {
        try
        {
            return x.ToString();
        }
        catch (TestException e) when (e.AdditionalInfo is ValueTuple<string, string>)
        {
            return null;
        }
    }
}

public class TestException : Exception
{
    public object AdditionalInfo { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that the usage of <see cref="System.ValueTuple"/> within type casts will produce the expected diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateValueTupleUsageInTypeCastsAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod(object input)
    {
        var test1 = (ValueTuple<int, int>)input;
        var test2 = (System.ValueTuple<System.ValueTuple<int, long>, byte>)input;
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public void TestMethod(object input)
    {
        var test1 = ((int, int))input;
        var test2 = (((int, long), byte))input;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(7, 22),
                Diagnostic().WithLocation(8, 22),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that the usage of <see cref="System.ValueTuple"/> within a default expression will produce the expected diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateValueTupleUsageInDefaultExpressionAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var test1 = default(ValueTuple<int, int>);
        var test2 = default(System.ValueTuple<System.ValueTuple<int, long>, byte>);
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var test1 = default((int, int));
        var test2 = default(((int, long), byte));
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(7, 29),
                Diagnostic().WithLocation(8, 29),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that the usage of <see cref="System.ValueTuple"/> within a delegate will produce the expected diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateValueTupleUsageInDelegateAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public delegate System.ValueTuple<int, bool> TestDelegate(ValueTuple<int, ValueTuple<int, long>> arg1, (long, double) arg2, (long, System.ValueTuple<bool, bool>) arg3);
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public delegate (int, bool) TestDelegate((int, (int, long)) arg1, (long, double) arg2, (long, (bool, bool)) arg3);
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(5, 21),
                Diagnostic().WithLocation(5, 63),
                Diagnostic().WithLocation(5, 136),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
