// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// A closing square bracket within a C# statement is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a closing square bracket is not correct.</para>
    ///
    /// <para>A closing square bracket should never be preceded by whitespace, unless it is the first character on the
    /// line.</para>
    ///
    /// <para>A closing square bracket should be followed by whitespace, unless it is the last character on the line, it
    /// is followed by a closing bracket or an opening parenthesis, it is followed by a comma or semicolon, it is
    /// followed by a string interpolation alignment component or string interpolation formatting component, or it is
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
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1011.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1011Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1011MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1011Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

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
            bool precededBySpace = firstInLine || token.IsPrecededByWhitespace(context.CancellationToken);
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
                case SyntaxKind.MinusGreaterThanToken:
                case SyntaxKind.QuestionToken:
                case SyntaxKindEx.DotDotToken:
                    precedesSpecialCharacter = true;
                    break;

                case SyntaxKind.ExclamationToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusMinusToken:
                    precedesSpecialCharacter = true;
                    suppressFollowingSpaceError = false;
                    break;

                case SyntaxKind.LessThanToken:
                    precedesSpecialCharacter = token.Parent.IsKind(SyntaxKindEx.FunctionPointerUnmanagedCallingConventionList);
                    suppressFollowingSpaceError = false;
                    break;

                case SyntaxKind.GreaterThanToken:
                    precedesSpecialCharacter = nextToken.Parent.IsKind(SyntaxKind.TypeArgumentList);
                    break;

                case SyntaxKind.QuestionToken:
                    precedesSpecialCharacter = nextToken.Parent.IsKind(SyntaxKind.ConditionalAccessExpression) || nextToken.Parent.IsKind(SyntaxKind.NullableType);
                    break;

                case SyntaxKind.CloseBraceToken:
                    precedesSpecialCharacter = nextToken.Parent is InterpolationSyntax;
                    break;

                case SyntaxKind.ColonToken:
                    precedesSpecialCharacter =
                        nextToken.Parent.IsKind(SyntaxKind.InterpolationFormatClause) ||
                        nextToken.Parent.IsKind(SyntaxKindEx.CasePatternSwitchLabel);
                    suppressFollowingSpaceError = false;
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
                // Closing square bracket should{ not} be {preceded} by a space.
                var properties = TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, " not", "preceded"));
            }

            if (!lastInLine)
            {
                if (!precedesSpecialCharacter && !followedBySpace)
                {
                    // Closing square bracket should{} be {followed} by a space.
                    var properties = TokenSpacingProperties.InsertFollowing;
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, string.Empty, "followed"));
                }
                else if (precedesSpecialCharacter && followedBySpace && !suppressFollowingSpaceError)
                {
                    // Closing square brackets should {not} be {followed} by a space
                    var properties = TokenSpacingProperties.RemoveFollowing;
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, " not", "followed"));
                }
            }
        }
    }
}
