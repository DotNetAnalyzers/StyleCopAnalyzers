// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// More than one declaration of a partial type contains summary documentation.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[NoCodeFix("The user must choose which partial declaration retains the summary.")]
internal class SA1653OnlyOnePartialDeclarationShouldContainSummaryDocumentation : DiagnosticAnalyzer
{
    /// <summary>
    /// The ID for diagnostics produced by this analyzer.
    /// </summary>
    public const string DiagnosticId = "SA1653";

    private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1653Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1653MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1653Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

    private static readonly DiagnosticDescriptor Descriptor =
        new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(HandleNamedType, SymbolKind.NamedType);
    }

    private static void HandleNamedType(SymbolAnalysisContext context)
    {
        var references = context.Symbol.DeclaringSyntaxReferences;
        if (references.Length < 2)
        {
            return;
        }

        TypeDeclarationSyntax? firstDeclaration = null;
        bool reportedFirstDeclaration = false;
        foreach (var reference in references)
        {
            if (reference.GetSyntax(context.CancellationToken) is not TypeDeclarationSyntax declaration)
            {
                continue;
            }

            var documentation = declaration.GetDocumentationCommentTriviaSyntax();
            if (documentation == null || documentation.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag) == null)
            {
                continue;
            }

            if (firstDeclaration == null)
            {
                firstDeclaration = declaration;
                continue;
            }

            if (!reportedFirstDeclaration)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstDeclaration.Identifier.GetLocation(), context.Symbol.Name));
                reportedFirstDeclaration = true;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation(), context.Symbol.Name));
        }
    }
}
