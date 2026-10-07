// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1130UseLambdaSyntax,
        StyleCop.Analyzers.ReadabilityRules.SA1130CodeFixProvider>;

    public partial class SA1130CSharp8UnitTests : SA1130CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestDelegateInNullableEnabledGenericAsync()
        {
            var testCode = @"#nullable enable
using System;

public class TestClass
{
    public void TestMethod<T>() where T : class
    {
        Func<T?> resolve = {|#0:delegate|} { return default; };
    }
}
";

            var fixedCode = @"#nullable enable
using System;

public class TestClass
{
    public void TestMethod<T>() where T : class
    {
        Func<T?> resolve = () => { return default; };
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestDelegateUsedAsNamedArgumentWithNullableTypesAsync()
        {
            var testCode = @"#nullable enable
using System;
using System.Linq;

public class TestClass
{
    public void Test()
    {
        Invoke(resolve: {|#0:delegate|}
        {
            return """";
        });
    }

    private void Invoke(string? description = null, Func<object?, string?> resolve = null!)
    {
        _ = resolve(0);
    }
}
";

            var fixedCode = @"#nullable enable
using System;
using System.Linq;

public class TestClass
{
    public void Test()
    {
        Invoke(resolve: arg =>
        {
            return """";
        });
    }

    private void Invoke(string? description = null, Func<object?, string?> resolve = null!)
    {
        _ = resolve(0);
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
