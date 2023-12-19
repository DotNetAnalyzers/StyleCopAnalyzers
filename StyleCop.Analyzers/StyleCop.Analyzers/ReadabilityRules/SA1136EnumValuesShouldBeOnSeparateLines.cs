// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Enum values should be placed on their own lines for maximum readability.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1136EnumValuesShouldBeOnSeparateLines : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1136EnumValuesShouldBeOnSeparateLines"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1136";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1136.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1136Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1136MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1136Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> HandleEnumDeclarationAction = HandleEnumDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(HandleEnumDeclarationAction, SyntaxKind.EnumDeclaration);
        }

        private static void HandleEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            if (enumDeclaration.Members.Count < 2)
            {
                return;
            }

            var previousLine = enumDeclaration.Members[0].GetLineSpan().EndLinePosition.Line;
            for (var i = 1; i < enumDeclaration.Members.Count; i++)
            {
                var currentMember = enumDeclaration.Members[i];
                var currentLine = currentMember.GetLineSpan().StartLinePosition.Line;

                if (currentLine == previousLine)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentMember.Identifier.GetLocation()));
                }

                previousLine = currentLine;
            }
        }
    }
}
