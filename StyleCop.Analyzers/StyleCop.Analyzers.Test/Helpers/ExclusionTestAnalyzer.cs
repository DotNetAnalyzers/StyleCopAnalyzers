// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace TestHelper
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A analyzer that will report a diagnostic at the start of the code file if the
    /// file is not excluded from code analysis.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ExclusionTestAnalyzer : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "SA9999";
        private const string Title = "Exclusion test";
        private const string MessageFormat = "Exclusion test";
        private const string Description = "Exclusion test.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA9999.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, "TestRules", DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = AnalyzeTree;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(SyntaxTreeAction);
        }

        private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
        {
            if (context.Tree.IsWhitespaceOnly(context.CancellationToken))
            {
                // Handling of empty documents is now the responsibility of the analyzers
                return;
            }

            // Report a diagnostic if we got called
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Tree.GetLocation(TextSpan.FromBounds(0, 0))));
        }
    }
}
