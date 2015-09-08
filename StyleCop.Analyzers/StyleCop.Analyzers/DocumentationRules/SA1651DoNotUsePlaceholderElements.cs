﻿namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The documentation for the element contains one or more &lt;placeholder&gt; elements.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the element documentation contains &lt;placeholder&gt;
    /// elements:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// This method &lt;placeholder&gt;performs some operation&lt;/placeholder&gt;.
    /// /// &lt;/summary&gt;
    /// public void SomeOperation()
    /// {
    ///     ...
    /// }
    /// </code>
    ///
    /// <para>Placeholder elements should be reviewed and removed from documentation.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1651DoNotUsePlaceholderElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1651DoNotUsePlaceholderElements"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1651";
        private const string Title = "Do not use placeholder elements";
        private const string MessageFormat = "Do not use placeholder elements";
        private const string Description = "The element documentation contains a <placeholder> element.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1651.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleXmlElement, SyntaxKind.XmlElement);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleXmlEmptyElement, SyntaxKind.XmlEmptyElement);
        }

        private static void HandleXmlElement(SyntaxNodeAnalysisContext context)
        {
            XmlElementSyntax syntax = (XmlElementSyntax)context.Node;
            if (!string.Equals("placeholder", syntax.StartTag?.Name?.ToString(), StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.GetLocation()));
        }

        private static void HandleXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            XmlEmptyElementSyntax syntax = (XmlEmptyElementSyntax)context.Node;
            if (!string.Equals("placeholder", syntax.Name?.ToString(), StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.GetLocation()));
        }
    }
}
