// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpecialRules
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class SA0000Roslyn7446Workaround : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA0000Roslyn7446Workaround"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA0000";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpecialResources.SA0000Title), SpecialResources.ResourceManager, typeof(SpecialResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpecialResources.SA0000MessageFormat), SpecialResources.ResourceManager, typeof(SpecialResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpecialResources.SA0000Description), SpecialResources.ResourceManager, typeof(SpecialResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA0000Roslyn7446Workaround.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpecialRules, DiagnosticSeverity.Hidden, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

        private static readonly bool CallGetDeclarationDiagnostics;

        static SA0000Roslyn7446Workaround()
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
