// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1615ElementReturnValueMustBeDocumented>;

    public partial class SA1615CSharp9UnitTests : SA1615CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3975, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3975")]
        public async Task TestCovariantOverrideMissingReturnsDocumentationAsync()
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
    /// <summary>Creates a base instance.</summary>
    /// <returns>A <see cref=""BaseType""/> instance.</returns>
    public virtual BaseType Create() => new BaseType();
}

public class DerivedClass : BaseClass
{
    /// <summary>Creates a derived instance.</summary>
    public override [|DerivedType|] Create() => new DerivedType();
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3975, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3975")]
        public async Task TestCovariantOverrideInheritsReturnsDocumentationAsync()
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
    /// <summary>Creates a base instance.</summary>
    /// <returns>A <see cref=""BaseType""/> instance.</returns>
    public virtual BaseType Create() => new BaseType();
}

public class DerivedClass : BaseClass
{
    /// <inheritdoc/>
    public override DerivedType Create() => new DerivedType();
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3971, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3971")]
        public async Task TestPartialMethodDeclarationMissingReturnsDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Tests a partial method.
/// </summary>
public partial class TestClass
{
    /// <summary>Declaration.</summary>
    public partial {|#0:int|} TestMethod(int value);

    public partial int TestMethod(int value) => value;
}";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpDiagnosticAsync(testCode, new[] { expected }, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
