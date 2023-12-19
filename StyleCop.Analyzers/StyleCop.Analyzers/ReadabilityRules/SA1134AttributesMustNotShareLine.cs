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
    /// An attribute is placed on the same line of code as another attribute or element.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1134AttributesMustNotShareLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// Properties key used to indicate that a code fix should be inserted before the attribute.
        /// </summary>
        public const string FixWithNewLineBeforeKey = "FixWithNewLineBefore";

        /// <summary>
        /// Properties key used to indicate that a code fix should be inserted after the attribute.
        /// </summary>
        public const string FixWithNewLineAfterKey = "FixWithNewLineAfter";

        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1134AttributesMustNotShareLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1134";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1134.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1134Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1134MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1134Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> HandleAttributeListAction = HandleAttributeList;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(HandleAttributeListAction, SyntaxKind.AttributeList);
        }

        private static void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var diagnosticProperties = ImmutableDictionary.CreateBuilder<string, string>();
            AttributeListSyntax attributeList = (AttributeListSyntax)context.Node;
            bool violation = false;

            if (attributeList.Parent.IsKind(SyntaxKind.Parameter) || attributeList.Parent.IsKind(SyntaxKind.TypeParameter))
            {
                // no analysis required for parameters or type (generic) parameters
                return;
            }

            var attributeListLineSpan = attributeList.GetLineSpan();

            var prevToken = attributeList.OpenBracketToken.GetPreviousToken();
            if (!prevToken.IsMissingOrDefault())
            {
                var prevTokenLineSpan = prevToken.GetLineSpan();
                if (prevTokenLineSpan.EndLinePosition.Line == attributeListLineSpan.EndLinePosition.Line)
                {
                    diagnosticProperties.Add(FixWithNewLineBeforeKey, string.Empty);
                    violation = true;
                }
            }

            var nextToken = attributeList.CloseBracketToken.GetNextToken();
            if (!nextToken.IsMissingOrDefault())
            {
                var nextTokenLineSpan = nextToken.GetLineSpan();

                // do not report for trailing attribute lists, to prevent unnecessary diagnostics and issues with the code fix
                if ((nextTokenLineSpan.EndLinePosition.Line == attributeListLineSpan.EndLinePosition.Line) && !nextToken.Parent.IsKind(SyntaxKind.AttributeList))
                {
                    diagnosticProperties.Add(FixWithNewLineAfterKey, string.Empty);
                    violation = true;
                }
            }

            if (violation)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, attributeList.OpenBracketToken.GetLocation(), diagnosticProperties.ToImmutable()));
            }
        }
    }
}
