// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An element documentation header above a C# element is followed by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when the element documentation header above an element is followed by a
    /// blank line. For example:</para>
    ///
    /// <code language="csharp">
    /// /// <summary>
    /// /// Gets a value indicating whether the control is enabled.
    /// /// </summary>
    ///
    /// public bool Enabled
    /// {
    ///     get { return this.enabled; }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate an instance of this violation, since the documentation header is followed by
    /// a blank line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1506";
        private const string Title = "Element documentation headers must not be followed by blank line";
        private const string MessageFormat = "Element documentation headers must not be followed by blank line";
        private const string Description = "An element documentation header above a C# element is followed by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1506.md";

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
            var triviaList = context.Node.GetLeadingTrivia();

            var eolCount = 0;
            for (var i = triviaList.Count - 1; i >= 0; i--)
            {
                switch (triviaList[i].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    break;
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    eolCount--;
                    break;
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    if (eolCount > 0)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, triviaList[i + 1].GetLocation()));
                    }

                    return;
                default:
                    // no documentation found
                    return;
                }
            }
        }
    }
}
