// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists,
        StyleCop.Analyzers.ReadabilityRules.SA1100CodeFixProvider>;

    public partial class SA1100CSharp9UnitTests : SA1100CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3975, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3975")]
        public async Task TestCovariantOverrideAllowsBaseCallWhenOverrideExistsAsync()
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
    protected virtual BaseType Create() => new BaseType();
}

public class DerivedClass : BaseClass
{
    protected override DerivedType Create()
    {
        var value = base.Create();
        return new DerivedType();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3975, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3975")]
        public async Task TestBaseCallFlaggedWhenOverrideOnlyInIntermediateTypeAsync()
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
    protected virtual BaseType Create() => new BaseType();
}

public class IntermediateClass : BaseClass
{
    protected override DerivedType Create() => new DerivedType();
}

public class DerivedClass : IntermediateClass
{
    public BaseType CreateThroughBase()
    {
        return [|base|].Create();
    }
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
    protected virtual BaseType Create() => new BaseType();
}

public class IntermediateClass : BaseClass
{
    protected override DerivedType Create() => new DerivedType();
}

public class DerivedClass : IntermediateClass
{
    public BaseType CreateThroughBase()
    {
        return this.Create();
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
