// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1513ClosingBraceMustBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1513CodeFixProvider>;

    public partial class SA1513CSharp9UnitTests : SA1513CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3410, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3410")]
        public async Task TestThrowSwitchExpressionValueAsync()
        {
            var testCode = @"using System;

public class Foo
{
    public void Baz(string arg)
    {
        throw arg switch
        {
            """" => new ArgumentException(),
            _ => new Exception()
        };
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3658, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3658")]
        public async Task TestInitAccessorAsync()
        {
            var testCode = @"using System;

public class Foo
{
    public int X
    {
        get
        {
            return 0;
        }
        init
        {
        }
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        public async Task TestBlankLineAfterInitAccessorAsync()
        {
            var testCode = @"using System;

public class Foo
{
    public int X
    {
        get
        {
            return 0;
        }

        init
        {
        }
    }{|#0:
|}    public void Bar()
    {
    }
}
";

            var fixedCode = @"using System;

public class Foo
{
    public int X
    {
        get
        {
            return 0;
        }

        init
        {
        }
    }

    public void Bar()
    {
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
