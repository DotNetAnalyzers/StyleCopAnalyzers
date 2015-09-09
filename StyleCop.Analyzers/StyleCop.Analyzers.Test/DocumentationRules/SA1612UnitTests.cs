// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1612ElementParameterDocumentationMustMatchElementParameters"/>.
    /// </summary>
    public class SA1612UnitTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> Declarations
        {
            get
            {
                yield return new[] { "    public ClassName Method(string foo, string bar) { return null; }" };
                yield return new[] { "    public delegate ClassName Method(string foo, string bar);" };
                yield return new[] { "    public ClassName this[string foo, string bar] { get { return null; } set { } }" };
            }
        }

        [Fact]
        public async Task TestMemberWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Method() { return null; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithoutParamsAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithValidParamsAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param name=""foo"">Test</param>
    ///<param name=""bar"">Test</param>
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithInvalidParamsAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param name=""boo"">Test</param>
    ///<param name=""far"">Test</param>
$$
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 21).WithArguments("boo"),
                this.CSharpDiagnostic().WithLocation(11, 21).WithArguments("far")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithInvalidParamsThatShouldBeHandledBySA1613Async(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param>Test</param>
    ///<param/>
    ///<param name="""">Test</param>
    ///<param name=""    "">Test</param>
$$
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMembersWithAllDocumentationWrongOrderAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""bar"">Param 2</param>
    /// <param name=""foo"">Param 1</param>
    $$
}";

            var diagnostic = this.CSharpDiagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = new[]
            {
                diagnostic.WithLocation(10, 22).WithArguments("bar", 2),
                diagnostic.WithLocation(11, 22).WithArguments("foo", 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMembersWithTooManyDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""foo"">Param 1</param>
    /// <param name=""bar"">Param 2</param>
    /// <param name=""bar"">Param 3</param>
    $$
}";

            var diagnostic = this.CSharpDiagnostic()
                .WithMessageFormat("The parameter documentation for '{0}' should be at position {1}.");

            var expected = diagnostic.WithLocation(12, 22).WithArguments("bar", 2);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1612ElementParameterDocumentationMustMatchElementParameters();
        }
    }
}
