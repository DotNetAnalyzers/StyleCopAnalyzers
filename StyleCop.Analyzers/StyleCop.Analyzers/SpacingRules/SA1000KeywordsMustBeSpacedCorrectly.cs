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
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// The spacing around a C# keyword is incorrect.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a keyword is incorrect.</para>
    ///
    /// <para>The following C# keywords should always be followed by a single space: <strong>catch</strong>,
    /// <strong>fixed</strong>, <strong>for</strong>, <strong>foreach</strong>, <strong>from</strong>,
    /// <strong>group</strong>, <strong>if</strong>, <strong>in</strong>, <strong>into</strong>, <strong>join</strong>,
    /// <strong>let</strong>, <strong>lock</strong>, <strong>orderby</strong>, <strong>out</strong>,
    /// <strong>ref</strong>, <strong>return</strong>, <strong>select</strong>, <strong>switch</strong>,
    /// <strong>throw</strong>, <strong>using</strong>, <strong>var</strong>, <strong>where</strong>,
    /// <strong>while</strong>, <strong>yield</strong>.</para>
    ///
    /// <para>The following keywords should not be followed by any space: <strong>checked</strong>,
    /// <strong>default</strong>, <strong>sizeof</strong>, <strong>typeof</strong>, <strong>unchecked</strong>.</para>
    ///
    /// <para>The <strong>new</strong> and <strong>stackalloc</strong> keywords should always be followed by a space,
    /// except where used to create a new implicitly-typed array, in which case there should be no space between the
    /// keyword and the opening array bracket.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1000KeywordsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1000KeywordsMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1000";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1000.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1000Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1000MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1000Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> IdentifierNameAction = HandleIdentifierName;
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

            // handle var (which appears as an identifier name??)
            context.RegisterSyntaxNodeAction(IdentifierNameAction, SyntaxKind.IdentifierName);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                switch (token.Kind())
                {
                case SyntaxKindEx.AndKeyword:
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
                case SyntaxKind.IsKeyword:
                case SyntaxKind.JoinKeyword:
                case SyntaxKind.LetKeyword:
                case SyntaxKind.LockKeyword:
                case SyntaxKindEx.NotKeyword:
                case SyntaxKindEx.OrKeyword:
                case SyntaxKind.OrderByKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.SelectKeyword:
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
                    if (token.Parent.IsKind(SyntaxKindEx.DefaultLiteralExpression))
                    {
                        // Ignore spacing around a default literal expression for now
                        break;
                    }

                    HandleDisallowedSpaceToken(ref context, token);
                    break;

                case SyntaxKind.NameOfKeyword:
                case SyntaxKind.SizeOfKeyword:
                case SyntaxKind.TypeOfKeyword:
                    HandleDisallowedSpaceToken(ref context, token);
                    break;

                case SyntaxKind.NewKeyword:
                case SyntaxKind.StackAllocKeyword:
                    HandleNewOrStackAllocKeywordToken(ref context, token);
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
            if (!(invocationExpressionSyntax.Expression is IdentifierNameSyntax identifierNameSyntax))
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

        private static void HandleIdentifierName(SyntaxNodeAnalysisContext context)
        {
            var identifierNameSyntax = (IdentifierNameSyntax)context.Node;
            if (identifierNameSyntax.IsVar)
            {
                var nextToken = identifierNameSyntax.Identifier.GetNextToken();
                switch (nextToken.Kind())
                {
                case SyntaxKind.IdentifierToken:
                case SyntaxKindEx.UnderscoreToken:
                    // Always check these
                    break;

                case SyntaxKind.OpenParenToken:
                    if (nextToken.Parent.IsKind(SyntaxKindEx.ParenthesizedVariableDesignation))
                    {
                        // We have something like this:
                        //   var (x, i) = (a, b);
                        break;
                    }

                    // Could be calling a function named 'var'
                    return;

                default:
                    // Not something to check
                    return;
                }

                HandleRequiredSpaceToken(ref context, identifierNameSyntax.Identifier);
            }
        }

        private static void HandleRequiredSpaceToken(ref SyntaxTreeAnalysisContext context, SyntaxToken token)
            => HandleRequiredSpaceToken(ReportSyntaxTreeDiagnostic, ref context, token);

        private static void HandleRequiredSpaceToken(ref SyntaxNodeAnalysisContext context, SyntaxToken token)
            => HandleRequiredSpaceToken(ReportSyntaxNodeDiagnostic, ref context, token);

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

        private static void HandleNewOrStackAllocKeywordToken(ref SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            bool needSpace;
            SyntaxToken nextToken = token.GetNextToken();
            switch (nextToken.Kind())
            {
            case SyntaxKind.OpenBracketToken:
                if (token.Parent.IsKind(SyntaxKind.ImplicitArrayCreationExpression)
                    || token.Parent.IsKind(SyntaxKindEx.ImplicitStackAllocArrayCreationExpression))
                {
                    // This is handled by SA1026
                    return;
                }

                // Disallowed, but can we hit this??
                needSpace = false;
                break;

            case SyntaxKind.OpenParenToken:
                // Disallowed for new() constraint, but otherwise allowed for tuple types
                needSpace = !token.Parent.IsKind(SyntaxKind.ConstructorConstraint)
                    && !token.Parent.IsKind(SyntaxKindEx.ImplicitObjectCreationExpression);
                break;

            default:
                needSpace = true;
                break;
            }

            if (!needSpace)
            {
                HandleDisallowedSpaceToken(ref context, token);
            }
            else
            {
                HandleRequiredSpaceToken(ref context, token);
            }
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
