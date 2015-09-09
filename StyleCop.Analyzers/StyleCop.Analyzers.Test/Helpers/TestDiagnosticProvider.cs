// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;

    internal sealed class TestDiagnosticProvider : FixAllContext.DiagnosticProvider
    {
        private ImmutableArray<Diagnostic> diagnostics;

        private TestDiagnosticProvider(ImmutableArray<Diagnostic> diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<Diagnostic>>(this.diagnostics);
        }

        public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.diagnostics.Where(i => i.Location.GetLineSpan().Path == document.Name));
        }

        public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.diagnostics.Where(i => !i.Location.IsInSource));
        }

        internal static TestDiagnosticProvider Create(ImmutableArray<Diagnostic> diagnostics)
        {
            return new TestDiagnosticProvider(diagnostics);
        }
    }
}
