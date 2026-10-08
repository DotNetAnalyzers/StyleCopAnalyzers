// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1400AccessModifierMustBeDeclared,
        StyleCop.Analyzers.MaintainabilityRules.SA1400CodeFixProvider>;

    public partial class SA1400CSharp11UnitTests : SA1400CSharp10UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.BaseTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3588, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3588")]
        public async Task TestTypeDeclarationWithFileModifierAsync(string typeName)
        {
            var testCode = $@"file {typeName} TypeName {{ }}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that static abstract and static virtual interface members, used for generic math, do not require an
        /// access modifier.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3994, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3994")]
        public async Task TestStaticAbstractInterfaceMembersAsync()
        {
            var testCode = @"public interface IAddable<TSelf>
    where TSelf : IAddable<TSelf>
{
    static abstract TSelf Zero { get; }

    static abstract TSelf operator +(TSelf left, TSelf right);

    static abstract explicit operator int(TSelf value);

    static virtual TSelf Twice(TSelf value) => value + value;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticResult[] GetExpectedResultTestTypeWithPrimaryConstructorWithoutAccessModifier()
        {
            return new[]
            {
                Diagnostic().WithLocation(0).WithArguments("TestType"),
            };
        }
    }
}
