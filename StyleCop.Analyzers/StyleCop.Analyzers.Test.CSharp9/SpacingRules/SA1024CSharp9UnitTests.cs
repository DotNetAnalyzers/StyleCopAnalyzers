// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1024CSharp9UnitTests : SA1024CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3248, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3248")]
        public async Task TestRecordInheritanceAsync()
        {
            const string testCode = @"
public abstract record BaseQuery<T>;
public record MyQuery1(){|#0::|}BaseQuery<object>;
public record MyQuery2(){|#1::|} BaseQuery<object>;
public record MyQuery3() {|#2::|}BaseQuery<object>;";
            const string fixedCode = @"
public abstract record BaseQuery<T>;
public record MyQuery1() : BaseQuery<object>;
public record MyQuery2() : BaseQuery<object>;
public record MyQuery3() : BaseQuery<object>;";

            await new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                ExpectedDiagnostics =
                {
                    // /0/Test0.cs(3,25): warning SA1024: Colon should be preceded by a space
                    Diagnostic(DescriptorPreceded).WithLocation(0),

                    // /0/Test0.cs(3,25): warning SA1024: Colon should be followed by a space
                    Diagnostic(DescriptorFollowed).WithLocation(0),

                    // /0/Test0.cs(4,25): warning SA1024: Colon should be preceded by a space
                    Diagnostic(DescriptorPreceded).WithLocation(1),

                    // /0/Test0.cs(5,26): warning SA1024: Colon should be followed by a space
                    Diagnostic(DescriptorFollowed).WithLocation(2),
                },
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
