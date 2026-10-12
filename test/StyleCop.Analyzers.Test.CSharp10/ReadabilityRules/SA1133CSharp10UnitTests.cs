// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1133DoNotCombineAttributes,
        StyleCop.Analyzers.ReadabilityRules.SA1133CodeFixProvider>;

    public partial class SA1133CSharp10UnitTests : SA1133CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3987, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3987")]
        public async Task TestCombinedAttributesOnLambdaExpressionsAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var a = [A, {|#0:B|}] () => 0;
        var b = [return: A, {|#1:B|}] int () => 0;
        this.Use([A, {|#2:B|}] (int x) => x);
    }

    private void Use(Func<int, int> f)
    {
    }
}

[AttributeUsage(AttributeTargets.All)]
public class AAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.All)]
public class BAttribute : Attribute
{
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var a = [A] [B] () => 0;
        var b = [return: A] [return: B] int () => 0;
        this.Use([A] [B] (int x) => x);
    }

    private void Use(Func<int, int> f)
    {
    }
}

[AttributeUsage(AttributeTargets.All)]
public class AAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.All)]
public class BAttribute : Attribute
{
}
";

            var expected = new[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
