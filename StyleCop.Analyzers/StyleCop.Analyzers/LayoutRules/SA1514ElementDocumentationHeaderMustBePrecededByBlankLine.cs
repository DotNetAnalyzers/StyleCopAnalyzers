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
    /// An element documentation header above a C# element is not preceded by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when the element documentation header above an element is not preceded by
    /// a blank line. For example:</para>
    ///
    /// <code language="csharp">
    /// public bool Visible
    /// {
    ///     get { return this.visible; }
    /// }
    /// /// &lt;summary&gt;
    /// /// Gets a value indicating whether the control is enabled.
    /// /// &lt;/summary&gt;
    /// public bool Enabled
    /// {
    ///     get { return this.enabled; }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate an instance of this violation, since the documentation header is not
    /// preceded by a blank line.</para>
    ///
    /// <para>An exception to this rule occurs when the documentation header is the first item within its scope. In this
    /// case, the header should not be preceded by a blank line. For example:</para>
    ///
    /// <code language="csharp">
    /// public class Class1
    /// {
    ///     /// &lt;summary&gt;
    ///     /// Gets a value indicating whether the control is enabled.
    ///     /// &lt;/summary&gt;
    ///     public bool Enabled
    ///     {
    ///         get { return this.enabled; }
    ///     }
    /// }
    /// </code>
    ///
    /// <para>In the code above, the header is the first item within its scope, and thus it should not be preceded by a
    /// blank line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1514ElementDocumentationHeaderMustBePrecededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1514ElementDocumentationHeaderMustBePrecededByBlankLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1514";
        private const string Title = "Element documentation header must be preceded by blank line";
        private const string MessageFormat = "Element documentation header must be preceded by blank line";
        private const string Description = "An element documentation header above a C# element is not preceded by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1514.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> HandledSyntaxKinds =
            ImmutableArray.Create(
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.EnumMemberDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.DestructorDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.OperatorDeclaration,
                SyntaxKind.ConversionOperatorDeclaration);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> DeclarationAction = HandleDeclaration;

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
            context.RegisterSyntaxNodeActionHonorExclusions(DeclarationAction, HandledSyntaxKinds);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
        {
            var nodeTriviaList = context.Node.GetLeadingTrivia();
            var documentationHeaderIndex = context.Node.GetLeadingTrivia().IndexOf(SyntaxKind.SingleLineDocumentationCommentTrivia);

            if (documentationHeaderIndex == -1)
            {
                // there is no documentation header.
                return;
            }

            var documentationHeader = nodeTriviaList[documentationHeaderIndex];
            var triviaList = TriviaHelper.GetContainingTriviaList(documentationHeader, out documentationHeaderIndex);
            var eolCount = 0;
            var done = false;
            for (var i = documentationHeaderIndex - 1; !done && (i >= 0); i--)
            {
                var trivia = triviaList[i];
                if (trivia.IsDirective
                    && !trivia.IsKind(SyntaxKind.EndIfDirectiveTrivia)
                    && !trivia.IsKind(SyntaxKind.RegionDirectiveTrivia)
                    && !trivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia))
                {
                    return;
                }

                switch (trivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    break;

                case SyntaxKind.EndIfDirectiveTrivia:
                case SyntaxKind.RegionDirectiveTrivia:
                case SyntaxKind.EndRegionDirectiveTrivia:
                    eolCount++;
                    done = true;
                    break;
                default:
                    done = true;
                    break;
                }
            }

            if (eolCount >= 2)
            {
                // there is a blank line available
                return;
            }

            if (!done)
            {
                var prevToken = documentationHeader.Token.GetPreviousToken();
                if (prevToken.IsKind(SyntaxKind.OpenBraceToken))
                {
                    // no leading blank line necessary at start of scope.
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, GetDiagnosticLocation(documentationHeader)));
        }

        private static Location GetDiagnosticLocation(SyntaxTrivia documentationHeader)
        {
            var documentationHeaderStructure = (DocumentationCommentTriviaSyntax)documentationHeader.GetStructure();
            return Location.Create(documentationHeaderStructure.SyntaxTree, documentationHeaderStructure.GetLeadingTrivia().Span);
        }
    }
}
