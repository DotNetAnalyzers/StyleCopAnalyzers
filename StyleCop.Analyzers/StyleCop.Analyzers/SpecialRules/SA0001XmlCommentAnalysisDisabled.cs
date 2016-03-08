// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpecialRules
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The project is configured to not parse XML documentation comments.
    /// </summary>
    [NoCodeFix("The necessary actions for this code fix are not supported by the analysis infrastructure.")]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA0001XmlCommentAnalysisDisabled : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA0001XmlCommentAnalysisDisabled"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA0001";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA0001.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpecialResources.SA0001Title), SpecialResources.ResourceManager, typeof(SpecialResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpecialResources.SA0001MessageFormat), SpecialResources.ResourceManager, typeof(SpecialResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpecialResources.SA0001Description), SpecialResources.ResourceManager, typeof(SpecialResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpecialRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            Analyzer analyzer = new Analyzer();
            context.RegisterSyntaxTreeAction(analyzer.HandleSyntaxTree);
            context.RegisterCompilationEndAction(analyzer.HandleCompilation);
        }

        private sealed class Analyzer
        {
            private bool documentationAnalysisDisabled;

            public void HandleCompilation(CompilationAnalysisContext context)
            {
                if (Volatile.Read(ref this.documentationAnalysisDisabled))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.None));
                }
            }

            public void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
            {
                if (context.Tree.Options.DocumentationMode == DocumentationMode.None)
                {
                    if (context.Tree.IsWhitespaceOnly(context.CancellationToken))
                    {
                        // Handling of empty documents is now the responsibility of the analyzers
                        return;
                    }

                    Volatile.Write(ref this.documentationAnalysisDisabled, true);
                }
            }
        }
    }
}
