// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for the SA1621 diagnostic.
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
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(10, 8),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(11, 8),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(12, 25),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(13, 25),
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
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(5, 4),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(6, 4),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(7, 21),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(8, 21),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type with included documentation will produce no diagnostics.
        /// </summary>
        /// <param name="p">The type declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypeWithIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <include file='TypeWithTypeparamsDoc.xml' path='/Foo/*'/>
public ##
";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type without typeparam documentation in the included documentation will produce no diagnostics.
        /// </summary>
        /// <param name="p">The type declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypeWithoutTypeparamInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <include file='TypeWithoutTypeparamsDoc.xml' path='/Foo/*'/>
public ##
";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type with inheritdoc in the included documentation will produce no diagnostics.
        /// </summary>
        /// <param name="p">The type declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypeWithInheritDocInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <include file='TypeWithInheritdoc.xml' path='/Foo/*'/>
public ##
";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type with invalid typeparam documentation in the included documentation will produce the expected diagnostics.
        /// </summary>
        /// <param name="p">The type declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypeWithInvalidTypeParamInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <include file='TypeWithInvalidTypeparamsDoc.xml' path='/Foo/*'/>
public ##
";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(2, 5),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(2, 5),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(2, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with included documentation will produce no diagnostics.
        /// </summary>
        /// <param name="p">The method declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMethodWithIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='MethodWithTypeparamsDoc.xml' path='/TestClass/Foo/*'/>
    public ##
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method without typeparam documentation in the included documentation will produce no diagnostics.
        /// </summary>
        /// <param name="p">The method declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMethodWithoutTypeparamInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='MethodWithoutTypeparamsDoc.xml' path='/TestClass/Foo/*'/>
    public ##
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with inheritdoc in the included documentation will produce no diagnostics.
        /// </summary>
        /// <param name="p">The method declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMethodWithInheritDocInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='MethodWithoutTypeparamsDoc.xml' path='/TestClass/Foo/*'/>
    public ##
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with invalid typeparam documentation in the included documentation will produce the expected diagnostics.
        /// </summary>
        /// <param name="p">The method declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMethodWithInvalidTypeParamInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='MethodWithInvalidTypeparamsDoc.xml' path='/TestClass/Foo/*'/>
    public ##
}
";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(5, 9),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(5, 9),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(5, 9),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentTypeWithTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <summary>Test class</summary>
  <typeparam name=""Ta"">Param 1</typeparam>
  <typeparam name=""Tb"">Param 2</typeparam>
</Foo>
";
            resolver.XmlReferences.Add("TypeWithTypeparamsDoc.xml", contentTypeWithTypeparamDoc);

            string contentTypeWithoutTypeparamsDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <summary>Test class</summary>
</Foo>
";
            resolver.XmlReferences.Add("TypeWithoutTypeparamsDoc.xml", contentTypeWithoutTypeparamsDoc);

            string contentTypeInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <inheritdoc/>
</Foo>
";
            resolver.XmlReferences.Add("TypeWithInheritdoc.xml", contentTypeInheritdoc);

            string contentTypeWithInvalidTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <summary>Test class</summary>
  <typeparam>Param 1</typeparam>
  <typeparam name="""">Param 2</typeparam>
  <typeparam name=""  "">Param 3</typeparam>
</Foo>
";
            resolver.XmlReferences.Add("TypeWithInvalidTypeparamsDoc.xml", contentTypeWithInvalidTypeparamDoc);

            string contentMethodWithTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <summary>Test class</summary>
    <typeparam name=""Ta"">Param 1</typeparam>
    <typeparam name=""Tb"">Param 2</typeparam>
  </Foo>
</TestClass>
";
            resolver.XmlReferences.Add("MethodWithTypeparamsDoc.xml", contentMethodWithTypeparamDoc);

            string contentMethodWithoutTypeparamsDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <summary>Test class</summary>
  </Foo>
</TestClass>
";
            resolver.XmlReferences.Add("MethodWithoutTypeparamsDoc.xml", contentMethodWithoutTypeparamsDoc);

            string contentMethodInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <inheritdoc/>
  </Foo>
</TestClass>
";
            resolver.XmlReferences.Add("MethodWithInheritdoc.xml", contentMethodInheritdoc);

            string contentMethodWithInvalidTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <summary>Test class</summary>
    <typeparam>Param 1</typeparam>
    <typeparam name="""">Param 2</typeparam>
    <typeparam name=""  "">Param 3</typeparam>
  </Foo>
</TestClass>
";
            resolver.XmlReferences.Add("MethodWithInvalidTypeparamsDoc.xml", contentMethodWithInvalidTypeparamDoc);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            yield return GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor.Id;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new GenericTypeParameterDocumentationAnalyzer();
        }
    }
}
