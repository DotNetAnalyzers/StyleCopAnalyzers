// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented,
        StyleCop.Analyzers.DocumentationRules.SA1600CodeFixProvider>;

    public partial class SA1600CSharp15UnitTests : SA1600CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that an undocumented union declaration is reported once, and that a documented union declaration is
        /// not reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionDeclarationAsync()
        {
            var testCode = @"
public union {|#0:Pet|}(int, string);

/// <summary>
/// A documented union.
/// </summary>
public union DocumentedPet(int, string);

/// <summary>
/// A documented class.
/// </summary>
public class Outer
{
    public union {|#1:NestedPet|}(int, string);

    private union PrivateNestedPet(int, string);
}
";

            await new CSharpTest()
            {
                TestCode = testCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) },
                DisabledDiagnostics = { "CS1591" },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a partial union declaration is not reported, since partial elements are reported by SA1601
        /// instead.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestPartialUnionDeclarationAsync()
        {
            var testCode = @"
public partial union Pet(int, string);

public partial union Pet
{
}
";

            await new CSharpTest()
            {
                TestCode = testCode,
                DisabledDiagnostics = { "CS1591" },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that each undocumented member of a union is reported exactly once. The analyzer driver in Roslyn
        /// 5.9 ran syntax node actions on union members three times (dotnet/roslyn#84570), which reported each of these
        /// diagnostics three times.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionMembersReportedOnceAsync()
        {
            var testCode = @"
/// <summary>
/// A documented union.
/// </summary>
public union Pet(int, string)
{
    public const int {|#0:Legs|} = 4;

    public int {|#1:Count|} => Legs;

    public void {|#2:Feed|}()
    {
    }

    public class {|#3:Toy|}
    {
        public void {|#4:Squeak|}()
        {
        }
    }
}
";

            await new CSharpTest()
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0),
                    Diagnostic().WithLocation(1),
                    Diagnostic().WithLocation(2),
                    Diagnostic().WithLocation(3),
                    Diagnostic().WithLocation(4),
                },
                DisabledDiagnostics = { "CS1591" },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticResult[] GetExpectedResultTestRegressionMethodGlobalNamespace(string code)
        {
            if (code == "public void {|#0:TestMember|}() { }")
            {
                return base.GetExpectedResultTestRegressionMethodGlobalNamespace(code);
            }

            // The C# 15 compiler reports CS9348 instead of CS0116 for non-method members in the compilation unit
            return new[]
            {
                DiagnosticResult.CompilerError("CS9348").WithMessage("A compilation unit cannot directly contain members such as fields, methods or properties").WithLocation(0),
                Diagnostic().WithLocation(0),
            };
        }
    }
}
