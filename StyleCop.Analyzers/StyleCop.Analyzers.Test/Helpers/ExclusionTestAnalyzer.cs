// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace TestHelper
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers;

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
        private const string Description = "Exclusion test";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA9999.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, "TestRules", DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(AnalyzeTree);
        }

        private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
        {
            // Report a diagnostic if we got called
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Tree.GetLocation(TextSpan.FromBounds(0, 0))));
        }
    }
}
