﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
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

        [Theory(DisplayName = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1948")]
        [InlineData("interface Test { }")]
        [InlineData("class Test { }")]
        [InlineData("struct Test { }")]
        [InlineData("enum Test { }")]
        [InlineData("delegate void Test ();")]
        public async Task TestTypeWithEmptyBaseListAndCrefAttributeAsync(string declaration)
        {
            var testCode = @"/// <inheritdoc cref=""object""/>
";

            await this.VerifyCSharpDiagnosticAsync(testCode + declaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("Test() { }")]
        [InlineData("void Foo() { }")]
        [InlineData("string foo;")]
        [InlineData("string Foo { get; set; }")]
        [InlineData("string this [string f] { get { return f; } }")]
        [InlineData("event System.Action foo;")]
        [InlineData("event System.Action Foo { add { } remove { } }")]
        [InlineData("~Test() { }")]
        [InlineData("public static Test operator +(Test value) { return value; }")]
        [InlineData("public static explicit operator Test(int value) { return new Test(); }")]
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

        [Theory(DisplayName = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1948")]
        [InlineData("Test() { }")]
        [InlineData("void Foo() { }")]
        [InlineData("string foo;")]
        [InlineData("string Foo { get; set; }")]
        [InlineData("string this [string f] { get { return f; } }")]
        [InlineData("event System.Action foo;")]
        [InlineData("event System.Action Foo { add { } remove { } }")]
        [InlineData("~Test() { }")]
        [InlineData("public static Test operator +(Test value) { return value; }")]
        [InlineData("public static explicit operator Test(int value) { return new Test(); }")]
        public async Task TestMemberThatShouldNotHaveInheritDocButHasCrefAttributeAsync(string declaration)
        {
            var testCode = @"class Test
{{
    /// <inheritdoc cref=""object""></inheritdoc>
    {0}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

        /// <summary>
        /// Verifies that a class that includes the inheritdoc will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCorrectClassInheritDocAsync()
        {
            var testCode = @"
/// <summary>Base class</summary>
public class BaseClass { }

/// <include file='ClassInheritDoc.xml' path='/TestClass/*'/>
public class TestClass : BaseClass
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a class that includes an invalid inheritdoc will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIncorrectClassInheritDocAsync()
        {
            var testCode = @"
/// <include file='ClassInheritDoc.xml' path='/TestClass/*'/>
public class TestClass
{
}
";

            var expected = this.CSharpDiagnostic().WithLocation(2, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method that includes the inheritdoc will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCorrectMethodInheritDocAsync()
        {
            var testCode = @"
/// <summary>Base class</summary>
public interface ITest 
{
  /// <summary>My test method,</summary>
  void TestMethod();
}

/// <summary>Test class</summary>
public class TestClass : ITest
{
  /// <include file='MethodInheritDoc.xml' path='/TestClass/TestMethod/*'/>
  public void TestMethod()
  {
  }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method that includes an invalid inheritdoc will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIncorrectMethodInheritDocAsync()
        {
            var testCode = @"
/// <summary>Base class</summary>
public interface ITest 
{
}

/// <summary>Test class</summary>
public class TestClass : ITest
{
  /// <include file='MethodInheritDoc.xml' path='/TestClass/TestMethod/*'/>
  public void TestMethod() { }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(10, 7);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentClassInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <inheritdoc/>
</TestClass>
";
            resolver.XmlReferences.Add("ClassInheritDoc.xml", contentClassInheritDoc);

            string contentMethodInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <inheritdoc/>
  </TestMethod>
</TestClass>
";
            resolver.XmlReferences.Add("MethodInheritDoc.xml", contentMethodInheritDoc);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1648InheritDocMustBeUsedWithInheritingClass();
        }
    }
}
