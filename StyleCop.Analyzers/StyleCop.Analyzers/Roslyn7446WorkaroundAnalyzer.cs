// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class Roslyn7446WorkaroundAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="Roslyn7446WorkaroundAnalyzer"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "Roslyn7446WorkaroundAnalyzer";
        private const string Title = "Roslyn Bug 7446 Workaround";
        private const string MessageFormat = "Roslyn Bug 7446 Workaround";
        private const string Description = "Roslyn Bug 7446 Workaround";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Roslyn7446WorkaroundAnalyzer.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

        private static readonly bool CallGetDeclarationDiagnostics;

        static Roslyn7446WorkaroundAnalyzer()
        {
            // dotnet/roslyn#7446 was fixed for Roslyn 1.2
            CallGetDeclarationDiagnostics = typeof(AdditionalText).GetTypeInfo().Assembly.GetName().Version < new Version(1, 2, 0, 0);
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            if (!CallGetDeclarationDiagnostics)
            {
                return;
            }

            context.RegisterCompilationStartAction(CompilationStartAction);
        }

#pragma warning disable RS1012 // Start action has no registered actions.
        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
#pragma warning restore RS1012 // Start action has no registered actions.
        {
            context.Compilation.GetDeclarationDiagnostics(context.CancellationToken);
        }
    }
}
