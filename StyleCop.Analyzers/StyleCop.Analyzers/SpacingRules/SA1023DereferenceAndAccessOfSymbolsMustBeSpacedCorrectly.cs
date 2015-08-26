﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A dereference symbol or an access-of symbol within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a dereference or access-of symbol is not
    /// correct.</para>
    ///
    /// <para>The spacing around the symbol depends upon whether the symbol is used within a type declaration. If so,
    /// the symbol must always be followed by a single space, unless it is the last character on the line, or is
    /// followed by an opening square bracket or a parenthesis. In addition, the symbol should not be preceded by
    /// whitespace, and should not be the first character on the line. An example of a properly spaced dereference
    /// symbol used within a type declaration is:</para>
    ///
    /// <code language="cs">
    /// object* x = null;
    /// </code>
    ///
    /// <para>When a dereference or access-of symbol is used outside of a type declaration, the opposite rule applies.
    /// In this case, the symbol must always be preceded by a single space, unless it is the first character on the
    /// line, or is preceded by an opening square bracket, a parenthesis or a symbol of the same type i.e. an equals.
    /// The symbol should not be followed by whitespace, and should not be the last character on the line. For
    /// example:</para>
    ///
    /// <code language="cs">
    /// y = *x;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1023";
        private const string Title = "Dereference and access of symbols must be spaced correctly";
        private const string MessageFormat = "Dereference symbol '*' must {0}.";
        private const string Description = "A dereference symbol or an access-of symbol within a C# element is not spaced correctly.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1023.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(HandleSyntaxTree);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                if (token.IsKind(SyntaxKind.AsteriskToken))
                {
                    HandleAsteriskToken(context, token);
                }
            }
        }

        private static void HandleAsteriskToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            bool allowAtLineStart;
            bool allowAtLineEnd;
            bool allowPrecedingSpace;
            bool allowTrailingSpace;
            switch (token.Parent.Kind())
            {
            case SyntaxKind.PointerType:
                allowAtLineStart = false;
                allowAtLineEnd = true;
                allowPrecedingSpace = false;
                var nextToken = token.GetNextToken();
                switch (nextToken.Kind())
                {
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.OpenParenToken:
                    allowTrailingSpace = false;
                    break;
                default:
                    allowTrailingSpace = true;
                    break;
                }

                break;

            case SyntaxKind.PointerIndirectionExpression:
                allowAtLineStart = true;
                allowAtLineEnd = false;
                allowTrailingSpace = false;
                var prevToken = token.GetPreviousToken();
                switch (prevToken.Kind())
                {
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CloseParenToken:
                    allowPrecedingSpace = false;
                    break;
                default:
                    allowPrecedingSpace = true;
                    break;
                }

                break;

            default:
                return;
            }

            bool firstInLine = token.IsFirstInLine();
            bool precededBySpace = firstInLine || token.IsPrecededByWhitespace();
            bool followedBySpace = token.IsFollowedByWhitespace();
            bool lastInLine = token.IsLastInLine();

            if (!allowAtLineStart && firstInLine)
            {
                // Dereference symbol '*' must {not appear at the beginning of a line}.
                var properties = new Dictionary<string, string>
                {
                    [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationPreceding,
                    [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionRemove
                };
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), "not appear at the beginning of a line"));
            }
            else if (!allowPrecedingSpace && precededBySpace)
            {
                // Dereference symbol '*' must {not be preceded by a space}.
                var properties = new Dictionary<string, string>
                {
                    [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationPreceding,
                    [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionRemove
                };
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), "not be preceded by a space"));
            }

            if (!allowAtLineEnd && lastInLine)
            {
                // Dereference symbol '*' must {not appear at the end of a line}.
                var properties = new Dictionary<string, string>
                {
                    [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationFollowing,
                    [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionRemove
                };
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), "not appear at the end of a line"));
            }
            else if (!allowTrailingSpace && followedBySpace)
            {
                // Dereference symbol '*' must {not be followed by a space}.
                var properties = new Dictionary<string, string>
                {
                    [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationFollowing,
                    [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionRemove
                };
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), "not be followed by a space"));
            }

            if (!followedBySpace && allowTrailingSpace)
            {
                // Dereference symbol '*' must {be followed by a space}.
                var properties = new Dictionary<string, string>
                {
                    [OpenCloseSpacingCodeFixProvider.LocationKey] = OpenCloseSpacingCodeFixProvider.LocationFollowing,
                    [OpenCloseSpacingCodeFixProvider.ActionKey] = OpenCloseSpacingCodeFixProvider.ActionInsert
                };
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties.ToImmutableDictionary(), "be followed by a space"));
            }
        }
    }
}
