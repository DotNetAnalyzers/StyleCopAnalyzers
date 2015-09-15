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
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1601PartialElementsMustBeDocumented"/>.
    /// </summary>
    public class SA1601UnitTests : DiagnosticVerifier
    {
        private const string SettingsFileName = "stylecop.json";
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), expected, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            return TestSettings;
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1601PartialElementsMustBeDocumented();
        }
    }
}
