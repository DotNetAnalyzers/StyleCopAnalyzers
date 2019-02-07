﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1131UseReadableConditions,
        StyleCop.Analyzers.ReadabilityRules.SA1131CodeFixProvider>;

    public class SA1131UnitTests
    {
        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        [InlineData(">=", "<=")]
        [InlineData("<=", ">=")]
        [InlineData(">", "<")]
        [InlineData("<", ">")]
        public async Task TestYodaComparismAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        const int j = 6;
        if (j {oldOperator} i) {{ }}
    }}
}}";
            var fixedCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        const int j = 6;
        if (i {newOperator} j) {{ }}
    }}
}}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(9, 13),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        [InlineData(">=", "<=")]
        [InlineData("<=", ">=")]
        [InlineData(">", "<")]
        [InlineData("<", ">")]
        public async Task TestYodaComparismAsAnArgumentAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        const int j = 6;
        Test(j {oldOperator} i);
    }}
    public void Test(bool argument) {{ }}
}}";
            var fixedCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        const int j = 6;
        Test(i {newOperator} j);
    }}
    public void Test(bool argument) {{ }}
}}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(9, 14),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        [InlineData(">=", "<=")]
        [InlineData("<=", ">=")]
        [InlineData(">", "<")]
        [InlineData("<", ">")]
        public async Task TestYodaComparismOutsideIfAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        const int j = 6;
        bool b = j {oldOperator} i;
    }}
}}";
            var fixedCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        const int j = 6;
        bool b = i {newOperator} j;
    }}
}}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(9, 18),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        [InlineData(">=", "<=")]
        [InlineData("<=", ">=")]
        [InlineData(">", "<")]
        [InlineData("<", ">")]
        public async Task TestDefaultComparismOutsideIfAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        bool b = default(int) {oldOperator} i;
    }}
}}";
            var fixedCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        bool b = i {newOperator} default(int);
    }}
}}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 18),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        [InlineData(">=", "<=")]
        [InlineData("<=", ">=")]
        [InlineData(">", "<")]
        [InlineData("<", ">")]
        public async Task TestStaticReadOnlyAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    static readonly int j = 5;
    public void Test()
    {{
        int i = 5;
        bool b = j {oldOperator} i;
    }}
}}";
            var fixedCode = $@"
using System;
public class TypeName
{{
    static readonly int j = 5;
    public void Test()
    {{
        int i = 5;
        bool b = i {newOperator} j;
    }}
}}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(9, 18),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        public async Task TestDefaultStringComparismOutsideIfAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        string i = ""x"";
        bool b = default(string) {oldOperator} i;
    }}
}}";
            var fixedCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        string i = ""x"";
        bool b = i {newOperator} default(string);
    }}
}}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 18),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        public async Task TestNullComparismOutsideIfAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        string i = ""x"";
        bool b = null {oldOperator} i;
    }}
}}";
            var fixedCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        string i = ""x"";
        bool b = i {newOperator} null;
    }}
}}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 18),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        public async Task TestDefaultStructComparismOutsideIfAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        TestStruct i = default(TestStruct);
        bool b = default(TestStruct) {oldOperator} i;
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
        TestStruct i = default(TestStruct);
        bool b = i {newOperator} default(TestStruct);
    }}
}}

struct TestStruct 
{{
    public static bool operator == (TestStruct a, TestStruct b) {{ return true; }}
    public static bool operator != (TestStruct a, TestStruct b) {{ return false; }}
}}
";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 18),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCorrectComparismAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        const int i = 5;
        int j = 6;
        if (j == i) { }
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData(">=")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData("<")]
        public async Task TestCorrectComparismOutsideIfAsync(string @operator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        const int i = 5;
        int j = 6;
        bool b = j {@operator} i;
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData(">=")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData("<")]
        public async Task TestStaticFieldAsync(string @operator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    static int i = 5;
    public void Test()
    {{
        int j = 6;
        bool b = j {@operator} i;
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData(">=")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData("<")]
        public async Task TestReadOnlyFieldAsync(string @operator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    readonly int i = 5;
    public void Test()
    {{
        int j = 6;
        bool b = j {@operator} i;
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData(">=")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData("<")]
        public async Task TestNormalFieldAsync(string @operator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    int i = 5;
    public void Test()
    {{
        int j = 6;
        bool b = j {@operator} i;
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData(">=")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData("<")]
        public async Task TestCorrectComparismNoConstantAsync(string @operator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        int j = 6;
        if (j {@operator} i) {{ }}
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData(">=")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData("<")]
        public async Task TestCorrectComparismOutsideIfNoConstantAsync(string @operator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        int j = 6;
        bool b = j {@operator} i;
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("==", "==")]
        [InlineData("!=", "!=")]
        [InlineData(">=", "<=")]
        [InlineData("<=", ">=")]
        [InlineData(">", "<")]
        [InlineData("<", ">")]
        public async Task TestCommentsAsync(string oldOperator, string newOperator)
        {
            var testCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        const int j = 6;
        bool b = /*1*/j/*2*/{oldOperator}/*3*/i/*4*/;
    }}
}}";
            var fixedCode = $@"
using System;
public class TypeName
{{
    public void Test()
    {{
        int i = 5;
        const int j = 6;
        bool b = /*1*/i/*2*/{newOperator}/*3*/j/*4*/;
    }}
}}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(9, 23),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }
    }
}
