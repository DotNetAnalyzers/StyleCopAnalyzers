﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.LayoutRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    public partial class SA1500CSharp9UnitTests : SA1500CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3272, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3272")]
        public async Task TestSingleLineRecordAsync(string keyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {keyword} TestRecord;
}}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3272, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3272")]
        public async Task TestSingleLineRecordWithParameterAsync(string keyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {keyword} TestRecord(int Count);
}}
";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                FixedCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3272, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3272")]
        public async Task TestMultiLineRecordAsync(string keyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {keyword} TestRecord
    {{
        public int Count {{ get; init; }}
    }}
}}
";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                FixedCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3272, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3272")]
        public async Task TestMultiLineRecordWithParameterAsync(string keyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {keyword} TestRecord(int Count)
    {{
        public int Count2 {{ get; init; }} = 0;
    }}
}}
";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                FixedCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3667, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3667")]
        public async Task TestInitAccessorAsync()
        {
            var testCode = @"
class TestClass
{
    int Property1
    {
        init
        [|{|] }
    }

    int Property2
    {
        init [|{|]
        }
    }
}";

            var fixedCode = @"
class TestClass
{
    int Property1
    {
        init { }
    }

    int Property2
    {
        init
        {
        }
    }
}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
