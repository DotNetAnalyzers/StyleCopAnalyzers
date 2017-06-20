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
    /// This class contains unit tests for <see cref="SA1619GenericTypeParametersMustBeDocumentedPartialClass"/>.
    /// </summary>
    public class SA1619UnitTests : DiagnosticVerifier
    {
        private string currentTestSettings;

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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestPartialTypesWithNoDocumentationAsync(string p)
        {
            var testCode = @"
public partial ##";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public async Task TestPartialTypesInheritDocAsync(string p)
        {
            var testCode = @"
/// <inheritdoc/>
public partial ##";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(5, 30).WithArguments("Ta"),
                this.CSharpDiagnostic().WithLocation(5, 34).WithArguments("Tb"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
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
            this.currentTestSettings = $@"
{{
  ""settings"": {{
    ""documentationRules"": {{
      ""documentExposedElements"": false,
      ""{interfaceSettingName}"": false
    }}
  }}
}}
";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(3, 32).WithArguments("T");
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            this.currentTestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentExposedElements"": false
    }
  }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            string contentClassWithoutSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
</TestClass>
";
            resolver.XmlReferences.Add("ClassWithoutSummary.xml", contentClassWithoutSummary);

            string contentClassWithoutTypeparamDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class</summary>
</TestClass>
";
            resolver.XmlReferences.Add("ClassWithoutTypeparamDoc.xml", contentClassWithoutTypeparamDoc);

            string contentClassInheritdoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <inheritdoc/>
</TestClass>
";
            resolver.XmlReferences.Add("ClassWithIneheritdoc.xml", contentClassInheritdoc);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override string GetSettings()
        {
            return this.currentTestSettings ?? base.GetSettings();
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1619GenericTypeParametersMustBeDocumentedPartialClass();
        }
    }
}
