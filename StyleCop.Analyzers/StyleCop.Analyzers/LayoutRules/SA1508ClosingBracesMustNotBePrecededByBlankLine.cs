// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A closing brace within a C# element, statement, or expression is preceded by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when a closing brace is preceded by a blank line. For example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         return this.enabled;
    ///
    ///     }
    ///
    /// }
    /// </code>
    ///
    /// <para>The code above would generate two instances of this violation, since there are two places where closing
    /// braces are preceded by blank lines.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1508ClosingBracesMustNotBePrecededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1508ClosingBracesMustNotBePrecededByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1508";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1508.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1508Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1508MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1508Description), LayoutResources.ResourceManager, typeof(LayoutResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> BlockAction = HandleBlock;
        private static readonly Action<SyntaxNodeAnalysisContext> InitializerExpressionAction = HandleInitializerExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousObjectCreationExpressionAction = HandleAnonymousObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> SwitchStatementAction = HandleSwitchStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseTypeDeclarationAction = HandleBaseTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> AccessorListAction = HandleAccessorList;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(BlockAction, SyntaxKind.Block);
            context.RegisterSyntaxNodeAction(InitializerExpressionAction, SyntaxKinds.InitializerExpression);
            context.RegisterSyntaxNodeAction(AnonymousObjectCreationExpressionAction, SyntaxKind.AnonymousObjectCreationExpression);
            context.RegisterSyntaxNodeAction(SwitchStatementAction, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(BaseTypeDeclarationAction, SyntaxKinds.BaseTypeDeclaration);
            context.RegisterSyntaxNodeAction(AccessorListAction, SyntaxKind.AccessorList);
        }

        private static void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;
            AnalyzeCloseBrace(context, block.CloseBraceToken);
        }

        private static void HandleInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (InitializerExpressionSyntax)context.Node;
            AnalyzeCloseBrace(context, expression.CloseBraceToken);
        }

        private static void HandleAnonymousObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (AnonymousObjectCreationExpressionSyntax)context.Node;
            AnalyzeCloseBrace(context, expression.CloseBraceToken);
        }

        private static void HandleSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;
            AnalyzeCloseBrace(context, switchStatement.CloseBraceToken);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
            AnalyzeCloseBrace(context, namespaceDeclaration.CloseBraceToken);
        }

        private static void HandleBaseTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (BaseTypeDeclarationSyntax)context.Node;
            AnalyzeCloseBrace(context, typeDeclaration.CloseBraceToken);
        }

        private static void HandleAccessorList(SyntaxNodeAnalysisContext context)
        {
            var accessorList = (AccessorListSyntax)context.Node;
            AnalyzeCloseBrace(context, accessorList.CloseBraceToken);
        }

        private static void AnalyzeCloseBrace(SyntaxNodeAnalysisContext context, SyntaxToken closeBraceToken)
        {
            var previousToken = closeBraceToken.GetPreviousToken();
            if ((closeBraceToken.GetLineSpan().StartLinePosition.Line - previousToken.GetLineSpan().EndLinePosition.Line) < 2)
            {
                // there will be no blank lines when the closing brace and the preceding token are not at least two lines apart.
                return;
            }

            var separatingTrivia = TriviaHelper.MergeTriviaLists(previousToken.TrailingTrivia, closeBraceToken.LeadingTrivia);

            // skip all leading whitespace for the close brace
            // the index must be checked because two tokens can be more than two lines apart and
            // still only be separated by whitespace trivia due to compilation errors
            var index = separatingTrivia.Count - 1;
            while (index >= 0 && separatingTrivia[index].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                index--;
            }

            var done = false;
            var eolCount = 0;
            while (!done && index >= 0)
            {
                switch (separatingTrivia[index].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    break;
                default:
                    done = true;
                    break;
                }

                index--;
            }

            if (eolCount > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, closeBraceToken.GetLocation()));
            }
        }
    }
}
