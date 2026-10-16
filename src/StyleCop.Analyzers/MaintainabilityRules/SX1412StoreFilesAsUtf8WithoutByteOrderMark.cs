// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules;

using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Store files without the UTF-8 byte order mark.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class SX1412StoreFilesAsUtf8WithoutByteOrderMark : DiagnosticAnalyzer
{
    /// <summary>
    /// The ID for diagnostics produced by this analyzer.
    /// </summary>
    public const string DiagnosticId = "SX1412";

    private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SX1412Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SX1412MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SX1412Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

    private static readonly DiagnosticDescriptor Descriptor =
        new(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink);

    private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;
    private static readonly Encoding Utf8WithoutByteOrderMark = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(SyntaxTreeAction);
    }

    private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        if (context.Tree.IsWhitespaceOnly(context.CancellationToken))
        {
            return;
        }

        Encoding encoding = context.Tree.Encoding;
        if (encoding == null
            || !string.Equals(encoding.WebName, Utf8WithoutByteOrderMark.WebName, StringComparison.OrdinalIgnoreCase)
            || encoding.GetPreamble().Length != 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, new TextSpan(0, 0))));
        }
    }
}
