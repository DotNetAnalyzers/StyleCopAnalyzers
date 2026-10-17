// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Generic type arguments in cref attributes should use braces instead of escaped angle brackets.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class SA1654ProperlyEncodeGenericBrackets : DiagnosticAnalyzer
{
    /// <summary>
    /// The ID for diagnostics produced by this analyzer.
    /// </summary>
    public const string DiagnosticId = "SA1654";

    private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1654Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1654MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1654Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

    private static readonly DiagnosticDescriptor Descriptor =
        new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(HandleXmlCrefAttribute, SyntaxKind.XmlCrefAttribute);
    }

    private static void HandleXmlCrefAttribute(SyntaxNodeAnalysisContext context)
    {
        var attribute = (XmlCrefAttributeSyntax)context.Node;
        foreach (var token in attribute.Cref.DescendantTokens())
        {
            if (token.Parent is TypeArgumentListSyntax
                && (token.ValueText == "<" || token.ValueText == ">")
                && ((token.Text.Length > 0 && token.Text[0] == '&') || token.Text == ">"))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, attribute.Cref.GetLocation()));
                return;
            }
        }
    }
}
