// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1316TupleFieldNamesMustUseCorrectCasing,
        StyleCop.Analyzers.NamingRules.SA1316CodeFixProvider>;

    /// <summary>
    /// This class contains the CSharp 7.x unit tests for SA1316.
    /// </summary>
    /// <seealso cref="SA1316TupleFieldNamesMustUseCorrectCasing"/>
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
      ""tupleFieldNameCasing"": ""camelCase""
    }
  }
}
";

        private const string PascalCaseTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""tupleFieldNameCasing"": ""pascalCase""
    }
  }
}
";

        private const string CamelCaseInferredTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleFieldNames"" : true,
      ""tupleFieldNameCasing"": ""camelCase""
    }
  }
}
";

        private const string PascalCaseInferredTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleFieldNames"" : true,
      ""tupleFieldNameCasing"": ""pascalCase""
    }
  }
}
";

        private const string CamelCaseExplicitOnlyTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleFieldNames"" : false,
      ""tupleFieldNameCasing"": ""camelCase""
    }
  }
}
";

        private const string PascalCaseExplicitOnlyTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleFieldNames"" : false,
      ""tupleFieldNameCasing"": ""pascalCase""
    }
  }
}
";

        /// <summary>
        /// Validates the properly named tuple field names will not produce diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleFieldName1">The expected tuple field name for the first field.</param>
        /// <param name="tupleFieldName2">The expected tuple field name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "fieldName1", "fieldName2")]
        [InlineData(CamelCaseTestSettings, "fieldName1", "fieldName2")]
        [InlineData(PascalCaseTestSettings, "FieldName1", "FieldName2")]
        public async Task ValidateProperCasedTupleFieldNamesAsync(string settings, string tupleFieldName1, string tupleFieldName2)
        {
            var testCode = $@"
public class TestClass
{{
    public (int {tupleFieldName1}, int {tupleFieldName2}) TestMethod()
    {{
        return (1, 1);
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that tuple fields with no name will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateNoTupleFieldNamesAsync()
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

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7, testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the properly named inferred tuple field names will not produce diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleFieldName1">The expected tuple field name for the first field.</param>
        /// <param name="tupleFieldName2">The expected tuple field name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "fieldName1", "fieldName2")]
        [InlineData(CamelCaseTestSettings, "fieldName1", "fieldName2")]
        [InlineData(PascalCaseTestSettings, "FieldName1", "FieldName2")]
        [InlineData(CamelCaseInferredTestSettings, "fieldName1", "fieldName2")]
        [InlineData(PascalCaseInferredTestSettings, "FieldName1", "FieldName2")]
        [InlineData(CamelCaseExplicitOnlyTestSettings, "fieldName1", "fieldName2")]
        [InlineData(PascalCaseExplicitOnlyTestSettings, "FieldName1", "FieldName2")]
        public async Task ValidateProperCasedInferredTupleFieldNamesAsync(string settings, string tupleFieldName1, string tupleFieldName2)
        {
            var testCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var {tupleFieldName1} = 1;
        var {tupleFieldName2} = ""test"";
        var tuple = ({tupleFieldName1}, {tupleFieldName2});
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7_1, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the improperly named tuple field names will produce the expected diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleFieldName1">The expected tuple field name for the first field.</param>
        /// <param name="tupleFieldName2">The expected tuple field name for the second field.</param>
        /// <param name="fixedTupleFieldName1">The expected fixed tuple field name for the first field.</param>
        /// <param name="fixedTupleFieldName2">The expected fixed tuple field name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "FieldName1", "FieldName2", "fieldName1", "fieldName2")]
        [InlineData(CamelCaseTestSettings, "FieldName1", "FieldName2", "fieldName1", "fieldName2")]
        [InlineData(PascalCaseTestSettings, "fieldName1", "fieldName2", "FieldName1", "FieldName2")]
        public async Task ValidateImproperCasedTupleFieldNamesAsync(string settings, string tupleFieldName1, string tupleFieldName2, string fixedTupleFieldName1, string fixedTupleFieldName2)
        {
            var testCode = $@"
public class TestClass
{{
    public (int [|{tupleFieldName1}|], int [|{tupleFieldName2}|]) TestMethod1()
    {{
        return (1, 1);
    }}

    public (int /* 1 */ [|{tupleFieldName1}|] /* 2 */ , int /* 3 */ [|{tupleFieldName2}|] /* 4 */) TestMethod2()
    {{
        return (1, 1);
    }}
}}
";

            var fixedCode = $@"
public class TestClass
{{
    public (int {fixedTupleFieldName1}, int {fixedTupleFieldName2}) TestMethod1()
    {{
        return (1, 1);
    }}

    public (int /* 1 */ {fixedTupleFieldName1} /* 2 */ , int /* 3 */ {fixedTupleFieldName2} /* 4 */) TestMethod2()
    {{
        return (1, 1);
    }}
}}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // diagnostics are specified inline
            };

            await VerifyCSharpFixAsync(LanguageVersionEx.CSharp7, testCode, settings, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        ///  Verifies that improperly named inferred tuple field names are ignored when the 'includeInferredTupleFieldNames' option is not set to true.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleFieldName1">The expected tuple field name for the first field.</param>
        /// <param name="tupleFieldName2">The expected tuple field name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "FieldName1", "FieldName2")]
        [InlineData(CamelCaseTestSettings, "FieldName1", "FieldName2")]
        [InlineData(PascalCaseTestSettings, "fieldName1", "fieldName2")]
        [InlineData(CamelCaseExplicitOnlyTestSettings, "FieldName1", "FieldName2")]
        [InlineData(PascalCaseExplicitOnlyTestSettings, "fieldName1", "fieldName2")]
        public async Task ValidateImproperCasedInferredTupleFieldNamesAreIgnoredAsync(string settings, string tupleFieldName1, string tupleFieldName2)
        {
            var testCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var {tupleFieldName1} = 1;
        var {tupleFieldName2} = ""test"";
        var tuple = ({tupleFieldName1}, {tupleFieldName2});
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7_1, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the improperly named inferred tuple field names will produce the expected diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleFieldName1">The expected tuple field name for the first field.</param>
        /// <param name="tupleFieldName2">The expected tuple field name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(CamelCaseInferredTestSettings, "FieldName1", "FieldName2")]
        [InlineData(PascalCaseInferredTestSettings, "fieldName1", "fieldName2")]
        public async Task ValidateImproperCasedImplicitTupleFieldNamesAsync(string settings, string tupleFieldName1, string tupleFieldName2)
        {
            //// TODO: C# 7.1
            var testCode = $@"
public class TestClass
{{
    public void TestMethod1()
    {{
        var {tupleFieldName1} = 1;
        var {tupleFieldName2} = ""test"";
        var tuple = ([|{tupleFieldName1}|], [|{tupleFieldName2}|]);
    }}

    public void TestMethod2()
    {{
        var {tupleFieldName1} = 1;
        var {tupleFieldName2} = ""test"";
        var tuple = (/* 1 */ [|{tupleFieldName1}|] /* 2 */, /* 3 */ [|{tupleFieldName2}|] /* 4 */);
    }}
}}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // diagnostics are specified inline
            };

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7_1, testCode, settings, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
