// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The C# code contains a region.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever a region is placed anywhere within the code. In many editors,
    /// including Visual Studio, the region will appear collapsed by default, hiding the code within the region. It is
    /// generally a bad practice to hide code, as this can lead to bad decisions as the code is maintained over
    /// time.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1124DoNotUseRegions : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1124DoNotUseRegions"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1124";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1124Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1124MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1124Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1124.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> RegionDirectiveTriviaAction = HandleRegionDirectiveTrivia;

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
            context.RegisterSyntaxNodeActionHonorExclusions(RegionDirectiveTriviaAction, SyntaxKind.RegionDirectiveTrivia);
        }

        private static void HandleRegionDirectiveTrivia(SyntaxNodeAnalysisContext context)
        {
            RegionDirectiveTriviaSyntax regionSyntax = (RegionDirectiveTriviaSyntax)context.Node;

            // regions that are completely inside a body are handled by SA1123.
            if (!SA1123DoNotPlaceRegionsWithinElements.IsCompletelyContainedInBody(regionSyntax))
            {
                // Regions must not be used.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, regionSyntax.GetLocation()));
            }
        }
    }
}
