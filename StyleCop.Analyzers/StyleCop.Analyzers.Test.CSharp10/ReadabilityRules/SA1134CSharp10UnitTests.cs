// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1134AttributesMustNotShareLine,
        StyleCop.Analyzers.ReadabilityRules.SA1134CodeFixProvider>;

    public partial class SA1134CSharp10UnitTests : SA1134CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3623, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3623")]
        [WorkItem(3987, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3987")]
        public async Task TestAttributesOnLambdaExpressionsAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var a = [A] () => 0;
        var b = [A][B] () => 0;
        var c = [A] [B] (int x) => x;
        var d = [A, B] () => 0;
        var e = [return: A] int () => 0;
        var f = [A] static async () => await System.Threading.Tasks.Task.Yield();
        Func<int, int> g = [A] (x) => x;
        this.Use([A] (int x) => x);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3987, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3987")]
        public async Task TestAttributeOnLocalFunctionInsideLambdaAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        Action a = [A] () =>
        {
            {|#0:[|}B] void LocalFunction()
            {
            }
        };
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
        Action a = [A] () =>
        {
            [B]
            void LocalFunction()
            {
            }
        };
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

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
