// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
        StyleCop.Analyzers.LayoutRules.SA1502ElementMustNotBeOnASingleLine,
        StyleCop.Analyzers.LayoutRules.SA1502CodeFixProvider>;

    public partial class SA1502CSharp9UnitTests : SA1502CSharp8UnitTests
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
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        public async Task TestPropertiesInitAccessorsAsync()
        {
            var testCode = @"public class Foo
{
    private bool b;

    public bool Bar
    {
        get { return true; }
        init { this.b = value; }
    }

    public bool Baz
    {
        get
        {
            return true;
        }

        init
        {
            this.b = value;
        }
    }

    public int AutoBar { get; init; } = 7;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a property with its block on the same line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyInitOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    public bool Bar [|{|] init { return; } }
}";
            var fixedTestCode = @"public class Foo
{
    public bool Bar
    {
        init { return; }
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a property with its block on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyInitWithBlockOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    public bool Bar
    [|{|] init { return; } }
}";
            var fixedTestCode = @"public class Foo
{
    public bool Bar
    {
        init { return; }
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
