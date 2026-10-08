// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.OrderingRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1207ProtectedMustComeBeforeInternal,
        StyleCop.Analyzers.OrderingRules.SA1207CodeFixProvider>;

    public partial class SA1207CSharp9UnitTests : SA1207CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNestedRecordAsync(string keyword)
        {
            var testCode = $@"public class TestClass
{{
    internal {{|#0:protected|}} {keyword} TestRecord
    {{
    }}
}}
";

            var fixedCode = $@"public class TestClass
{{
    protected internal {keyword} TestRecord
    {{
    }}
}}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("protected", "internal");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorAndEnumAsync()
        {
            var testCode = @"public class TestClass
{
    internal {|#0:protected|} TestClass()
    {
    }

    internal {|#1:protected|} enum TestEnum
    {
    }
}
";

            var fixedCode = @"public class TestClass
{
    protected internal TestClass()
    {
    }

    protected internal enum TestEnum
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments("protected", "internal"),
                Diagnostic().WithLocation(1).WithArguments("protected", "internal"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
