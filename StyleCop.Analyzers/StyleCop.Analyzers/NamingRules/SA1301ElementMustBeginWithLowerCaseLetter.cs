// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// There are currently no situations in which this rule will fire.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This rule has no behavior by design.")]
    [NoCodeFix("Don't fix what isn't broken.")]
    internal class SA1301ElementMustBeginWithLowerCaseLetter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1301ElementMustBeginWithLowerCaseLetter"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1301";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1301.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1301Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1301MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1301Description), NamingResources.ResourceManager, typeof(NamingResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable RS2000 // Add analyzer diagnostic IDs to analyzer release.
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);
#pragma warning restore RS2000 // Add analyzer diagnostic IDs to analyzer release.
#pragma warning restore IDE0079 // Remove unnecessary suppression

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // Intentionally empty
        }
    }
}
