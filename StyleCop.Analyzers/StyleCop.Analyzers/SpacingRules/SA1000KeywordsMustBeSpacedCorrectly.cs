// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The spacing around a C# keyword is incorrect.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a keyword is incorrect.</para>
    ///
    /// <para>The following C# keywords must always be followed by a single space: <strong>catch</strong>,
    /// <strong>fixed</strong>, <strong>for</strong>, <strong>foreach</strong>, <strong>from</strong>,
    /// <strong>group</strong>, <strong>if</strong>, <strong>in</strong>, <strong>into</strong>, <strong>join</strong>,
    /// <strong>let</strong>, <strong>lock</strong>, <strong>orderby</strong>, <strong>return</strong>,
    /// <strong>select</strong>, <strong>stackalloc</strong>, <strong>switch</strong>, <strong>throw</strong>,
    /// <strong>using</strong>, <strong>where</strong>, <strong>while</strong>, <strong>yield</strong>.</para>
    ///
    /// <para>The following keywords must not be followed by any space: <strong>checked</strong>,
    /// <strong>default</strong>, <strong>sizeof</strong>, <strong>typeof</strong>, <strong>unchecked</strong>.</para>
    ///
    /// <para>The <strong>new</strong> keyword should always be followed by a space, unless it is used to create a new
    /// array, in which case there should be no space between the new keyword and the opening array bracket.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1000KeywordsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1000KeywordsMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1000";
        private const string Title = "Keywords must be spaced correctly";
        private const string MessageFormat = "The keyword '{0}' must{1} be followed by a space.";
        private const string Description = "The spacing around a C# keyword is incorrect.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1000.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly ReportDiagnosticCallback<SyntaxTreeAnalysisContext> ReportSyntaxTreeDiagnostic =
            (ref SyntaxTreeAnalysisContext context, Diagnostic diagnostic) => context.ReportDiagnostic(diagnostic);

        private static readonly ReportDiagnosticCallback<SyntaxNodeAnalysisContext> ReportSyntaxNodeDiagnostic =
            (ref SyntaxNodeAnalysisContext context, Diagnostic diagnostic) => context.ReportDiagnostic(diagnostic);

        private delegate void ReportDiagnosticCallback<TContext>(ref TContext context, Diagnostic diagnostic);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // handle everything except nameof
            context.RegisterSyntaxTreeAction(SyntaxTreeAction);

            // handle nameof (which appears as an invocation expression??)
            context.RegisterSyntaxNodeAction(InvocationExpressionAction, SyntaxKind.InvocationExpression);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                switch (token.Kind())
                {
                case SyntaxKind.AwaitKeyword:
                case SyntaxKind.CaseKeyword:
                case SyntaxKind.CatchKeyword:
                case SyntaxKind.FixedKeyword:
                case SyntaxKind.ForKeyword:
                case SyntaxKind.ForEachKeyword:
                case SyntaxKind.FromKeyword:
                case SyntaxKind.GroupKeyword:
                case SyntaxKind.IfKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.IntoKeyword:
                case SyntaxKind.JoinKeyword:
                case SyntaxKind.LetKeyword:
                case SyntaxKind.LockKeyword:
                case SyntaxKind.OrderByKeyword:
                case SyntaxKind.SelectKeyword:
                case SyntaxKind.StackAllocKeyword:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.UsingKeyword:
                case SyntaxKind.WhereKeyword:
                case SyntaxKind.WhileKeyword:
                case SyntaxKind.YieldKeyword:
                    HandleRequiredSpaceToken(ref context, token);
                    break;

                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                    if (token.GetNextToken().IsKind(SyntaxKind.OpenBraceToken))
                    {
                        HandleRequiredSpaceToken(ref context, token);
                    }
                    else
                    {
                        HandleDisallowedSpaceToken(ref context, token);
                    }

                    break;

                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.NameOfKeyword:
                case SyntaxKind.SizeOfKeyword:
                case SyntaxKind.TypeOfKeyword:
                    HandleDisallowedSpaceToken(ref context, token);
                    break;

                case SyntaxKind.NewKeyword:
                    HandleNewKeywordToken(ref context, token);
                    break;

                case SyntaxKind.ReturnKeyword:
                    HandleReturnKeywordToken(ref context, token);
                    break;

                case SyntaxKind.ThrowKeyword:
                    HandleThrowKeywordToken(ref context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            InvocationExpressionSyntax invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;
            IdentifierNameSyntax identifierNameSyntax = invocationExpressionSyntax.Expression as IdentifierNameSyntax;
            if (identifierNameSyntax == null)
            {
                return;
            }

            if (identifierNameSyntax.Identifier.IsMissing)
            {
                return;
            }

            if (identifierNameSyntax.Identifier.Text != "nameof")
            {
                return;
            }

            var constantValue = context.SemanticModel.GetConstantValue(invocationExpressionSyntax, context.CancellationToken);
            if (constantValue.HasValue && !string.IsNullOrEmpty(constantValue.Value as string))
            {
                // this is a nameof expression
                HandleDisallowedSpaceToken(ref context, identifierNameSyntax.Identifier);
            }
        }

        private static void HandleRequiredSpaceToken(ref SyntaxTreeAnalysisContext context, SyntaxToken token)
            => HandleRequiredSpaceToken(ReportSyntaxTreeDiagnostic, ref context, token);

        private static void HandleRequiredSpaceToken<TContext>(ReportDiagnosticCallback<TContext> reportDiagnostic, ref TContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            if (token.HasTrailingTrivia)
            {
                if (token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    return;
                }

                if (token.TrailingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return;
                }
            }

            reportDiagnostic(ref context, Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.InsertFollowing, token.Text, string.Empty));
        }

        private static void HandleDisallowedSpaceToken(ref SyntaxTreeAnalysisContext context, SyntaxToken token)
            => HandleDisallowedSpaceToken(ReportSyntaxTreeDiagnostic, ref context, token);

        private static void HandleDisallowedSpaceToken(ref SyntaxNodeAnalysisContext context, SyntaxToken token)
            => HandleDisallowedSpaceToken(ReportSyntaxNodeDiagnostic, ref context, token);

        private static void HandleDisallowedSpaceToken<TContext>(ReportDiagnosticCallback<TContext> reportDiagnostic, ref TContext context, SyntaxToken token)
        {
            if (token.IsMissing || !token.HasTrailingTrivia)
            {
                return;
            }

            if (!token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
            {
                return;
            }

            reportDiagnostic(ref context, Diagnostic.Create(Descriptor, token.GetLocation(), TokenSpacingProperties.RemoveFollowing, token.Text, " not"));
        }

        private static void HandleNewKeywordToken(ref SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            // if the next token is [ or (, then treat as disallowed
            SyntaxToken nextToken = token.GetNextToken();
            if (nextToken.IsKind(SyntaxKind.OpenBracketToken) || nextToken.IsKind(SyntaxKind.OpenParenToken))
            {
                if (token.Parent.IsKind(SyntaxKind.ImplicitArrayCreationExpression))
                {
                    // This is handled by SA1026
                    return;
                }

                HandleDisallowedSpaceToken(ref context, token);
                return;
            }

            // otherwise treat as required
            HandleRequiredSpaceToken(ref context, token);
        }

        private static void HandleReturnKeywordToken(ref SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            /* if the next token is ; or :, then treat as disallowed
             *   1. return;
             *   2. [return: Attribute(...)]
             */
            SyntaxToken nextToken = token.GetNextToken();
            if (nextToken.IsKind(SyntaxKind.SemicolonToken) || nextToken.IsKind(SyntaxKind.ColonToken))
            {
                HandleDisallowedSpaceToken(ref context, token);
                return;
            }

            // otherwise treat as required
            HandleRequiredSpaceToken(ref context, token);
        }

        private static void HandleThrowKeywordToken(ref SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            /* if the next token is ;, then treat as disallowed:
             *    throw;
             */
            SyntaxToken nextToken = token.GetNextToken();
            if (nextToken.IsKind(SyntaxKind.SemicolonToken))
            {
                HandleDisallowedSpaceToken(ref context, token);
                return;
            }

            // otherwise treat as required
            HandleRequiredSpaceToken(ref context, token);
        }
    }
}
