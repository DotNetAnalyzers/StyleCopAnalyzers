// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// The class contains unit tests for <see cref="SA1629DocumentationTextMustEndWithAPeriod"/>.
    /// </summary>
    public class SA1629UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// <summary>
public class TestClass
{
    /// <summary>
    /// Gets or sets test property #1
    /// </summary>
    /// <value>
    /// Dummy integer
    /// </value>
    public int TestProperty1 { get; set; }

    /// <summary>Gets or sets test property #2</summary>
    /// <value>Dummy integer</value>
    public int TestProperty2 { get; set; }

    /// <summary>
    /// Test method #1
    /// </summary>
    /// <typeparam name=""T"">
    /// Template type
    /// </typeparam>
    /// <param name=""arg1"">
    /// First argument
    /// </param>
    /// <returns>
    /// Some value
    /// </returns>
    /// <remarks>
    /// Random remark
    /// </remarks>
    /// <example>
    /// Random example
    /// </example>
    /// <exception cref=""System.Exception"">
    /// Exception description
    /// </exception>
    /// <permission cref=""System.Security.PermissionSet"">
    /// Everyone can access this method
    /// </permission>
    public int TestMethod1<T>(T arg1)
    {
        return 0;
    }

    /// <summary>Test method #2</summary>
    /// <typeparam name=""T"">Template type</typeparam>
    /// <param name=""arg1"">First argument</param>
    /// <returns>Some value</returns>
    /// <remarks>Random remark</remarks>
    /// <example>Random example</example>
    /// <exception cref=""System.Exception"">Exception description</exception>
    /// <permission cref=""System.Security.PermissionSet"">Everyone can access this method</permission>
    public int TestMethod2<T>(T arg1)
    {
        return 0;
    }
}
";

            var fixedTestCode = @"
/// <summary>
/// Test class.
/// <summary>
public class TestClass
{
    /// <summary>
    /// Gets or sets test property #1.
    /// </summary>
    /// <value>
    /// Dummy integer.
    /// </value>
    public int TestProperty1 { get; set; }

    /// <summary>Gets or sets test property #2.</summary>
    /// <value>Dummy integer.</value>
    public int TestProperty2 { get; set; }

    /// <summary>
    /// Test method #1.
    /// </summary>
    /// <typeparam name=""T"">
    /// Template type.
    /// </typeparam>
    /// <param name=""arg1"">
    /// First argument.
    /// </param>
    /// <returns>
    /// Some value.
    /// </returns>
    /// <remarks>
    /// Random remark.
    /// </remarks>
    /// <example>
    /// Random example.
    /// </example>
    /// <exception cref=""System.Exception"">
    /// Exception description.
    /// </exception>
    /// <permission cref=""System.Security.PermissionSet"">
    /// Everyone can access this method.
    /// </permission>
    public int TestMethod1<T>(T arg1)
    {
        return 0;
    }

    /// <summary>Test method #2.</summary>
    /// <typeparam name=""T"">Template type.</typeparam>
    /// <param name=""arg1"">First argument.</param>
    /// <returns>Some value.</returns>
    /// <remarks>Random remark.</remarks>
    /// <example>Random example.</example>
    /// <exception cref=""System.Exception"">Exception description.</exception>
    /// <permission cref=""System.Security.PermissionSet"">Everyone can access this method.</permission>
    public int TestMethod2<T>(T arg1)
    {
        return 0;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(3, 15),
                this.CSharpDiagnostic().WithLocation(8, 38),
                this.CSharpDiagnostic().WithLocation(11, 22),
                this.CSharpDiagnostic().WithLocation(15, 47),
                this.CSharpDiagnostic().WithLocation(16, 29),
                this.CSharpDiagnostic().WithLocation(20, 23),
                this.CSharpDiagnostic().WithLocation(23, 22),
                this.CSharpDiagnostic().WithLocation(26, 23),
                this.CSharpDiagnostic().WithLocation(29, 19),
                this.CSharpDiagnostic().WithLocation(32, 22),
                this.CSharpDiagnostic().WithLocation(35, 23),
                this.CSharpDiagnostic().WithLocation(38, 30),
                this.CSharpDiagnostic().WithLocation(41, 40),
                this.CSharpDiagnostic().WithLocation(48, 32),
                this.CSharpDiagnostic().WithLocation(49, 42),
                this.CSharpDiagnostic().WithLocation(50, 42),
                this.CSharpDiagnostic().WithLocation(51, 28),
                this.CSharpDiagnostic().WithLocation(52, 31),
                this.CSharpDiagnostic().WithLocation(53, 32),
                this.CSharpDiagnostic().WithLocation(54, 65),
                this.CSharpDiagnostic().WithLocation(55, 89),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAugmentedInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Test interface.
/// <summary>
public interface ITest
{
    /// <summary>Test method.</summary>
    /// <typeparam name=""T"">Template type.</typeparam>
    /// <param name=""arg1"">First argument.</param>
    /// <returns>Some value.</returns>
    int TestMethod<T>(T arg1);
}

/// <summary>
/// Test class.
/// <summary>
public class TestClass : ITest
{
    /// <inheritdoc/>
    /// <remarks>Random remark</remarks>
    /// <example>Random example</example>
    /// <exception cref=""System.Exception"">Exception description</exception>
    /// <permission cref=""System.Security.PermissionSet"">Everyone can access this method</permission>
    public int TestMethod<T>(T arg1)
    {
        return 0;
    }
}
";

            var fixedTestCode = @"
/// <summary>
/// Test interface.
/// <summary>
public interface ITest
{
    /// <summary>Test method.</summary>
    /// <typeparam name=""T"">Template type.</typeparam>
    /// <param name=""arg1"">First argument.</param>
    /// <returns>Some value.</returns>
    int TestMethod<T>(T arg1);
}

/// <summary>
/// Test class.
/// <summary>
public class TestClass : ITest
{
    /// <inheritdoc/>
    /// <remarks>Random remark.</remarks>
    /// <example>Random example.</example>
    /// <exception cref=""System.Exception"">Exception description.</exception>
    /// <permission cref=""System.Security.PermissionSet"">Everyone can access this method.</permission>
    public int TestMethod<T>(T arg1)
    {
        return 0;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(20, 31),
                this.CSharpDiagnostic().WithLocation(21, 32),
                this.CSharpDiagnostic().WithLocation(22, 65),
                this.CSharpDiagnostic().WithLocation(23, 89),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassInheritDoc.xml' path='/TestClass/*'/>
public class TestClass
{
    /// <include file='PropertyInheritDoc.xml' path='/TestClass/TestProperty/*'/>
    public int TestProperty { get; set; }

    /// <include file='MethodInheritDoc.xml' path='/TestClass/TestMethod/*'/>
    public int TestMethod<T>(T arg1)
    {
        return 0;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(3, 14),
                this.CSharpDiagnostic().WithLocation(6, 16),
                this.CSharpDiagnostic().WithLocation(9, 16),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        [Fact]
        public async Task TestInvalidIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='InvalidClassInheritDoc.xml' path='/TestClass/*'/>
public class TestClass
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentClassInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class</summary>
</TestClass>
";
            resolver.XmlReferences.Add("ClassInheritDoc.xml", contentClassInheritDoc);

            string contentInvalidClassInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class<summary>
</TestClass>
";
            resolver.XmlReferences.Add("InvalidClassInheritDoc.xml", contentInvalidClassInheritDoc);

            string contentPropertyInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestProperty>
    <summary>Gets or sets test property</summary>
    <value>Dummy integer</value>
  </TestProperty>
</TestClass>
";
            resolver.XmlReferences.Add("PropertyInheritDoc.xml", contentPropertyInheritDoc);

            string contentMethodInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <summary>Test method</summary>
    <typeparam name=""T"">Template type</typeparam>
    <param name=""arg1"">First argument</param>
    <returns>Some value</returns>
    <remarks>Random remark</remarks>
    <example>Random example</example>
    <exception cref=""System.Exception"">Exception description</exception>
    <permission cref=""System.Security.PermissionSet"">Everyone can access this method</permission>
  </TestMethod>
</TestClass>
";
            resolver.XmlReferences.Add("MethodInheritDoc.xml", contentMethodInheritDoc);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1629CodeFixProvider();
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1629DocumentationTextMustEndWithAPeriod();
        }
    }
}
