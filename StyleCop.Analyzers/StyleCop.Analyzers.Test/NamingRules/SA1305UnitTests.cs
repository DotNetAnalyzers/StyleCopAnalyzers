// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1305UnitTests : DiagnosticVerifier
    {
        private const string SettingsFileName = "stylecop.json";
        private const string DefaultTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowCommonHungarianPrefixes"": true,
      ""allowedHungarianPrefixes"": [ ]
    }
  }
}
";

        private string customTestSettings;

        public static IEnumerable<object[]> CommonPrefixes
        {
            get
            {
                yield return new object[] { "as" };
                yield return new object[] { "at" };
                yield return new object[] { "by" };
                yield return new object[] { "do" };
                yield return new object[] { "go" };
                yield return new object[] { "if" };
                yield return new object[] { "in" };
                yield return new object[] { "is" };
                yield return new object[] { "it" };
                yield return new object[] { "no" };
                yield return new object[] { "of" };
                yield return new object[] { "on" };
                yield return new object[] { "or" };
                yield return new object[] { "to" };
            }
        }

        [Fact]
        public async Task TestValidFieldNamesAreNotReportedAsync()
        {
            var testCode = @" public class TestClass
{
    string bar, Car, fooBar, x, yz;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidVariableNamesAreNotReportedAsync()
        {
            var testCode = @" public class TestClass
{
    public void TestMethod()
    {
        string bar, Car, fooBar, x, yz;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidFieldNamesAreReportedAsync()
        {
            var testCode = @" public class TestClass
{
    string baR, caRe, daRE, fAre;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 12).WithArguments("field", "baR"),
                this.CSharpDiagnostic().WithLocation(3, 17).WithArguments("field", "caRe"),
                this.CSharpDiagnostic().WithLocation(3, 23).WithArguments("field", "daRE"),
                this.CSharpDiagnostic().WithLocation(3, 29).WithArguments("field", "fAre")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidVariableNamesAreReportedAsync()
        {
            var testCode = @" public class TestClass
{
    public void TestMethod()
    {
        string baR, caRe, daRE, fAre;
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 16).WithArguments("variable", "baR"),
                this.CSharpDiagnostic().WithLocation(5, 21).WithArguments("variable", "caRe"),
                this.CSharpDiagnostic().WithLocation(5, 27).WithArguments("variable", "daRE"),
                this.CSharpDiagnostic().WithLocation(5, 33).WithArguments("variable", "fAre")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldsAreNotReportedAsync()
        {
            var testCode = @" public interface ITestInterface
{
    event System.Action abC;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonPrefixes))]
        public async Task TestAllowedCommonPrefixesAsync(string prefix)
        {
            var testCode = $@" public class TestClass
{{
    string {prefix}R;
}}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonPrefixes))]
        public async Task TestAllowedCommonPrefixesWhenDisabledAsync(string prefix)
        {
            this.customTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowCommonHungarianPrefixes"": false,
      ""allowedHungarianPrefixes"": [ ]
    }
  }
}
";

            var testCode = $@" public class TestClass
{{
    string {prefix}R;
}}
";

            var expected = this.CSharpDiagnostic().WithLocation(3, 12).WithArguments("field", $"{prefix}R");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExcludedPrefixesAreNotReportedAsync()
        {
            this.customTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowCommonHungarianPrefixes"": false,
      ""allowedHungarianPrefixes"": [ ""ba"", ""ca"", ""da"", ""f"" ]
    }
  }
}
";

            var testCode = @" public class TestClass
{
    string baR, caRe, daRE, fAre;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1305FieldNamesMustNotUseHungarianNotation();
        }

        protected override string GetSettings()
        {
            return this.customTestSettings ?? DefaultTestSettings;
        }
    }
}
