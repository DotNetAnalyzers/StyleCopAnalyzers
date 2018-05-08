// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;

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
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 18),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var newProject = base.ApplyCompilationOptions(project);

            var parseOptions = (CSharpParseOptions)newProject.ParseOptions;

            return newProject.WithParseOptions(parseOptions.WithLanguageVersion(LanguageVersion.Latest));
        }
    }
}
