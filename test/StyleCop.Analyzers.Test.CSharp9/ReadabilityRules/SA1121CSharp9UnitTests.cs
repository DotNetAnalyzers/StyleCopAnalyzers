// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
        StyleCop.Analyzers.ReadabilityRules.SA1121CodeFixProvider>;

    public partial class SA1121CSharp9UnitTests : SA1121CSharp8UnitTests
    {
        /// <summary>
        /// Verifies that fixing other built-in types does not change native-sized integers in C# 9.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFixAllPreservesNativeSizedIntegersInCSharp9Async()
        {
            var testCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static [|String|] Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));

    private static unsafe UIntPtr GetUnsignedAddress(object value) => new UIntPtr(Unsafe.AsPointer(ref value));
}";

            var fixedCode = @"
using System;
using System.Runtime.CompilerServices;

class TestClass
{
    private static string Format => ""{3}{0}.{1}{2} (target address: 0x{4:X"" + (IntPtr.Size * 2) + ""})"";

    private static unsafe IntPtr GetAddress(object value) => new IntPtr(Unsafe.AsPointer(ref value));

    private static unsafe UIntPtr GetUnsignedAddress(object value) => new UIntPtr(Unsafe.AsPointer(ref value));
}";

            await new CSharpTest(LanguageVersionEx.CSharp9)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task TestNativeSizedIntegersDoNotReportAsync()
        {
            // Explicitly test with C# 9 since C# 11 and later include separate coverage where these cases do report.
            var testCode = @"
using System;

class TestClass
{
    IntPtr field1;
    System.UIntPtr field2;
}";

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp9, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task TestNativeSizedIntegerAliasesDoNotReportAsync()
        {
            var testCode = @"
class TestClass
{
    nint field1;
    nuint field2;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3970, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3970")]
        public async Task TestFunctionPointerParametersAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    private unsafe delegate*<{|#0:System.Int64|}, {|#1:System.Int32|}> field1;
    private unsafe delegate*<{|#2:Int64|}, {|#3:Int32|}> field2;
}
";

            var fixedCode = @"using System;
public class TestClass
{
    private unsafe delegate*<long, int> field1;
    private unsafe delegate*<long, int> field2;
}
";

            var expected = new[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
