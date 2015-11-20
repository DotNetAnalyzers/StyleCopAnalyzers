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
    /// This class contains unit tests for <see cref="SA1611ElementParametersMustBeDocumented"/>.
    /// </summary>
    public class SA1611UnitTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                // These method names are chosen so that the position of the parameters are always the same. This makes testing easier
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string param3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string param3);" };
                yield return new object[] { "System.String this[string param1, string param2, string param3] { get { return param1; } }" };
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string @param3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string @param3);" };
                yield return new object[] { "System.String this[string param1, string param2, string @param3] { get { return param1; } }" };
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string p\\u0061ram3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string p\\u0061ram3);" };
                yield return new object[] { "System.String this[string param1, string param2, string p\\u0061ram3] { get { return param1; } }" };
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationAsync(string p)
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
    /// <param name=""param1"">Param 1</param>
    /// <param name=""param2""></param>
    /// <param name=""param3"">Param 3</param>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationAlternativeSyntaxAsync(string p)
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
    /// <param name=""param1"">Param 1</param>
    /// <param name=""p&#97;ram2""></param>
    /// <param name=""p&#x61;ram3"">Param 3</param>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationWrongOrderAsync(string p)
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
    /// <param name=""param1"">Param 1</param>
    /// <param name=""param3"">Param 3</param>
    /// <param name=""param2""></param>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithNoDocumentationAsync(string p)
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
        [MemberData(nameof(Data))]
        public async Task TestInheritDocAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestMissingParametersAsync(string p)
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
    public ##
}";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(10, 38).WithArguments("param1"),
                this.CSharpDiagnostic().WithLocation(10, 53).WithArguments("param2"),
                this.CSharpDiagnostic().WithLocation(10, 68).WithArguments("param3"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid operator declarations will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyValidOperatorDeclarationsAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// </summary>
public class TestClass
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""value"">The value to use.</param>
    public static TestClass operator +(TestClass value)
    {   
        return value;
    }

    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""value"">The value to use.</param>
    public static explicit operator TestClass(int value)
    {   
        return new TestClass();
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid operator declarations will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyInvalidOperatorDeclarationsAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// </summary>
public class TestClass
{
    /// <summary>
    /// Foo
    /// </summary>
    public static TestClass operator +(TestClass value)
    {   
        return value;
    }

    /// <summary>
    /// Foo
    /// </summary>
    public static explicit operator TestClass(int value)
    {   
        return new TestClass();
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(10, 50).WithArguments("value"),
                this.CSharpDiagnostic().WithLocation(18, 51).WithArguments("value")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1611ElementParametersMustBeDocumented();
        }
    }
}
