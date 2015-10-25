// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A line within a documentation header above a C# element does not begin with a single space.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a line within a documentation header does not begin with a single
    /// space. For example:</para>
    ///
    /// <code language="cs">
    /// ///&lt;summary&gt;
    /// ///The summary text.
    /// ///&lt;/summary&gt;
    /// ///   &lt;param name="x"&gt;The document root.&lt;/param&gt;
    /// ///    &lt;param name="y"&gt;The Xml header token.&lt;/param&gt;
    /// private void Method1(int x, int y)
    /// {
    /// }
    /// </code>
    ///
    /// <para>The header lines should begin with a single space after the three leading forward slashes:</para>
    ///
    /// <code language="cs">
    /// /// &lt;summary&gt;
    /// /// The summary text.
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="x"&gt;The document root.&lt;/param&gt;
    /// /// &lt;param name="y"&gt;The Xml header token.&lt;/param&gt;
    /// private void Method1(int x, int y)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1004DocumentationLinesMustBeginWithSingleSpace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1004DocumentationLinesMustBeginWithSingleSpace"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1004";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1004Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1004MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1004Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1004.md";

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
            foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                switch (trivia.Kind())
                {
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    HandleDocumentationCommentExteriorTrivia(context, trivia);
                    break;

                default:
                    break;
                }
            }
        }

        private static void HandleDocumentationCommentExteriorTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
        {
            SyntaxToken token = trivia.Token;
            if (token.IsMissing)
            {
                return;
            }

            switch (token.Kind())
            {
            case SyntaxKind.EqualsToken:
            case SyntaxKind.DoubleQuoteToken:
            case SyntaxKind.SingleQuoteToken:
            case SyntaxKind.IdentifierToken:
            case SyntaxKind.GreaterThanToken:
            case SyntaxKind.SlashGreaterThanToken:
            case SyntaxKind.LessThanToken:
            case SyntaxKind.LessThanSlashToken:
            case SyntaxKind.XmlCommentStartToken:
            case SyntaxKind.XmlCommentEndToken:
            case SyntaxKind.XmlCDataStartToken:
            case SyntaxKind.XmlCDataEndToken:
                if (!token.HasLeadingTrivia)
                {
                    break;
                }

                SyntaxTrivia lastLeadingTrivia = token.LeadingTrivia.Last();
                switch (lastLeadingTrivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    if (lastLeadingTrivia.ToFullString().StartsWith(" ", StringComparison.Ordinal))
                    {
                        return;
                    }

                    break;

                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    if (lastLeadingTrivia.ToFullString().EndsWith(" "))
                    {
                        return;
                    }

                    break;

                default:
                    break;
                }

                break;

            case SyntaxKind.EndOfDocumentationCommentToken:
            case SyntaxKind.XmlTextLiteralNewLineToken:
                return;

            case SyntaxKind.XmlTextLiteralToken:
                if (token.Text.StartsWith("  ", StringComparison.Ordinal))
                {
                    SyntaxKind grandparentKind = token.Parent?.Parent?.Kind() ?? SyntaxKind.None;
                    if (grandparentKind != SyntaxKind.SingleLineDocumentationCommentTrivia
                        && grandparentKind != SyntaxKind.MultiLineDocumentationCommentTrivia)
                    {
                        // Allow extra indentation for nested text and elements.
                        return;
                    }
                }
                else if (token.Text.StartsWith(" ", StringComparison.Ordinal))
                {
                    return;
                }
                else if (trivia.ToFullString().EndsWith(" "))
                {
                    // javadoc-style documentation comments without a leading * on one of the lines.
                    return;
                }

                break;

            default:
                break;
            }

            // Documentation line must begin with a space.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
        }
    }
}
