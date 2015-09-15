// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1648InheritDocMustBeUsedWithInheritingClass"/> analyzer.
    /// </summary>
    public class SA1648UnitTests : DiagnosticVerifier
    {
        [Fact]
        public async Task TestClassOverridesClassAsync()
        {
            var testCode = @"class Base { }
/// <inheritdoc/>
class Test : Base { }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassImplementsInterfaceAsync()
        {
            var testCode = @"interface IBase { }
/// <inheritdoc/>
class Test : IBase { }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassOverridesClassAndImplementsInterfaceAsync()
        {
            var testCode = @"class Base { }
interface IBase { }
/// <inheritdoc/>
class Test : Base, IBase { }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceImplementsInterfaceAsync()
        {
            var testCode = @"interface IBase { }
/// <inheritdoc/>
interface ITest : IBase { }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("interface Test { }")]
        [InlineData("class Test { }")]
        [InlineData("struct Test { }")]
        [InlineData("enum Test { }")]
        [InlineData("delegate void Test ();")]
        public async Task TestTypeWithEmptyBaseListAsync(string declaration)
        {
            var testCode = @"/// <inheritdoc/>
";

            var expected = this.CSharpDiagnostic().WithLocation(1, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode + declaration, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("Test() { }")]
        [InlineData("void Foo() { }")]
        [InlineData("string foo;")]
        [InlineData("string Foo { get; set; }")]
        [InlineData("string this [string f] { get { return f; } }")]
        [InlineData("event System.Action foo;")]
        [InlineData("event System.Action Foo { add { } remove { } }")]
        public async Task TestMemberThatShouldNotHaveInheritDocAsync(string declaration)
        {
            var testCode = @"class Test
{{
    /// <inheritdoc/>
    {0}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(3, 9);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public override void Foo() { }")]
        [InlineData("public override string Bar { get; set; }")]
        [InlineData("public override string this [string f] { get { return f; } }")]
        [InlineData("public override event System.Action EventName1 { add { } remove { } }")]
        [InlineData("public override event System.Action EventName2 { add { } remove { } }")]
        public async Task TestMemberThatCanHaveInheritDocOverrideAsync(string declaration)
        {
            var testCode = @"class TestBase
{{
    public virtual void Foo() {{ }}
    public virtual string Bar {{ get; set; }}
    public virtual string this [string f] {{ get {{ return f; }} }}
    public virtual event System.Action EventName1;
    public virtual event System.Action EventName2 {{ add {{ }} remove {{ }} }}
}}
class Test : TestBase
{{
    /// <inheritdoc/>
    {0}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMemberThatCanHaveInheritDocExplicitImplementAsync(string type)
        {
            var testCode = @"interface ITest
{{
    void Foo();
    string Bar {{ get; set; }}
    string this [string f] {{ get; }}
    event System.Action EventName;
}}
{0} Test : ITest
{{
    /// <inheritdoc/>
    void ITest.Foo() {{ }}
    /// <inheritdoc/>
    string ITest.Bar {{ get; set; }}
    /// <inheritdoc/>
    string ITest.this [string f] {{ get {{ return f; }} }}
    /// <inheritdoc/>
    event System.Action ITest.EventName {{ add {{ }} remove {{ }} }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, type), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMemberThatCanHaveInheritDocImplicitImplementAsync(string type)
        {
            var testCode = @"interface ITest
{{
    void Foo();
    string Bar {{ get; set; }}
    string this [string f] {{ get; }}
    event System.Action EventName1;
    event System.Action EventName2;
}}
{0} Test : ITest
{{
    /// <inheritdoc/>
    public void Foo() {{ }}
    /// <inheritdoc/>
    public string Bar {{ get; set; }}
    /// <inheritdoc/>
    public string this [string f] {{ get {{ return f; }} }}
    /// <inheritdoc/>
    public event System.Action EventName1;
    /// <inheritdoc/>
    public event System.Action EventName2 {{ add {{ }} remove {{ }} }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, type), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1648InheritDocMustBeUsedWithInheritingClass();
        }
    }
}
