// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.GenericTypeParameterDocumentationAnalyzer>;

    /// <summary>
    /// This class contains unit tests for the SA1620 diagnostic.
    /// </summary>
    public class SA1620UnitTests
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypesWithoutTypeParametersAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class Foo { }";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var diagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620WrongOrderDescriptor);

            var expected = new[]
            {
                diagnostic.WithLocation(10, 26).WithArguments("Tb", 2),
                diagnostic.WithLocation(11, 26).WithArguments("Ta", 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
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

            var diagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620WrongOrderDescriptor);

            var expected = new[]
            {
                diagnostic.WithLocation(5, 22).WithArguments("Tb", 2),
                diagnostic.WithLocation(6, 22).WithArguments("Ta", 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithNoDocumentationAsync(string p)
        {
            var testCode = @"
public ##";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMembersExcludeAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <exclude/>
    public ##
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesInheritDocAsync(string p)
        {
            var testCode = @"
/// <inheritdoc/>
public ##";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesExcludeAsync(string p)
        {
            var testCode = @"
/// <exclude/>
public ##";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMembersWithInvalidDocumentationAsync(string p)
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
    /// <typeparam name=""Tc"">Param 3</param>
    public ##
}";

            var diagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620MissingTypeParameterDescriptor);
            var expected = diagnostic.WithLocation(12, 26).WithArguments("Tc");

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithInvalidDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
/// <typeparam name=""Ta"">Param 1</param>
/// <typeparam name=""Tb"">Param 2</param>
/// <typeparam name=""Tc"">Param 3</param>
public ##";

            var diagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620MissingTypeParameterDescriptor);
            var expected = diagnostic.WithLocation(7, 22).WithArguments("Tc");

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
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
    /// <typeparam name=""Ta"">Param 1</param>
    /// <typeparam name=""Tb"">Param 2</param>
    /// <typeparam name=""Tb"">Param 3</param>
    public ##
}";

            var diagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620WrongOrderDescriptor);
            var expected = diagnostic.WithLocation(12, 26).WithArguments("Tb", 2);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypesWithTooManyDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
/// <typeparam name=""Ta"">Param 1</param>
/// <typeparam name=""Tb"">Param 2</param>
/// <typeparam name=""Tb"">Param 3</param>
public ##";

            var diagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620WrongOrderDescriptor);
            var expected = diagnostic.WithLocation(7, 22).WithArguments("Tb", 2);

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type with wrongly ordered typeparam documentation in the included documentation will produce the expected diagnostics.
        /// </summary>
        /// <param name="p">The type declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypeWithWronglyOrderedTypeParamInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <include file='TypeWithWronglyOrderedTypeparamsDoc.xml' path='/Foo/*'/>
public ##
";
            var diagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620WrongOrderDescriptor);
            DiagnosticResult[] expected =
            {
                diagnostic.WithLocation(2, 5).WithArguments("Tb", 2),
                diagnostic.WithLocation(2, 5).WithArguments("Ta", 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type with a missing typeparam in the included documentation will produce the expected diagnostics.
        /// </summary>
        /// <param name="p">The type declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestTypeWithMissingTypeParamInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <include file='TypeWithMissingTypeparamDoc.xml' path='/Foo/*'/>
public ##
";

            var wrongOrderDiagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620WrongOrderDescriptor);
            var missingDiagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620MissingTypeParameterDescriptor);

            DiagnosticResult[] expected =
            {
                wrongOrderDiagnostic.WithLocation(2, 5).WithArguments("Tb", 2),
                missingDiagnostic.WithLocation(2, 5).WithArguments("Tc"),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with wrongly ordered typeparam documentation in the included documentation will produce the expected diagnostics.
        /// </summary>
        /// <param name="p">The method declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMethodWithWronglyOrderedTypeParamInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='MethodWithWronglyOrderedTypeparamsDoc.xml' path='/TestClass/Foo/*'/>
    public ##
}
";
            var diagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620WrongOrderDescriptor);
            DiagnosticResult[] expected =
            {
                diagnostic.WithLocation(5, 9).WithArguments("Tb", 2),
                diagnostic.WithLocation(5, 9).WithArguments("Ta", 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with a missing typeparam in the included documentation will produce the expected diagnostics.
        /// </summary>
        /// <param name="p">The method declaration text that will be used.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Members))]
        public async Task TestMethodWithMissingTypeParamInIncludedDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='MethodWithMissingTypeparamDoc.xml' path='/TestClass/Foo/*'/>
    public ##
}
";

            var wrongOrderDiagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620WrongOrderDescriptor);
            var missingDiagnostic = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1620MissingTypeParameterDescriptor);

            DiagnosticResult[] expected =
            {
                wrongOrderDiagnostic.WithLocation(5, 9).WithArguments("Tb", 2),
                missingDiagnostic.WithLocation(5, 9).WithArguments("Tc"),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = CreateTest(expected);
            test.TestCode = source;

            return test.RunAsync(cancellationToken);
        }

        private static StyleCopDiagnosticVerifier<GenericTypeParameterDocumentationAnalyzer>.CSharpTest CreateTest(DiagnosticResult[] expected)
        {
            string contentTypeWithTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <summary>Test class</summary>
  <typeparam name=""Ta"">Param 1</typeparam>
  <typeparam name=""Tb"">Param 2</typeparam>
</Foo>
";
            string contentTypeWithoutTypeparamsDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <summary>Test class</summary>
</Foo>
";
            string contentTypeInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <inheritdoc/>
</Foo>
";
            string contentTypeWithWronglyOrderedTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <summary>Test class</summary>
  <typeparam name=""Tb"">Param 2</typeparam>
  <typeparam name=""Ta"">Param 1</typeparam>
</Foo>
";
            string contentTypeWithMissingTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Foo>
  <summary>Test class</summary>
  <typeparam name=""Tb"">Param 2</typeparam>
  <typeparam name=""Tc"">Param 3</typeparam>
</Foo>
";
            string contentMethodWithTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <summary>Test class</summary>
    <typeparam name=""Ta"">Param 1</typeparam>
    <typeparam name=""Tb"">Param 2</typeparam>
  </Foo>
</TestClass>
";
            string contentMethodWithoutTypeparamsDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <summary>Test class</summary>
  </Foo>
</TestClass>
";
            string contentMethodInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <inheritdoc/>
  </Foo>
</TestClass>
";
            string contentMethodWithWronglyOrderedTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <summary>Test class</summary>
    <typeparam name=""Tb"">Param 2</typeparam>
    <typeparam name=""Ta"">Param 1</typeparam>
  </Foo>
</TestClass>
";
            string contentMethodWithMissingTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Foo>
    <summary>Test class</summary>
    <typeparam name=""Tb"">Param 2</typeparam>
    <typeparam name=""Tc"">Param 3</typeparam>
  </Foo>
</TestClass>
";

            var test = new StyleCopDiagnosticVerifier<GenericTypeParameterDocumentationAnalyzer>.CSharpTest
            {
                XmlReferences =
                {
                    { "TypeWithTypeparamsDoc.xml", contentTypeWithTypeparamDoc },
                    { "TypeWithoutTypeparamsDoc.xml", contentTypeWithoutTypeparamsDoc },
                    { "TypeWithInheritdoc.xml", contentTypeInheritdoc },
                    { "TypeWithWronglyOrderedTypeparamsDoc.xml", contentTypeWithWronglyOrderedTypeparamDoc },
                    { "TypeWithMissingTypeparamDoc.xml", contentTypeWithMissingTypeparamDoc },
                    { "MethodWithTypeparamsDoc.xml", contentMethodWithTypeparamDoc },
                    { "MethodWithoutTypeparamsDoc.xml", contentMethodWithoutTypeparamsDoc },
                    { "MethodWithInheritdoc.xml", contentMethodInheritdoc },
                    { "MethodWithWronglyOrderedTypeparamsDoc.xml", contentMethodWithWronglyOrderedTypeparamDoc },
                    { "MethodWithMissingTypeparamDoc.xml", contentMethodWithMissingTypeparamDoc },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test;
        }
    }
}
