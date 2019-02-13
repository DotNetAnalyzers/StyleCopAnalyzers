// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<Analyzers.DocumentationRules.SA1601PartialElementsMustBeDocumented>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1601PartialElementsMustBeDocumented"/>.
    /// </summary>
    public class SA1601UnitTests
    {
        private const string TestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentPrivateElements"": true
    }
  }
}
";

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestPartialTypeWithDocumentationAsync(string typeKeyword)
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial {0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestPartialTypeWithoutDocumentationAsync(string typeKeyword)
        {
            var testCode = @"
public partial {0}
TypeName
{{
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 1);

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), expected, CancellationToken.None).ConfigureAwait(false);

            // The same situation is allowed if 'documentExposedElements' and 'documentInterfaces' is false
            string interfaceSettingName = typeKeyword == "interface" ? "documentInterfaces" : "ignoredProperty";
            var currentTestSettings = $@"
{{
  ""settings"": {{
    ""documentationRules"": {{
      ""documentExposedElements"": false,
      ""{interfaceSettingName}"": false
    }}
  }}
}}
";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), currentTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestPartialClassWithEmptyDocumentationAsync(string typeKeyword)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial {0} 
TypeName
{{
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), expected, CancellationToken.None).ConfigureAwait(false);

            // The same situation is allowed if 'documentExposedElements' and 'documentInterfaces' is false
            string interfaceSettingName = typeKeyword == "interface" ? "documentInterfaces" : "ignoredProperty";
            var currentTestSettings = $@"
{{
  ""settings"": {{
    ""documentationRules"": {{
      ""documentExposedElements"": false,
      ""{interfaceSettingName}"": false
    }}
  }}
}}
";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), currentTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial class TypeName
{
    /// <summary>
    /// Some Documentation
    /// </summary>
    partial void MemberName();
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial class TypeName
{
    partial void MemberName();
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 18);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            // The same situation is allowed if 'documentPrivateElements' is false (the default)
            await VerifyCSharpDiagnosticAsync(testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodWithEmptyDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial class TypeName
{
    /// <summary>
    /// 
    /// </summary>
    partial void MemberName();
}";

            DiagnosticResult expected = Diagnostic().WithLocation(10, 18);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            // The same situation is allowed if 'documentPrivateElements' is false (the default)
            await VerifyCSharpDiagnosticAsync(testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, TestSettings, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, TestSettings, expected, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new StyleCopDiagnosticVerifier<SA1601PartialElementsMustBeDocumented>.CSharpTest
            {
                TestCode = source,
                Settings = testSettings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
