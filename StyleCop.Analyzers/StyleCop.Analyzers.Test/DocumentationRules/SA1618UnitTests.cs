// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1618GenericTypeParametersMustBeDocumented"/>.
    /// </summary>
    public class SA1618UnitTests : DiagnosticVerifier
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

        [Fact]
        public async Task TestMembersWithoutTypeParametersAsync()
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
    public void Foo() { }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        public async Task TestMembersWithAllDocumentationAlternativeSyntaxAsync(string p)
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
    /// <typeparam name=""T&#97;"">Param 1</param>
    /// <typeparam name=""T&#x62;"">Param 2</param>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithAllDocumentationAlternativeSyntaxAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
/// <typeparam name=""T&#97;"">Param 1</param>
/// <typeparam name=""T&#x62;"">Param 2</param>
public ##";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
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
    /// <typeparam name=""Tb"">Param 2</param>
    /// <typeparam name=""Ta"">Param 1</param>
    public ##
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithAllDocumentationWrongOrderAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
/// <typeparam name=""Tb"">Param 2</param>
/// <typeparam name=""Ta"">Param 1</param>
public ##";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMembersInheritDocAsync(string p)
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
        [MemberData(nameof(Types))]
        public async Task TestTypesInheritDocAsync(string p)
        {
            var testCode = @"
/// <inheritdoc/>
public ##";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMembersWithMissingDocumentationAsync(string p)
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
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 30).WithArguments("Ta"),
                this.CSharpDiagnostic().WithLocation(10, 34).WithArguments("Tb"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithMissingDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public ##";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(5, 22).WithArguments("Ta"),
                this.CSharpDiagnostic().WithLocation(5, 26).WithArguments("Tb"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestPartialTypesWithMissingDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial ##";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic type with included documentation will work.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericTypeWithIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithTypeparamDoc.xml' path='/TestClass/*'/>
public class TestClass<T>
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic method with included documentation will work.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericMethodWithIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// </summary>
public class TestClass
{
  /// <include file='MethodWithTypeparamDoc.xml' path='/TestClass/TestMethod/*'/>
  public void TestMethod<T>(T param1) { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic type without a typeparam in included documentation will flag.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericTypeWithoutTypeparamInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithoutTypeparamDoc.xml' path='/TestClass/*'/>
public class TestClass<T>
{
}
";

            var expected = this.CSharpDiagnostic().WithLocation(3, 24).WithArguments("T");
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic method without a typeparam included documentation will flag.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericMethodWithoutTypeparamInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// </summary>
public class TestClass
{
  /// <include file='MethodWithoutTypeparamDoc.xml' path='/TestClass/TestMethod/*'/>
  public void TestMethod<T>(T param1) { }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(8, 26).WithArguments("T");
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic type with &lt;inheritdoc&gt; in included documentation will work.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericTypeWithInheritdocInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithIneheritdoc.xml' path='/TestClass/*'/>
public class TestClass<T>
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic method with &lt;inheritdoc&gt; in included documentation will work.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericMethodWithInheritdocInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// </summary>
public class TestClass
{
  /// <include file='MethodWithInheritdoc.xml' path='/TestClass/TestMethod/*'/>
  public void TestMethod<T>(T param1) { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2446, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2446")]
        public async Task TestPrivateMethodMissingGenericParametersAsync()
        {
            var testCode = @"
internal class ClassName
{
    ///
    private void Test1<T>(int arg) { }

    /**
     *
     */
    private void Test2<T>(int arg) { }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentClassWithTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class</summary>
  <typeparam name=""T"">Param 1</typeparam>
</TestClass>
";
            resolver.XmlReferences.Add("ClassWithTypeparamDoc.xml", contentClassWithTypeparamDoc);

            string contentMethodWithTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <summary>Test class</summary>
    <typeparam name=""T"">Param 1</typeparam>
  </TestMethod>
</TestClass>
";
            resolver.XmlReferences.Add("MethodWithTypeparamDoc.xml", contentMethodWithTypeparamDoc);

            string contentClassWithoutTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class</summary>
</TestClass>
";
            resolver.XmlReferences.Add("ClassWithoutTypeparamDoc.xml", contentClassWithoutTypeparamDoc);

            string contentMethodWithoutTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <summary>Test class</summary>
  </TestMethod>
</TestClass>
";
            resolver.XmlReferences.Add("MethodWithoutTypeparamDoc.xml", contentMethodWithoutTypeparamDoc);

            string contentClassInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <inheritdoc/>
</TestClass>
";
            resolver.XmlReferences.Add("ClassWithIneheritdoc.xml", contentClassInheritdoc);

            string contentMethodWithInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
  <inheritdoc/>
  </TestMethod>
</TestClass>
";
            resolver.XmlReferences.Add("MethodWithInheritdoc.xml", contentMethodWithInheritdoc);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1618GenericTypeParametersMustBeDocumented();
        }
    }
}
