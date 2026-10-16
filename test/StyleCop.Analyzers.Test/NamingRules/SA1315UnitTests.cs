// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules;

    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1315ParametersShouldMatchInheritedNames,
        StyleCop.Analyzers.NamingRules.SA1315CodeFixProvider>;

    [UseCulture("en-US")]
    public class SA1315UnitTests
    {
        [Fact]
        [WorkItem(1949, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1949")]
        public async Task TestOverrideParameterNameMismatchAsync()
        {
            var testCode = @"public class BaseType
{
    public virtual void Method(int inheritedName)
    {
    }
}

public class DerivedType : BaseType
{
    public override void Method(int {|#0:actualName|})
    {
    }
}";

            var fixedCode = @"public class BaseType
{
    public virtual void Method(int inheritedName)
    {
    }
}

public class DerivedType : BaseType
{
    public override void Method(int inheritedName)
    {
    }
}";

            await VerifyCSharpFixAsync(
                testCode,
                Diagnostic().WithArguments("actualName").WithLocation(0),
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1949, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1949")]
        public async Task TestImplicitInterfaceImplementationParameterNameMismatchAsync()
        {
            var testCode = @"public interface IType
{
    void Method(int inheritedName);
}

public class TypeName : IType
{
    public void Method(int {|#0:actualName|})
    {
    }
}";

            var fixedCode = @"public interface IType
{
    void Method(int inheritedName);
}

public class TypeName : IType
{
    public void Method(int inheritedName)
    {
    }
}";

            await VerifyCSharpFixAsync(
                testCode,
                Diagnostic().WithArguments("actualName").WithLocation(0),
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1949, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1949")]
        public async Task TestExplicitInterfaceImplementationParameterNameMismatchAsync()
        {
            var testCode = @"public interface IType
{
    void Method(int inheritedName);
}

public class TypeName : IType
{
    void IType.Method(int {|#0:actualName|})
    {
    }
}";

            var fixedCode = @"public interface IType
{
    void Method(int inheritedName);
}

public class TypeName : IType
{
    void IType.Method(int inheritedName)
    {
    }
}";

            await VerifyCSharpFixAsync(
                testCode,
                Diagnostic().WithArguments("actualName").WithLocation(0),
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoDiagnosticForMatchingOrNonInheritedParametersAsync()
        {
            var testCode = @"public interface IType
{
    void InterfaceMethod(int inheritedName);
}

public class BaseType
{
    public virtual void VirtualMethod(int inheritedName)
    {
    }

    public virtual void HiddenMethod(int inheritedName)
    {
    }
}

public class TypeName : BaseType, IType
{
    public override void VirtualMethod(int inheritedName)
    {
    }

    public void InterfaceMethod(int inheritedName)
    {
    }

    public void NewMethod(int unrelatedName)
    {
    }

    public new void HiddenMethod(int unrelatedName)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1949, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1949")]
        public async Task TestOverrideUsesBaseMethodFromIntermediateTypeAsync()
        {
            var testCode = @"public class BaseType
{
    public virtual void Method(int inheritedName)
    {
    }
}

public class IntermediateType : BaseType
{
}

public class DerivedType : IntermediateType
{
    public override void Method(int {|#0:actualName|})
    {
    }
}";

            var fixedCode = @"public class BaseType
{
    public virtual void Method(int inheritedName)
    {
    }
}

public class IntermediateType : BaseType
{
}

public class DerivedType : IntermediateType
{
    public override void Method(int inheritedName)
    {
    }
}";

            await VerifyCSharpFixAsync(
                testCode,
                Diagnostic().WithArguments("actualName").WithLocation(0),
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoDiagnosticWhenNameMatchesAnImplementedInterfaceAsync()
        {
            var testCode = @"public interface IFirst
{
    void Method(int firstName);
}

public interface ISecond
{
    void Method(int secondName);
}

public interface IDerived : IFirst
{
}

public class TypeName : IDerived, ISecond
{
    public void Method(int firstName)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1949, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1949")]
        public async Task TestOverridePrefersBaseMethodNameOverInterfaceNameAsync()
        {
            var testCode = @"public interface IType
{
    void Method(int interfaceName);
}

public abstract class BaseType
{
    public abstract void Method(int baseName);
}

public class DerivedType : BaseType, IType
{
    public override void Method(int {|#0:actualName|})
    {
    }
}";

            var fixedCode = @"public interface IType
{
    void Method(int interfaceName);
}

public abstract class BaseType
{
    public abstract void Method(int baseName);
}

public class DerivedType : BaseType, IType
{
    public override void Method(int baseName)
    {
    }
}";

            await VerifyCSharpFixAsync(
                testCode,
                Diagnostic().WithArguments("actualName").WithLocation(0),
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }
    }
