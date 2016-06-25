// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;

    /// <summary>
    /// Unit tests for <see cref="SA1117ParametersMustBeOnSameLineOrSeparateLines"/>.
    /// </summary>
    /// <remarks>
    /// The test cases can be found in the SA1117 subfolder.
    /// </remarks>
    public partial class SA1117UnitTests : DiagnosticVerifier
    {
        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1117ParametersMustBeOnSameLineOrSeparateLines();
        }
    }
}
