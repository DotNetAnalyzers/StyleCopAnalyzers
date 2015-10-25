// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A closing square bracket within a C# statement is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a closing square bracket is not correct.</para>
    ///
    /// <para>A closing square bracket must never be preceded by whitespace, unless it is the first character on the
    /// line.</para>
    ///
    /// <para>A closing square bracket must be followed by whitespace, unless it is the last character on the line, it
    /// is followed by a closing bracket or an opening parenthesis, it is followed by a comma or semicolon, or it is
    /// followed by certain types of operator symbols.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1011ClosingSquareBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1011ClosingSquareBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1011";
        private const string Title = "Closing square brackets must be spaced correctly";
        private const string MessageFormat = "Closing square bracket must{0} be {1} by a space.";
        private const string Description = "A closing square bracket within a C# statement is not spaced correctly.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1011.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

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
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                if (token.IsKind(SyntaxKind.CloseBracketToken))
                {
                    HandleCloseBracketToken(context, token);
                }
            }
        }

        private static void HandleCloseBracketToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            // attribute brackets are handled separately
            if (token.Parent.IsKind(SyntaxKind.AttributeList))
            {
                return;
            }

            bool firstInLine = token.IsFirstInLine();
            bool precededBySpace = firstInLine || token.IsPrecededByWhitespace();
            bool followedBySpace = token.IsFollowedByWhitespace();
            bool lastInLine = token.IsLastInLine();
            bool precedesSpecialCharacter;

            // Tests for this rule have a lot of exclusions which are supposed to be caught by other rules
            bool suppressFollowingSpaceError = true;

            if (!lastInLine)
            {
                SyntaxToken nextToken = token.GetNextToken();
                switch (nextToken.Kind())
                {
                case SyntaxKind.CloseBracketToken:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.SemicolonToken:
                // TODO: "certain types of operator symbols"
                case SyntaxKind.DotToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.CloseParenToken:
                    precedesSpecialCharacter = true;
                    break;
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusMinusToken:
                    precedesSpecialCharacter = true;
                    suppressFollowingSpaceError = false;
                    break;

                case SyntaxKind.GreaterThanToken:
                    precedesSpecialCharacter = nextToken.Parent.IsKind(SyntaxKind.TypeArgumentList);
                    break;

                case SyntaxKind.QuestionToken:
                    precedesSpecialCharacter = nextToken.Parent.IsKind(SyntaxKind.ConditionalAccessExpression);
                    break;

                case SyntaxKind.CloseBraceToken:
                    precedesSpecialCharacter = nextToken.Parent is InterpolationSyntax;
                    break;

                default:
                    precedesSpecialCharacter = false;
                    break;
                }
            }
            else
            {
                precedesSpecialCharacter = false;
            }

            if (!firstInLine && precededBySpace)
            {
                // Closing square bracket must{ not} be {preceded} by a space.
                var properties = TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, " not", "preceded"));
            }

            if (!lastInLine)
            {
                if (!precedesSpecialCharacter && !followedBySpace)
                {
                    // Closing square bracket must{} be {followed} by a space.
                    var properties = TokenSpacingProperties.InsertFollowing;
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, string.Empty, "followed"));
                }
                else if (precedesSpecialCharacter && followedBySpace && !suppressFollowingSpaceError)
                {
                    // Closing square brackets must {not} be {followed} by a space
                    var properties = TokenSpacingProperties.RemoveFollowing;
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, " not", "followed"));
                }
            }
        }
    }
}
