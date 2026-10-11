// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    public partial class SA1206CSharp14UnitTests : SA1206CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestModifierOrderInExtensionBlockAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        static {|#0:public|} int GetZero() => 0;
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public static int GetZero() => 0;
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "static");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4030, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4030")]
        public async Task TestOverrideBeforeAccessModifierOnCompoundAssignmentOperatorAsync()
        {
            var testCode = @"
public class Base
{
    public int Value;

    public virtual void operator +=(int x)
    {
        this.Value += x;
    }
}

public class Derived : Base
{
    override {|#0:public|} void operator +=(int x)
    {
        this.Value += x * 2;
    }
}
";
            var fixedCode = @"
public class Base
{
    public int Value;

    public virtual void operator +=(int x)
    {
        this.Value += x;
    }
}

public class Derived : Base
{
    public override void operator +=(int x)
    {
        this.Value += x * 2;
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "override");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
