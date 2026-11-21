// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules;

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.Lightup;

/// <summary>
/// A string or character literal contains a zero-width character that is invisible in most editors.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class SA1144ZeroWidthCharactersInLiteralsMustBeEscaped : DiagnosticAnalyzer
{
    /// <summary>
    /// The ID for diagnostics produced by the <see cref="SA1144ZeroWidthCharactersInLiteralsMustBeEscaped"/> analyzer.
    /// </summary>
    public const string DiagnosticId = "SA1144";

    private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1144Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1144MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1144Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

    private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);
    private static readonly Action<SyntaxNodeAnalysisContext> LiteralExpressionAction = HandleLiteralExpression;
    private static readonly Action<SyntaxNodeAnalysisContext> InterpolatedStringTextAction = HandleInterpolatedStringText;

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(LiteralExpressionAction, SyntaxKind.StringLiteralExpression, SyntaxKind.CharacterLiteralExpression, SyntaxKindEx.Utf8StringLiteralExpression);
        context.RegisterSyntaxNodeAction(InterpolatedStringTextAction, SyntaxKind.InterpolatedStringText);
    }

    /// <summary>
    /// Gets the escape sequence for a zero-width character.
    /// </summary>
    /// <param name="c">The character to escape.</param>
    /// <returns>The escape sequence.</returns>
    internal static string GetEscapeSequence(char c)
        => "\\u" + ((int)c).ToString("X4", System.Globalization.CultureInfo.InvariantCulture);

    internal static bool IsZeroWidthCharacter(char c)
    {
        switch (c)
        {
        case '\u200B': // ZERO WIDTH SPACE
        case '\u2060': // WORD JOINER
        case '\uFEFF': // ZERO WIDTH NO-BREAK SPACE
            return true;

        default:
            return false;
        }
    }

    internal static bool ContainsZeroWidthCharacter(string text)
    {
        foreach (var c in text)
        {
            if (IsZeroWidthCharacter(c))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the literal token is written with a syntax that supports unicode escape sequences.
    /// </summary>
    /// <param name="token">The literal token.</param>
    /// <returns><see langword="true"/> if an escape sequence can replace a character in the token.</returns>
    internal static bool SupportsEscapeSequences(SyntaxToken token)
    {
        var text = token.Text;
        switch (token.Kind())
        {
        case SyntaxKind.CharacterLiteralToken:
            return true;

        case SyntaxKind.InterpolatedStringTextToken:
            return token.Parent?.Parent is InterpolatedStringExpressionSyntax interpolatedString
                && interpolatedString.StringStartToken.Text == "$\"";

        default:
            return text.Length > 0 && text[0] == '"' && !text.StartsWith("\"\"\"", StringComparison.Ordinal);
        }
    }

    private static void HandleLiteralExpression(SyntaxNodeAnalysisContext context)
    {
        var token = ((LiteralExpressionSyntax)context.Node).Token;
        ReportIfNeeded(context, token);
    }

    private static void HandleInterpolatedStringText(SyntaxNodeAnalysisContext context)
    {
        var token = ((InterpolatedStringTextSyntax)context.Node).TextToken;
        ReportIfNeeded(context, token);
    }

    private static void ReportIfNeeded(SyntaxNodeAnalysisContext context, SyntaxToken token)
    {
        if (ContainsZeroWidthCharacter(token.Text))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
        }
    }
}
