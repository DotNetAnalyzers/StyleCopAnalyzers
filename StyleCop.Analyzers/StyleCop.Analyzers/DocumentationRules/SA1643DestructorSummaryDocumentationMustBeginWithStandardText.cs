// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The XML documentation header for a C# finalizer does not contain the appropriate summary text.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when the summary tag within the documentation header for a finalizer does
    /// not begin with the proper text.</para>
    ///
    /// <para>The rule is intended to standardize the summary text for a finalizer. The summary for a finalizer must
    /// begin with "Finalizes an instance of the {class name} class." For example, the following shows the finalizer for
    /// the <c>Customer</c> class.</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Finalizes an instance of the Customer class.
    /// /// &lt;/summary&gt;
    /// ~Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>It is possible to embed other tags into the summary text. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Finalizes an instance of the &lt;see cref="Customer"/&gt; class.
    /// /// &lt;/summary&gt;
    /// ~Customer()
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1643DestructorSummaryDocumentationMustBeginWithStandardText : StandardTextDiagnosticBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1643DestructorSummaryDocumentationMustBeginWithStandardText"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1643";
        private const string Title = "Destructor summary documentation must begin with standard text";
        private const string MessageFormat = "Destructor summary documentation must begin with standard text";
        private const string Description = "The XML documentation header for a C# finalizer does not contain the appropriate summary text.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1643.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> DestructorDeclarationAction = HandleDestructor;

        /// <summary>
        /// Gets the standard text which is expected to appear at the beginning of the <c>&lt;summary&gt;</c>
        /// documentation for a destructor.
        /// </summary>
        /// <value>
        /// A two-element array containing the standard text which is expected to appear at the beginning of the
        /// <c>&lt;summary&gt;</c> documentation for a destructor. The first element appears before the name of the
        /// containing class, followed by a <c>&lt;see&gt;</c> element targeting the containing type, and finally
        /// followed by the second element of this array.
        /// </value>
        public static ImmutableArray<string> DestructorStandardText { get; } = ImmutableArray.Create("Finalizes an instance of the ", " class.");

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
            context.RegisterSyntaxNodeActionHonorExclusions(DestructorDeclarationAction, SyntaxKind.DestructorDeclaration);
        }

        private static void HandleDestructor(SyntaxNodeAnalysisContext context)
        {
            var destructorDeclaration = context.Node as DestructorDeclarationSyntax;

            if (destructorDeclaration != null)
            {
                HandleDeclaration(context, DestructorStandardText[0], DestructorStandardText[1], Descriptor);
            }
        }
    }
}
