// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    public partial class SA1206CSharp9UnitTests : SA1206CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3975, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3975")]
        public async Task TestCovariantOverrideKeywordsOutOfOrderAsync()
        {
            var testCode = @"
public class BaseType
{
}

public class DerivedType : BaseType
{
}

public class BaseClass
{
    public virtual BaseType Create() => new BaseType();
}

public class DerivedClass : BaseClass
{
    override [|public|] DerivedType Create() => new DerivedType();
}
";

            var fixedCode = @"
public class BaseType
{
}

public class DerivedType : BaseType
{
}

public class BaseClass
{
    public virtual BaseType Create() => new BaseType();
}

public class DerivedClass : BaseClass
{
    public override DerivedType Create() => new DerivedType();
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
