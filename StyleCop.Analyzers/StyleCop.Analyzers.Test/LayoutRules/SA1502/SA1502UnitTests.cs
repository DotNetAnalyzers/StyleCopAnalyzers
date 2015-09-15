// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    /// <remarks>
    /// The test cases can be found in the SA1502 subfolder.
    /// </remarks>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1502ElementMustNotBeOnASingleLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1502CodeFixProvider();
        }

        private static string FormatTestCode(string testCode, string placeHolderReplacement)
        {
            return testCode.Replace("##PH##", placeHolderReplacement);
        }
    }
}
