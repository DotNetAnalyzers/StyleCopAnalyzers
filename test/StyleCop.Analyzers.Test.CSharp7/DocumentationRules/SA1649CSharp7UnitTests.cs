// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.DocumentationRules
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Test.DocumentationRules;
    using Xunit;

    public partial class SA1649CSharp7UnitTests : SA1649UnitTests
    {
        [Fact]
        [WorkItem(2277, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2277")]
        public async Task VerifyCodeFixRenamesDocumentInPlaceAsync()
        {
            // Starting with this Roslyn version, Solution.WithDocumentName is available, so when the host supports
            // document info changes the code fix renames the document in place (keeping its DocumentId) instead of
            // removing it and adding a new one.
            using (var workspace = new AdhocWorkspace())
            {
                var (originalId, fixedSolution) = await ApplyCodeFixToMisnamedDocumentAsync(workspace).ConfigureAwait(false);

                var fixedDocument = Assert.Single(Assert.Single(fixedSolution.Projects).Documents);
                Assert.Equal(originalId, fixedDocument.Id);
                Assert.Equal("TestType.cs", fixedDocument.Name);
            }
        }
    }
}
