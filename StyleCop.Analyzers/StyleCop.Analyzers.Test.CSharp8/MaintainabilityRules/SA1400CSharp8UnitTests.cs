// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1400AccessModifierMustBeDeclared,
        StyleCop.Analyzers.MaintainabilityRules.SA1400CodeFixProvider>;

    public partial class SA1400CSharp8UnitTests : SA1400CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestDefaultInterfaceImplementationWithoutAccessModifierAsync()
        {
            var testCode = @"
public interface ITest
{
    void M()
    {
    }

    int P
    {
        get
        {
            return 0;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestStaticInterfaceMemberWithoutAccessModifierAsync()
        {
            var testCode = @"
public interface ITest
{
    static void M()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestDelegateDeclarationInsideInterfaceAsync()
        {
            var testCode = @"
public interface ITest
{
    delegate void Handler();
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestStaticFieldInsideInterfaceAsync()
        {
            var testCode = @"
public interface ITest
{
    static int Value = 0;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestTypeDeclarationInsideInterfaceAsync(string typeKind)
        {
            var testCode = $@"
public interface ITest
{{
    {typeKind} NestedType
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
