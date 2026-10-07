// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.CSharp7.DocumentationRules;
    using Xunit;

    public partial class SA1633CSharp8UnitTests : SA1633CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestNullableDirectiveBeforeFileHeaderAsync()
        {
            var testCode = @"#nullable enable
// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace TestNamespace
{
}
";

            var expected = Diagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
