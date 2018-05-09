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
    /// This class contains unit tests for SA1622.
    /// </summary>
    public class SA1622UnitTests : DiagnosticVerifier
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
        public async Task TestMemberWithEmptyParamsAsync(string declaration)
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
    ///<typeparam name=""Ta""></typeparam>
    ///<typeparam name=""Tb"">   

    ///</typeparam>
$$
}";

            var expected = new[]
            {
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(10, 8),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(11, 8),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithEmptyParamsAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
///<typeparam name=""Ta""></typeparam>
///<typeparam name=""Tb"">   

///</typeparam>
public $$";

            var expected = new[]
            {
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(5, 4),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(6, 4),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMemberWithEmptyParams2Async(string declaration)
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
    ///<typeparam name=""Ta""/>
    ///<typeparam name=""Tb"">
    ///<para>
    ///     
    ///</para>
    ///</typeparam>
$$
}";

            var expected = new[]
            {
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(10, 8),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(11, 8),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithEmptyParams2Async(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
///<typeparam name=""Ta""/>
///<typeparam name=""Tb"">
///<para>
///     
///</para>
public $$";

            var expected = new[]
            {
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(5, 4),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(6, 4),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a class with valid typeparameter tags in included documentation will not produce diagnostics.
        /// </summary>
        /// <param name="typeText">The type specific test text.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithValidTypeparameterInIncludedDocumentationAsync(string typeText)
        {
            var testCode = @"
/// <include file='TypeWithTypeparamsDoc.xml' path='/Foo/*'/>
public ##
";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", typeText), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a class with empty typeparameter tags in included documentation will produce diagnostics.
        /// </summary>
        /// <param name="typeText">The type specific test text.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithEmptyTypeparameterInIncludedDocumentationAsync(string typeText)
        {
            var testCode = @"
/// <include file='TypeWithEmptyTypeparamsDoc.xml' path='/Foo/*'/>
public ##
";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(2, 5),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(2, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", typeText), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with valid typeparameter tags in the included documentation will produce no diagnostics.
        /// </summary>
        /// <param name="memberText">The member declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMemberWithValidTypeparameterInIncludedDocumentationAsync(string memberText)
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='MethodWithTypeparamsDoc.xml' path='/TestClass/Foo/*'/>
    public ##
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", memberText), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with empty typeparameter tags in the included documentation will produce no diagnostics.
        /// </summary>
        /// <param name="memberText">The member declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMemberWithEmptyTypeparameterInIncludedDocumentationAsync(string memberText)
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='MethodWithEmptyTypeparamsDoc.xml' path='/TestClass/Foo/*'/>
    public ##
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(5, 9),
                this.CSharpDiagnostic(GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(5, 9),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", memberText), expected, CancellationToken.None).ConfigureAwait(false);
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

            string contentTypeWithEmptyTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <summary>Test class</summary>
  <typeparam name=""Ta""></typeparam>
  <typeparam name=""Tb""/>
</Foo>
";
            resolver.XmlReferences.Add("TypeWithEmptyTypeparamsDoc.xml", contentTypeWithEmptyTypeparamDoc);

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

            string contentMethodWithEmptyTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <summary>Test class</summary>
    <typeparam name=""Ta""/>
    <typeparam name=""Tb""></typeparam>
  </Foo>
</TestClass>
";
            resolver.XmlReferences.Add("MethodWithEmptyTypeparamsDoc.xml", contentMethodWithEmptyTypeparamDoc);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new GenericTypeParameterDocumentationAnalyzer();
        }
    }
}
