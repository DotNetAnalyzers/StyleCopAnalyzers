// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.ReadabilityRules.SA1131UseReadableConditions,
        Analyzers.ReadabilityRules.SA1131CodeFixProvider>;

    public class SA1131CSharp7UnitTests : SA1131UnitTests
    {
        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        [WorkItem(2675, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2675")]
        public async Task TestDefaultLiteralStructComparismOutsideIfAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        TestStruct i = default;
        bool b = default {oldOperator} i;
    }}
}}

struct TestStruct 
{{
    public static bool operator == (TestStruct a, TestStruct b) {{ return true; }}
    public static bool operator != (TestStruct a, TestStruct b) {{ return false; }}
}}
";
            var fixedCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        TestStruct i = default;
        bool b = i {newOperator} default;
    }}
}}

struct TestStruct 
{{
    public static bool operator == (TestStruct a, TestStruct b) {{ return true; }}
    public static bool operator != (TestStruct a, TestStruct b) {{ return false; }}
}}
";
            DiagnosticResult expected = Diagnostic().WithLocation(8, 18);
            await VerifyCSharpFixAsync(LanguageVersion.Latest, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
