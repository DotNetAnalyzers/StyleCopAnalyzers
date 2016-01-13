// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The C# code contains a tab character.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a tab character.</para>
    ///
    /// <para>Tabs should not be used within C# code, because the length of the tab character can vary depending upon
    /// the editor being used to view the code. This can cause the spacing and indexing of the code to vary from the
    /// developer's original intention, and can in some cases make the code difficult to read.</para>
    ///
    /// <para>For these reasons, tabs should not be used, and each level of indentation should consist of four spaces.
    /// This will ensure that the code looks the same no matter which editor is being used to view the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1027TabsMustNotBeUsed : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1027TabsMustNotBeUsed"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1027";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1027Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1027MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1027Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1027.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext, StyleCopSettings> SyntaxTreeAction = HandleSyntaxTree;

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

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context, StyleCopSettings settings)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                switch (trivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    HandleWhitespaceTrivia(context, settings.Indentation, trivia, context.CancellationToken);
                    break;

                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    HandleDocumentationCommentTrivia(context, trivia);
                    break;

                default:
                    break;
                }
            }
        }

        private static void HandleWhitespaceTrivia(SyntaxTreeAnalysisContext context, IndentationSettings indentationSettings, SyntaxTrivia trivia, CancellationToken cancellationToken)
        {
            bool potentiallyMultiLineToken = trivia.IsKind(SyntaxKind.MultiLineCommentTrivia);

            string fullString = trivia.ToFullString();
            if (fullString.StartsWith("////"))
            {
                // This is a line of commented code.
                return;
            }

            bool missingRequiredTabs = false;
            int preventTabsAtOrAfterIndex = 0;
            if (indentationSettings.UseTabs && StartsAtStartOfLine(trivia, cancellationToken))
            {
                int tabSize = indentationSettings.TabSize;
                int spaceCount = 0;

                int i;
                for (i = 0; i < fullString.Length; i++)
                {
                    char c = fullString[i];
                    if (c == '\t')
                    {
                        // This is a sneaky check for tabs after spaces
                        if (spaceCount > 0)
                        {
                            missingRequiredTabs = true;
                            break;
                        }
                    }
                    else if (c == ' ')
                    {
                        spaceCount++;
                        if (spaceCount >= tabSize)
                        {
                            missingRequiredTabs = true;
                            break;
                        }
                    }
                    else
                    {
                        // Found a non-whitespace character
                        break;
                    }
                }

                preventTabsAtOrAfterIndex = i;
            }

            if (!missingRequiredTabs && fullString.IndexOf('\t', preventTabsAtOrAfterIndex) < 0)
            {
                // No hard tabs were found.
                return;
            }

            // Tabs must not be used.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, trivia.GetLocation()));
        }

        private static bool StartsAtStartOfLine(SyntaxTrivia trivia, CancellationToken cancellationToken)
        {
            var fullSpan = trivia.FullSpan;
            var lineSpan = trivia.SyntaxTree.GetLineSpan(fullSpan, cancellationToken);
            return lineSpan.StartLinePosition.Character == 0;
        }

        private static void HandleWhitespaceToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            // Whitespace tokens only appear in XML documentation comments, and never appear at the start of a line
            // (even for /** */ documentation comments). All tabs in these tokens should be reported, regardless of the
            // UseTabs setting.
            string text = token.Text;
            if (text.IndexOf('\t') < 0)
            {
                // No hard tabs were found.
                return;
            }

            // Tabs must not be used.
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
        }

        private static void HandleDocumentationCommentTrivia(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
        {
            foreach (var token in trivia.GetStructure().DescendantTokens(descendIntoTrivia: true))
            {
                switch (token.Kind())
                {
                case SyntaxKind.XmlTextLiteralToken:
                    HandleWhitespaceToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }
    }
}
