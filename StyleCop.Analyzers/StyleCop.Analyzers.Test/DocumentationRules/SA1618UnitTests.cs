// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1618GenericTypeParametersMustBeDocumented>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1618GenericTypeParametersMustBeDocumented"/>.
    /// </summary>
    public class SA1618UnitTests
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
                yield return new object[] { "class Foo<{|#0:Ta|}, {|#1:Tb|}> { }" };
                yield return new object[] { "struct Foo<{|#0:Ta|}, {|#1:Tb|}> { }" };
                yield return new object[] { "interface Foo<{|#0:Ta|}, {|#1:Tb|}> { }" };
                yield return new object[] { "class Foo<{|#0:Ta|}, {|#1:T\\u0062|}> { }" };
                yield return new object[] { "struct Foo<{|#0:Ta|}, {|#1:T\\u0062|}> { }" };
                yield return new object[] { "interface Foo<{|#0:Ta|}, {|#1:T\\u0062|}> { }" };
                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new object[] { "record Foo<{|#0:Ta|}, {|#1:Tb|}> { }" };
                    yield return new object[] { "record Foo<{|#0:Ta|}, {|#1:T\\u0062|}> { }" };
                }

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new object[] { "record class Foo<{|#0:Ta|}, {|#1:Tb|}> { }" };
                    yield return new object[] { "record struct Foo<{|#0:Ta|}, {|#1:T\\u0062|}> { }" };
                }
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        [MemberData(nameof(Types))]
        public async Task TestTypesInheritDocAsync(string p)
        {
            var testCode = @"
/// <inheritdoc/>
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
            var expected = new[]
            {
                Diagnostic().WithLocation(10, 30).WithArguments("Ta"),
                Diagnostic().WithLocation(10, 34).WithArguments("Tb"),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(0).WithArguments("Ta"),
                Diagnostic().WithLocation(1).WithArguments("Tb"),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(3, 24).WithArguments("T");
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(8, 26).WithArguments("T");
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            string contentClassWithTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class</summary>
  <typeparam name=""T"">Param 1</typeparam>
</TestClass>
";
            string contentMethodWithTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <summary>Test class</summary>
    <typeparam name=""T"">Param 1</typeparam>
  </TestMethod>
</TestClass>
";
            string contentClassWithoutTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class</summary>
</TestClass>
";
            string contentMethodWithoutTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <summary>Test class</summary>
  </TestMethod>
</TestClass>
";
            string contentClassInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <inheritdoc/>
</TestClass>
";
            string contentMethodWithInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
  <inheritdoc/>
  </TestMethod>
</TestClass>
";

            var test = new StyleCopDiagnosticVerifier<SA1618GenericTypeParametersMustBeDocumented>.CSharpTest
            {
                TestCode = source,
                XmlReferences =
                {
                    { "ClassWithTypeparamDoc.xml", contentClassWithTypeparamDoc },
                    { "MethodWithTypeparamDoc.xml", contentMethodWithTypeparamDoc },
                    { "ClassWithoutTypeparamDoc.xml", contentClassWithoutTypeparamDoc },
                    { "MethodWithoutTypeparamDoc.xml", contentMethodWithoutTypeparamDoc },
                    { "ClassWithIneheritdoc.xml", contentClassInheritdoc },
                    { "MethodWithInheritdoc.xml", contentMethodWithInheritdoc },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
