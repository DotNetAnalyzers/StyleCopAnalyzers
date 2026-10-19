// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules;

    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
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
    public virtual void Method(int firstName, int secondName)
    {
    }
}

public class DerivedType : BaseType
{
    public override void Method(int {|#0:firstActualName|}, int {|#1:secondActualName|})
    {
    }
}";

            var fixedCode = @"public class BaseType
{
    public virtual void Method(int firstName, int secondName)
    {
    }
}

public class DerivedType : BaseType
{
    public override void Method(int firstName, int secondName)
    {
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("firstActualName", "firstName").WithMessage("Parameter 'firstActualName' should match an inherited parameter name: firstName").WithLocation(0),
                Diagnostic().WithArguments("secondActualName", "secondName").WithLocation(1),
            };

            await VerifyCSharpFixAsync(
                testCode,
                expected,
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
                Diagnostic().WithArguments("actualName", "inheritedName").WithLocation(0),
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
                Diagnostic().WithArguments("actualName", "inheritedName").WithLocation(0),
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that by-reference parameters and their uses can be renamed to the inherited name.
        /// </summary>
        /// <param name="modifier">The parameter modifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("out")]
        [InlineData("ref")]
        public async Task TestByReferenceOverrideParameterNameMismatchAsync(string modifier)
        {
            var testCode = $$"""
                public abstract class BaseType
                {
                    public abstract void Method({{modifier}} int inheritedName);
                }

                public class DerivedType : BaseType
                {
                    public override void Method({{modifier}} int {|#0:actualName|})
                    {
                        // Preserve this assignment and its comment.
                        actualName = 1;
                    }
                }
                """;

            var fixedCode = $$"""
                public abstract class BaseType
                {
                    public abstract void Method({{modifier}} int inheritedName);
                }

                public class DerivedType : BaseType
                {
                    public override void Method({{modifier}} int inheritedName)
                    {
                        // Preserve this assignment and its comment.
                        inheritedName = 1;
                    }
                }
                """;

            await VerifyCSharpFixAsync(
                testCode,
                Diagnostic().WithArguments("actualName", "inheritedName").WithLocation(0),
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that interface implementations offer a rename for by-reference parameters.
        /// </summary>
        /// <param name="modifier">The parameter modifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("out")]
        [InlineData("ref")]
        public async Task TestByReferenceInterfaceParameterNameMismatchAsync(string modifier)
        {
            var testCode = $$"""
                public interface IType
                {
                    void Method({{modifier}} int inheritedName);
                }

                public class TypeName : IType
                {
                    public void Method({{modifier}} int {|#0:actualName|})
                    {
                        actualName = 1;
                    }
                }
                """;

            var fixedCode = $$"""
                public interface IType
                {
                    void Method({{modifier}} int inheritedName);
                }

                public class TypeName : IType
                {
                    public void Method({{modifier}} int inheritedName)
                    {
                        inheritedName = 1;
                    }
                }
                """;

            await VerifyCSharpFixAsync(
                testCode,
                Diagnostic().WithArguments("actualName", "inheritedName").WithLocation(0),
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a local name conflict prevents the code fix, including for out parameters.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task TestNoCodeFixWhenInheritedNameConflictsWithLocalAsync()
        {
            var testCode = @"public interface IType
{
    void Method(out int inheritedName);
}

public class TypeName : IType
{
    public void Method(out int {|#0:actualName|})
    {
        int inheritedName = 1;
        actualName = inheritedName;
    }
}";

            await VerifyCSharpFixAsync(
                testCode,
                Diagnostic().WithArguments("actualName", "inheritedName").WithLocation(0),
                testCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the message lists all distinct inherited names and each rename is available.
        /// </summary>
        /// <param name="codeActionIndex">The index of the rename action to apply.</param>
        /// <param name="newName">The inherited name selected by the action.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Theory]
        [InlineData(0, "firstName")]
        [InlineData(1, "secondName")]
        public async Task TestMultipleInheritedNamesAsync(int codeActionIndex, string newName)
        {
            var testCode = @"public interface IFirst
{
    void Method(int firstName);
}

public interface ISecond
{
    void Method(int secondName);
}

public interface IThird
{
    void Method(int firstName);
}

public class TypeName : IFirst, ISecond, IThird
{
    public void Method(int {|#0:actualName|})
    {
    }
}";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = testCode.Replace("{|#0:actualName|}", newName),
                CodeActionIndex = codeActionIndex,
            };
            test.ExpectedDiagnostics.Add(Diagnostic().WithArguments("actualName", "firstName, secondName").WithMessage("Parameter 'actualName' should match an inherited parameter name: firstName, secondName").WithLocation(0));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithArguments("actualName", "inheritedName").WithLocation(0),
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
                Diagnostic().WithArguments("actualName", "baseName").WithLocation(0),
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMatchingInheritedInterfaceNameIsAllowedBySA1313Async()
        {
            var test = new CombinedCSharpTest
            {
                TestCode = @"public interface IBase
{
    void Method(int Value);
}

public interface IDerived : IBase
{
}

public interface IOther
{
    void Method(int OtherValue);
}

public class TypeName : IOther, IDerived
{
    public void Method(int Value)
    {
    }
}",
            };

            test.ExpectedDiagnostics.AddRange(new[]
            {
                new DiagnosticResult("SA1313", DiagnosticSeverity.Warning)
                    .WithMessage("Parameter 'Value' should begin with lower-case letter")
                    .WithSpan(3, 21, 3, 26),
                new DiagnosticResult("SA1313", DiagnosticSeverity.Warning)
                    .WithMessage("Parameter 'OtherValue' should begin with lower-case letter")
                    .WithSpan(12, 21, 12, 31),
            });

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        internal class CombinedCSharpTest : StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
            StyleCop.Analyzers.NamingRules.SA1315ParametersShouldMatchInheritedNames,
            StyleCop.Analyzers.NamingRules.SA1315CodeFixProvider>.CSharpTest
        {
            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
            {
                return new DiagnosticAnalyzer[]
                {
                    new StyleCop.Analyzers.NamingRules.SA1315ParametersShouldMatchInheritedNames(),
                    new StyleCop.Analyzers.NamingRules.SA1313ParameterNamesMustBeginWithLowerCaseLetter(),
                };
            }
        }
    }
