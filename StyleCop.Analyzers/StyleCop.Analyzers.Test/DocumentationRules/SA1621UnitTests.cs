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
    /// This class contains unit tests for <see cref="SA1621GenericTypeParameterDocumentationMustDeclareParameterName"/>.
    /// </summary>
    public class SA1621UnitTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> Members
        {
            get
            {
                // These method names are chosen so that the position of the parameters are always the same. This makes testing easier
                yield return new object[] { "void          Foo<Ta, Tb>() { }" };
                yield return new object[] { "delegate void Foo<Ta, Tb>();" };
                yield return new object[] { "void          Foo<Ta, T\\u0062>() { }" };
                yield return new object[] { "delegate void Foo<Ta, T\\u0062>();" };
            }
        }

        public static IEnumerable<object[]> Types
        {
            get
            {
                yield return new object[] { "class     Foo<Ta, Tb> { }" };
                yield return new object[] { "struct    Foo<Ta, Tb> { }" };
                yield return new object[] { "interface Foo<Ta, Tb> { }" };
                yield return new object[] { "class     Foo<Ta, T\\u0062> { }" };
                yield return new object[] { "struct    Foo<Ta, T\\u0062> { }" };
                yield return new object[] { "interface Foo<Ta, T\\u0062> { }" };
            }
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMembersWithNoDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithNoDocumentationAsync(string p)
        {
            var testCode = @"
public ##";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypesWithoutTypeParametersAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class Foo { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMembersWithAllDocumentationAsync(string p)
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
    /// <typeparam name=""Ta"">Param 1</param>
    /// <typeparam name=""Tb"">Param 2</param>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithAllDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
/// <typeparam name=""Ta"">Param 1</param>
/// <typeparam name=""Tb"">Param 2</param>
public ##";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMembersWithInvalidParamsAsync(string declaration)
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
    ///<typeparam>Test</typeparam>
    ///<typeparam/>
    ///<typeparam name="""">Test</typeparam>
    ///<typeparam name=""    "">Test</typeparam>
$$
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 8),
                this.CSharpDiagnostic().WithLocation(11, 8),
                this.CSharpDiagnostic().WithLocation(12, 19),
                this.CSharpDiagnostic().WithLocation(13, 19)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithInvalidParamsAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
///<typeparam>Test</typeparam>
///<typeparam/>
///<typeparam name="""">Test</typeparam>
///<typeparam name=""    "">Test</typeparam>
public $$";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(5, 4),
                this.CSharpDiagnostic().WithLocation(6, 4),
                this.CSharpDiagnostic().WithLocation(7, 15),
                this.CSharpDiagnostic().WithLocation(8, 15)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1621GenericTypeParameterDocumentationMustDeclareParameterName();
        }
    }
}
