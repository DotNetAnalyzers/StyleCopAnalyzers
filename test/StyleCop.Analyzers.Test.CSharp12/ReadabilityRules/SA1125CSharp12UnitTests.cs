// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1125UseShorthandForNullableTypes>;

    public partial class SA1125CSharp12UnitTests : SA1125CSharp11UnitTests
    {
        [Theory]
        [InlineData("System.Nullable<int>")]
        [InlineData("global::System.Nullable<int>")]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingAliasTargetAsync(string form)
        {
            string testCode = $@"
using MyNullableInt = {{|#0:{form}|}};

public class TestClass
{{
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingAliasTargetTypeArgumentAsync()
        {
            string testCode = @"
using MyList = System.Collections.Generic.List<{|#0:System.Nullable<int>|}>;

public class TestClass
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingStaticDirectiveAsync()
        {
            // Using static directives still require a type name, so the shorthand is not available there.
            string testCode = @"
using static System.Nullable<int>;

public class TestClass
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestUsingAliasTargetBeforeCSharp12Async()
        {
            string testCode = @"
using MyNullableInt = System.Nullable<int>;
using MyList = System.Collections.Generic.List<System.Nullable<int>>;

public class TestClass
{
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp11, testCode, settings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
