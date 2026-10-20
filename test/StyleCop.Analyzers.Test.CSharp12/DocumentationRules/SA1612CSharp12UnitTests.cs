// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.DocumentationRules
{
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp11.DocumentationRules;
    using Xunit;

    public partial class SA1612CSharp12UnitTests : SA1612CSharp11UnitTests
    {
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [WorkItem(754, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/754")]
        public Task TestCodeFixReconcilesClassAndStructPrimaryConstructorParametersAsync(string typeKeyword)
        {
            return this.VerifyCodeFixReordersAddsAndRemovesParamDocumentationAsync($"public {typeKeyword} TypeName(string foo, string bar, string baz) {{ }}");
        }
    }
}
