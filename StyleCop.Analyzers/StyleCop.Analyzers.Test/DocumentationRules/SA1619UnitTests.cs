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
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<Analyzers.DocumentationRules.SA1619GenericTypeParametersMustBeDocumentedPartialClass>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1619GenericTypeParametersMustBeDocumentedPartialClass"/>.
    /// </summary>
    public class SA1619UnitTests
    {
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
        public async Task TestTypesWithoutTypeParametersAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial class Foo { }";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestPartialTypesWithAllDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
/// <typeparam name=""Ta"">Param 1</param>
/// <typeparam name=""Tb"">Param 2</param>
public partial ##";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestPartialTypesWithAllDocumentationAlternativeSyntaxAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
/// <typeparam name=""T&#97;"">Param 1</param>
/// <typeparam name=""T&#x62;"">Param 2</param>
public partial ##";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestPartialTypesWithAllDocumentationWrongOrderAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
/// <typeparam name=""Tb"">Param 2</param>
/// <typeparam name=""Ta"">Param 1</param>
public partial ##";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestPartialTypesWithNoDocumentationAsync(string p)
        {
            var testCode = @"
public partial ##";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestPartialTypesInheritDocAsync(string p)
        {
            var testCode = @"
/// <inheritdoc/>
public partial ##";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = new[]
            {
                Diagnostic().WithLocation(5, 30).WithArguments("Ta"),
                Diagnostic().WithLocation(5, 34).WithArguments("Tb"),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        [WorkItem(2453, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2453")]
        public async Task TestPartialTypesWithMissingButNotRequiredDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public partial ##";

            // This situation is allowed if 'documentExposedElements' and 'documentInterfaces' is false
            string interfaceSettingName = p.StartsWith("interface ") ? "documentInterfaces" : "ignoredProperty";
            var testSettings = $@"
{{
  ""settings"": {{
    ""documentationRules"": {{
      ""documentExposedElements"": false,
      ""{interfaceSettingName}"": false
    }}
  }}
}}
";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), testSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestNonPartialTypesWithMissingDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public ##";

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestPartialTypesWithContentDocumentationAsync(string p)
        {
            var testCode = @"
/// <content>
/// Foo
/// </content>
public partial ##";

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic partial type with included documentation will work.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericPartialTypeWithIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithTypeparamDoc.xml' path='/TestClass/*'/>
public partial class TestClass<T>
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic partial type without a summary tag in the included documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericPartialTypeWithoutSummaryInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithoutSummary.xml' path='/TestClass/*'/>
public partial class TestClass<T>
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic partial type without a typeparam in included documentation will flag.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericPartialTypeWithoutTypeparamInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithoutTypeparamDoc.xml' path='/TestClass/*'/>
public partial class TestClass<T>
{
}
";

            var expected = Diagnostic().WithLocation(3, 32).WithArguments("T");
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2453, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2453")]
        public async Task TestGenericPartialTypeWithoutTypeparamInIncludedButNotRequiredDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithoutTypeparamDoc.xml' path='/TestClass/*'/>
public partial class TestClass<T>
{
}
";

            // The situation is allowed if 'documentExposedElements' false
            var testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentExposedElements"": false
    }
  }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, testSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a generic partial type with &lt;inheritdoc&gt; in included documentation will work.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGenericPartialTypeWithInheritdocInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithIneheritdoc.xml' path='/TestClass/*'/>
public partial class TestClass<T>
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, testSettings: null, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, testSettings: null, expected, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            string contentClassWithTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class</summary>
  <typeparam name=""T"">Param 1</typeparam>
</TestClass>
";
            string contentClassWithoutSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
</TestClass>
";
            string contentClassWithoutTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class</summary>
</TestClass>
";
            string contentClassInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <inheritdoc/>
</TestClass>
";

            var test = new StyleCopDiagnosticVerifier<SA1619GenericTypeParametersMustBeDocumentedPartialClass>.CSharpTest
            {
                TestCode = source,
                Settings = testSettings,
                XmlReferences =
                {
                    { "ClassWithTypeparamDoc.xml", contentClassWithTypeparamDoc },
                    { "ClassWithoutSummary.xml", contentClassWithoutSummary },
                    { "ClassWithoutTypeparamDoc.xml", contentClassWithoutTypeparamDoc },
                    { "ClassWithIneheritdoc.xml", contentClassInheritdoc },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
