namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Within a C# property, indexer or event, at least one of the child accessors is written on a single line, and at
    /// least one of the child accessors is written across multiple lines.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the accessors within a property, indexer or event are not
    /// consistently written on a single line or on multiple lines. This rule is intended to increase the readability of
    /// the code by requiring all of the accessors within an element to be formatted in the same way.</para>
    ///
    /// <para>For example, the following property would generate a violation of this rule, because one accessor is
    /// written on a single line while the other accessor snaps multiple lines.</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get { return this.enabled; }
    ///
    ///     set
    ///     {
    ///         this.enabled = value;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The violation can be avoided by placing both accessors on a single line, or expanding both accessors
    /// across multiple lines:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get { return this.enabled; }
    ///     set { this.enabled = value; }
    /// }
    ///
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         return this.enabled;
    ///     }
    ///
    ///     set
    ///     {
    ///         this.enabled = value;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1504AllAccessorsMustBeSingleLineOrMultiLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1504AllAccessorsMustBeSingleLineOrMultiLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1504";
        private const string Title = "All accessors must be single-line or multi-line";
        private const string MessageFormat = "All accessors must be single-line or multi-line";
        private const string Description = "Within a C# property, indexer or event, at least one of the child accessors is written on a single line, and at least one of the child accessors is written across multiple lines.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1504.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.IndexerDeclaration, SyntaxKind.EventDeclaration);
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var property = (BasePropertyDeclarationSyntax)context.Node;

            if (property.AccessorList == null
                || property.AccessorList.IsMissing
                || property.AccessorList.Accessors.Count < 2)
            {
                return;
            }

            var accessorsAnalyzeResult = this.AnalyzeAccessorsLineSpans(property.AccessorList.Accessors);
            if (accessorsAnalyzeResult.SingleLineAccessors.Count == 1
                && accessorsAnalyzeResult.MultiLineAccessors.Count == 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, accessorsAnalyzeResult.SingleLineAccessors.Single().Keyword.GetLocation()));
            }
        }

        private AccessorsAnalysisResult AnalyzeAccessorsLineSpans(IEnumerable<AccessorDeclarationSyntax> accessors)
        {
            var result = new AccessorsAnalysisResult();

            foreach (var accessorDeclarationSyntax in accessors)
            {
                var fileLinePositionSpan = accessorDeclarationSyntax.GetLineSpan();

                if (fileLinePositionSpan.StartLinePosition.Line == fileLinePositionSpan.EndLinePosition.Line)
                {
                    result.SingleLineAccessors.Add(accessorDeclarationSyntax);
                }
                else
                {
                    result.MultiLineAccessors.Add(accessorDeclarationSyntax);
                }
            }

            return result;
        }

        private class AccessorsAnalysisResult
        {
            public List<AccessorDeclarationSyntax> MultiLineAccessors { get; } = new List<AccessorDeclarationSyntax>();

            public List<AccessorDeclarationSyntax> SingleLineAccessors { get; } = new List<AccessorDeclarationSyntax>();
        }
    }
}
