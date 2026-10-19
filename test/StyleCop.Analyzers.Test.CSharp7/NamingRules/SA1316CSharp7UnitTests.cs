// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.NamingRules;
    using StyleCop.Analyzers.Test.Helpers;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Helpers.LanguageVersionTestExtensions;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1316TupleElementNamesShouldUseCorrectCasing,
        StyleCop.Analyzers.NamingRules.SA1316CodeFixProvider>;

    /// <summary>
    /// This class contains the CSharp 7.x unit tests for SA1316.
    /// </summary>
    /// <seealso cref="SA1316TupleElementNamesShouldUseCorrectCasing"/>
    /// <seealso cref="SA1316CodeFixProvider"/>
    public class SA1316CSharp7UnitTests
    {
        private const string DefaultTestSettings = @"
{
  ""settings"": {
  }
}
";

        private const string CamelCaseTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""tupleElementNameCasing"": ""camelCase""
    }
  }
}
";

        private const string PascalCaseTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""tupleElementNameCasing"": ""PascalCase""
    }
  }
}
";

        private const string CamelCaseInferredTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleElementNames"" : true,
      ""tupleElementNameCasing"": ""camelCase""
    }
  }
}
";

        private const string PascalCaseInferredTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleElementNames"" : true,
      ""tupleElementNameCasing"": ""PascalCase""
    }
  }
}
";

        private const string CamelCaseExplicitOnlyTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleElementNames"" : false,
      ""tupleElementNameCasing"": ""camelCase""
    }
  }
}
";

        private const string PascalCaseExplicitOnlyTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleElementNames"" : false,
      ""tupleElementNameCasing"": ""PascalCase""
    }
  }
}
";

        public static IEnumerable<object[]> TypesWithOneLowerCaseTupleElement { get; } = new[]
        {
            new[] { "(int I, string foo)" },
            new[] { "(int I, (bool foo, string S))" },
            new[] { "(int foo, string S)[]" },
            new[] { "List<(string S, bool foo)>" },
        };

        /// <summary>
        /// Validates the properly named tuple element names will not produce diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "ElementName1", "ElementName2")]
        [InlineData(CamelCaseTestSettings, "elementName1", "elementName2")]
        [InlineData(PascalCaseTestSettings, "ElementName1", "ElementName2")]
        public async Task ValidateProperCasedTupleElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2)
        {
            var testCode = $@"
public class TestClass
{{
    public (int {tupleElementName1}, int {tupleElementName2}) TestMethod()
    {{
        return (1, 1);
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that tuple fields with no name will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateNoTupleElementNamesAsync()
        {
            var testCode = @"
public class TestClass
{
    public (int, int) TestMethod()
    {
        return (1, 1);
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the properly named inferred tuple element names will not produce diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "elementName1", "elementName2")]
        [InlineData(CamelCaseTestSettings, "elementName1", "elementName2")]
        [InlineData(PascalCaseTestSettings, "ElementName1", "ElementName2")]
        [InlineData(CamelCaseInferredTestSettings, "elementName1", "elementName2")]
        [InlineData(PascalCaseInferredTestSettings, "ElementName1", "ElementName2")]
        [InlineData(CamelCaseExplicitOnlyTestSettings, "elementName1", "elementName2")]
        [InlineData(PascalCaseExplicitOnlyTestSettings, "ElementName1", "ElementName2")]
        public async Task ValidateProperCasedInferredTupleElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2)
        {
            var testCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var {tupleElementName1} = 1;
        var {tupleElementName2} = ""test"";
        var tuple = ({tupleElementName1}, {tupleElementName2});
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp7_1.OrLaterDefault(), testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the properly explicitly named tuple elements, even when using inferred tuple element names, will not produce diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <param name="tupleInferred1">The name of the first tuple element that would be inferred if not given explicitly.</param>
        /// <param name="tupleInferred2">The name of the second tuple element that would be inferred if not given explicitly.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(CamelCaseInferredTestSettings, "elementName1", "elementName2", "ElementValue1", "ElementValue2")]
        [InlineData(PascalCaseInferredTestSettings, "ElementName1", "ElementName2", "elementValue1", "elementValue2")]
        public async Task ValidateProperCasedExplicitNamesEvenWithInferredTupleElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2, string tupleInferred1, string tupleInferred2)
        {
            var testCode = $@"
public class TestClass
{{
    public void TestMethod1()
    {{
        var {tupleInferred1} = 1;
        var {tupleInferred2} = ""test"";
        var tuple = ({tupleElementName1}: {tupleInferred1}, {tupleElementName2}: {tupleInferred2});
    }}

    public void TestMethod2()
    {{
        var {tupleInferred1} = 1;
        var {tupleElementName2} = ""test"";
        var tuple = ({tupleElementName1}: {tupleInferred1}, {tupleElementName2});
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp7_1.OrLaterDefault(), testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the improperly named tuple element names will produce the expected diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <param name="fixedTupleElementName1">The expected fixed tuple element name for the first field.</param>
        /// <param name="fixedTupleElementName2">The expected fixed tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "elementName1", "elementName2", "ElementName1", "ElementName2")]
        [InlineData(CamelCaseTestSettings, "ElementName1", "ElementName2", "elementName1", "elementName2")]
        [InlineData(PascalCaseTestSettings, "elementName1", "elementName2", "ElementName1", "ElementName2")]
        public async Task ValidateImproperCasedTupleElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2, string fixedTupleElementName1, string fixedTupleElementName2)
        {
            var testCode = $@"
public class TestClass
{{
    public (int [|{tupleElementName1}|], int [|{tupleElementName2}|]) TestMethod1()
    {{
        return (1, 1);
    }}

    public (int /* 1 */ [|{tupleElementName1}|] /* 2 */ , int /* 3 */ [|{tupleElementName2}|] /* 4 */) TestMethod2()
    {{
        return (1, 1);
    }}
}}
";

            var fixedCode = $@"
public class TestClass
{{
    public (int {fixedTupleElementName1}, int {fixedTupleElementName2}) TestMethod1()
    {{
        return (1, 1);
    }}

    public (int /* 1 */ {fixedTupleElementName1} /* 2 */ , int /* 3 */ {fixedTupleElementName2} /* 4 */) TestMethod2()
    {{
        return (1, 1);
    }}
}}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // diagnostics are specified inline
            };

            await VerifyCSharpFixAsync(testCode, settings, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        ///  Verifies that improperly named inferred tuple element names are ignored when the 'includeInferredTupleElementNames' option is not set to true.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "ElementName1", "ElementName2")]
        [InlineData(CamelCaseTestSettings, "ElementName1", "ElementName2")]
        [InlineData(PascalCaseTestSettings, "elementName1", "elementName2")]
        [InlineData(CamelCaseExplicitOnlyTestSettings, "ElementName1", "ElementName2")]
        [InlineData(PascalCaseExplicitOnlyTestSettings, "elementName1", "elementName2")]
        public async Task ValidateImproperCasedInferredTupleElementNamesAreIgnoredAsync(string settings, string tupleElementName1, string tupleElementName2)
        {
            var testCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var {tupleElementName1} = 1;
        var {tupleElementName2} = ""test"";
        var tuple = ({tupleElementName1}, {tupleElementName2});
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp7_1.OrLaterDefault(), testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the improperly named inferred tuple element names will produce the expected diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(CamelCaseInferredTestSettings, "ElementName1", "ElementName2")]
        [InlineData(PascalCaseInferredTestSettings, "elementName1", "elementName2")]
        public async Task ValidateImproperCasedImplicitTupleElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2)
        {
            //// TODO: C# 7.1
            var testCode = $@"
public class TestClass
{{
    public void TestMethod1()
    {{
        var {tupleElementName1} = 1;
        var {tupleElementName2} = ""test"";
        var tuple = ([|{tupleElementName1}|], [|{tupleElementName2}|]);
    }}

    public void TestMethod2()
    {{
        var {tupleElementName1} = 1;
        var {tupleElementName2} = ""test"";
        var tuple = (/* 1 */ [|{tupleElementName1}|] /* 2 */, /* 3 */ [|{tupleElementName2}|] /* 4 */);
    }}
}}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // diagnostics are specified inline
            };

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp7_1.OrLaterDefault(), testCode, settings, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that improperly cased explicit element names in tuple expressions produce diagnostics, regardless
        /// of the 'includeInferredTupleElementNames' option.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The improper tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The improper tuple element name for the second field.</param>
        /// <param name="fixedTupleElementName1">The expected fixed tuple element name for the first field.</param>
        /// <param name="fixedTupleElementName2">The expected fixed tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "elementName1", "elementName2", "ElementName1", "ElementName2")]
        [InlineData(CamelCaseTestSettings, "ElementName1", "ElementName2", "elementName1", "elementName2")]
        [InlineData(PascalCaseTestSettings, "elementName1", "elementName2", "ElementName1", "ElementName2")]
        [InlineData(CamelCaseExplicitOnlyTestSettings, "ElementName1", "ElementName2", "elementName1", "elementName2")]
        [InlineData(PascalCaseExplicitOnlyTestSettings, "elementName1", "elementName2", "ElementName1", "ElementName2")]
        [InlineData(CamelCaseInferredTestSettings, "ElementName1", "ElementName2", "elementName1", "elementName2")]
        [InlineData(PascalCaseInferredTestSettings, "elementName1", "elementName2", "ElementName1", "ElementName2")]
        [WorkItem(3947, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3947")]
        public async Task ValidateImproperCasedExplicitTupleExpressionElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2, string fixedTupleElementName1, string fixedTupleElementName2)
        {
            var testCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var tuple1 = ([|{tupleElementName1}|]: 1, [|{tupleElementName2}|]: ""test"");
        var tuple2 = (/* 1 */ [|{tupleElementName1}|] /* 2 */: 1, {fixedTupleElementName2}: 2);
        var tuple3 = (_: 1, {fixedTupleElementName1}: 2);
    }}
}}
";

            var fixedCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var tuple1 = ({fixedTupleElementName1}: 1, {fixedTupleElementName2}: ""test"");
        var tuple2 = (/* 1 */ {fixedTupleElementName1} /* 2 */: 1, {fixedTupleElementName2}: 2);
        var tuple3 = (_: 1, {fixedTupleElementName1}: 2);
    }}
}}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // diagnostics are specified inline
            };

            await VerifyCSharpFixAsync(testCode, settings, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3031, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3031")]
        public async Task TestTupleDesconstructionCamelCaseAsync()
        {
            var testCode = @"
public class TypeName
{
    public void MethodName((string name, string value) obj)
    {
        (string name, string value) = obj;
    }
}
";

            await VerifyCSharpDiagnosticAsync(languageVersion: null, testCode, CamelCaseTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3031, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3031")]
        public async Task TestTupleDesconstructionPascalCaseAsync()
        {
            var testCode = @"
public class TypeName
{
    public void MethodName((string Name, string Value) obj)
    {
        (string name, string value) = obj;
    }
}
";

            await VerifyCSharpDiagnosticAsync(languageVersion: null, testCode, PascalCaseTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3031, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3031")]
        public async Task TestTupleDesconstructionPascalCaseListAsync()
        {
            var testCode = @"
using System.Collections.Generic;
public class TypeName
{
    public void MethodName(List<(string Name, string Value)> list)
    {
        foreach ((string name, string value) in list)
        {
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(languageVersion: null, testCode, PascalCaseTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3139, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3139")]
        public async Task TestTupleDesconstructionDiscardAsync()
        {
            var testCode = @"
public class TypeName
{
    public void MethodName((string Name, string Value) obj)
    {
        (string name, _) = obj;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInOverriddenMethodsParameterTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public class BaseType
{{
    public virtual void TestMethod({type.Replace("foo", "[|foo|]")} p)
    {{
    }}
}}

public class TestType : BaseType
{{
    public override void TestMethod({type} p)
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInExplicitlyImplementedMethodsParameterTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public interface TestInterface
{{
    void TestMethod({type.Replace("foo", "[|foo|]")} p);
}}

public class TestType : TestInterface
{{
    void TestInterface.TestMethod({type} p)
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInInterfaceImplementedMethodsParameterTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public interface TestInterface
{{
    void TestMethod({type.Replace("foo", "[|foo|]")} p);
}}

public class TestType : TestInterface
{{
    public void TestMethod({type} p)
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInOverriddenMethodsReturnTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public class BaseType
{{
    public virtual {type.Replace("foo", "[|foo|]")} TestMethod()
    {{
        throw new System.Exception();
    }}
}}

public class TestType : BaseType
{{
    public override {type} TestMethod()
    {{
        throw new System.Exception();
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInExplicitlyImplementedMethodsReturnTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public interface TestInterface
{{
    {type.Replace("foo", "[|foo|]")} TestMethod();
}}

public class TestType : TestInterface
{{
    {type} TestInterface.TestMethod()
    {{
        throw new System.Exception();
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInInterfaceImplementedMethodsReturnTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public interface TestInterface
{{
    {type.Replace("foo", "[|foo|]")} TestMethod();
}}

public class TestType : TestInterface
{{
    public {type} TestMethod()
    {{
        throw new System.Exception();
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInOverriddenPropertysTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public class BaseType
{{
    public virtual {type.Replace("foo", "[|foo|]")} TestProperty {{ get; set; }}
}}

public class TestType : BaseType
{{
    public override {type} TestProperty {{ get; set; }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInExplicitlyImplementedPropertysTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public interface TestInterface
{{
    {type.Replace("foo", "[|foo|]")} TestProperty {{ get; set; }}
}}

public class TestType : TestInterface
{{
    {type} TestInterface.TestProperty {{ get; set; }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInInterfaceImplementedPropertysTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public interface TestInterface
{{
    {type.Replace("foo", "[|foo|]")} TestProperty {{ get; set; }}
}}

public class TestType : TestInterface
{{
    public {type} TestProperty {{ get; set; }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInOverriddenEventsTypeAsync(string type)
        {
            var testCode = $@"
using System;
using System.Collections.Generic;

public class BaseType
{{
    public virtual event Action<{type.Replace("foo", "[|foo|]")}> TestEvent {{ add {{}} remove {{}} }}
}}

public class TestType : BaseType
{{
    public override event Action<{type}> TestEvent {{ add {{}} remove {{}} }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInExplicitlyImplementedEventsTypeAsync(string type)
        {
            var testCode = $@"
using System;
using System.Collections.Generic;

public interface TestInterface
{{
    event Action<{type.Replace("foo", "[|foo|]")}> TestEvent;
}}

public class TestType : TestInterface
{{
    event Action<{type}> TestInterface.TestEvent {{ add {{}} remove {{}} }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInInterfaceImplementedEventsTypeAsync(string type)
        {
            var testCode = $@"
using System;
using System.Collections.Generic;

public interface TestInterface
{{
    event Action<{type.Replace("foo", "[|foo|]")}> TestEvent;
}}

public class TestType : TestInterface
{{
    public event Action<{type}> TestEvent {{ add {{}} remove {{}} }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInOverriddenIndexersReturnTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public abstract class BaseType
{{
    public abstract {type.Replace("foo", "[|foo|]")} this[int i] {{ get; }}
}}

public class TestType : BaseType
{{
    public override {type} this[int i] => throw new System.Exception();
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInExplicitlyImplementedIndexersReturnTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public interface TestInterface
{{
    {type.Replace("foo", "[|foo|]")} this[int i] {{ get; }}
}}

public class TestType : TestInterface
{{
    {type} TestInterface.this[int i] => throw new System.Exception();
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TypesWithOneLowerCaseTupleElement))]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperTupleElementNameInInterfaceImplementedIndexersReturnTypeAsync(string type)
        {
            var testCode = $@"
using System.Collections.Generic;

public interface TestInterface
{{
    {type.Replace("foo", "[|foo|]")} this[int i] {{ get; }}
}}

public class TestType : TestInterface
{{
    public {type} this[int i] => throw new System.Exception();
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3781, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3781")]
        public async Task TestImproperVariableTupleElementNameInsideOverriddenMethodAsync()
        {
            var testCode = @"
using System.Collections.Generic;

public abstract class BaseType
{
    public virtual void TestMethod()
    {
    }
}

public class TestType : BaseType
{
    public override void TestMethod()
    {
        (int I, bool [|b|]) x;
    }
}
";

            var fixedCode = @"
using System.Collections.Generic;

public abstract class BaseType
{
    public virtual void TestMethod()
    {
    }
}

public class TestType : BaseType
{
    public override void TestMethod()
    {
        (int I, bool B) x;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that correcting a tuple element name also updates references to that element.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementNameFixUpdatesReferencesAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int /* keep */ [|nameA|], int Other) value)
    {
        return value.nameA + value.Other;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int /* keep */ NameA, int Other) value)
    {
        return value.NameA + value.Other;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an inferred anonymous-type property keeps its original name when the projected tuple element is renamed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenamePreservesInferredAnonymousPropertyNameAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int [|nameA|], int Other) value)
    {
        var projected = new { value.nameA };
        return projected.nameA;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int NameA, int Other) value)
    {
        var projected = new { nameA = value.NameA };
        return projected.nameA;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that preserving an inferred anonymous-type name does not collide with an explicit property.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenameDoesNotDuplicateAnonymousPropertyNameAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int [|nameA|], int Other) value)
    {
        var projected = new { value.nameA, NameA = 0 };
        return projected.nameA + projected.NameA;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int NameA, int Other) value)
    {
        var projected = new { nameA = value.NameA, NameA = 0 };
        return projected.nameA + projected.NameA;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an explicit anonymous-type property name is left unchanged while its tuple element reference is updated.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenameKeepsExplicitAnonymousPropertyNameAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int [|nameA|], int Other) value)
    {
        var projected = new { nameA = value.nameA };
        return projected.nameA;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int NameA, int Other) value)
    {
        var projected = new { nameA = value.NameA };
        return projected.nameA;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an escaped keyword used as an inferred anonymous property name stays escaped.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenamePreservesEscapedKeywordAnonymousPropertyNameAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int [|@class|], int Other) value)
    {
        var projected = new { value.@class };
        return projected.@class;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int Class, int Other) value)
    {
        var projected = new { @class = value.Class };
        return projected.@class;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a tuple element nested inside an anonymous projection does not become the property name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenameDoesNotNameAnonymousPropertyAfterNestedAccessAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int [|nameA|], int Other) value)
    {
        var projected = new { value.nameA.ToString().Length };
        return projected.Length;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int NameA, int Other) value)
    {
        var projected = new { value.NameA.ToString().Length };
        return projected.Length;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an inferred tuple element name is kept when the projected element is renamed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenamePreservesInferredTupleNameAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int [|nameA|], int Other) value)
    {
        var projected = (false, value.nameA);
        return projected.nameA;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int NameA, int Other) value)
    {
        var projected = (false, NameA: value.NameA);
        return projected.NameA;
    }
}
";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_1.OrLaterDefault(), testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a projection of an inferred tuple element is renamed along with its consumers.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenameUpdatesChainedInferredTupleNamesAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int [|nameA|], int Other) value)
    {
        var first = (false, value.nameA);
        var second = ("""", first.nameA, 0L);
        return second.nameA;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int NameA, int Other) value)
    {
        var first = (false, NameA: value.NameA);
        var second = ("""", NameA: first.NameA, 0L);
        return second.NameA;
    }
}
";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_1.OrLaterDefault(), testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the fix is not offered when the corrected inferred name would select another tuple element.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenameIsNotOfferedWhenInferredNameWouldCollideAsync()
        {
            const string source = @"
public class TestClass
{
    public int TestMethod((int nameA, int Other) value)
    {
        var projected = (false, value.nameA, NameA: """");
        return projected.nameA;
    }
}
";

            var workspace = await GenericAnalyzerTest.CreateWorkspaceAsync().ConfigureAwait(false);
            var projectId = ProjectId.CreateNewId();
            var references = await GenericAnalyzerTest.ReferenceAssemblies.ResolveAsync(LanguageNames.CSharp, CancellationToken.None).ConfigureAwait(false);
            var solution = workspace.CurrentSolution
                .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
                .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .WithProjectParseOptions(projectId, new CSharpParseOptions(LanguageVersion.CSharp7_1))
                .AddMetadataReferences(projectId, references);
            var documentId = DocumentId.CreateNewId(projectId);
            solution = solution.AddDocument(documentId, "Test.cs", SourceText.From(source));
            var document = solution.GetDocument(documentId);
            var compilation = await document.Project.GetCompilationAsync(CancellationToken.None).ConfigureAwait(false);
            var analyzer = new SA1316TupleElementNamesShouldUseCorrectCasing();
            var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync(CancellationToken.None).ConfigureAwait(false);
            var diagnostic = Assert.Single(diagnostics);

            var actions = new List<CodeAction>();
            var context = new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None);
            await new SA1316CodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);
            Assert.Empty(actions);
        }

        /// <summary>
        /// Verifies that the fix is not offered when duplicate inferred names were suppressed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenameIsNotOfferedWhenDuplicateInferredNamesAreSuppressedAsync()
        {
            const string source = @"
public class TestClass
{
    public int TestMethod((int nameA, int Other) value)
    {
        var projected = (value.nameA, value.nameA);
        return projected.Item1 + projected.Item2;
    }
}
";

            var workspace = await GenericAnalyzerTest.CreateWorkspaceAsync().ConfigureAwait(false);
            var projectId = ProjectId.CreateNewId();
            var references = await GenericAnalyzerTest.ReferenceAssemblies.ResolveAsync(LanguageNames.CSharp, CancellationToken.None).ConfigureAwait(false);
            var solution = workspace.CurrentSolution
                .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
                .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default))
                .WithProjectParseOptions(projectId, new CSharpParseOptions(LanguageVersion.CSharp7_1))
                .AddMetadataReferences(projectId, references);
            var documentId = DocumentId.CreateNewId(projectId);
            solution = solution.AddDocument(documentId, "Test.cs", SourceText.From(source));
            var document = solution.GetDocument(documentId);
            var compilation = await document.Project.GetCompilationAsync(CancellationToken.None).ConfigureAwait(false);
            var analyzer = new SA1316TupleElementNamesShouldUseCorrectCasing();
            var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync(CancellationToken.None).ConfigureAwait(false);
            var diagnostic = Assert.Single(diagnostics);

            var actions = new List<CodeAction>();
            var context = new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None);
            await new SA1316CodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);
            Assert.Empty(actions);
        }

        /// <summary>
        /// Verifies that an explicit tuple element name is left unchanged while its reference is updated.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenameKeepsExplicitTupleNameAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int [|nameA|], int Other) value)
    {
        var projected = (Kept: value.nameA, false);
        return projected.Kept;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int NameA, int Other) value)
    {
        var projected = (Kept: value.NameA, false);
        return projected.Kept;
    }
}
";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_1.OrLaterDefault(), testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a nested tuple-element access does not become the inferred element name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleElementRenameDoesNotNameTupleElementAfterNestedAccessAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestMethod((int [|nameA|], int Other) value)
    {
        var projected = (value.nameA.ToString().Length, false);
        return projected.Item1;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int TestMethod((int NameA, int Other) value)
    {
        var projected = (value.NameA.ToString().Length, false);
        return projected.Item1;
    }
}
";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_1.OrLaterDefault(), testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
