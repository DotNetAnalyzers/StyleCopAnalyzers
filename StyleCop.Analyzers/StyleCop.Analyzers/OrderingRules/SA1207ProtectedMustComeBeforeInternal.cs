namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The keyword <c>protected</c> is positioned after the keyword <c>internal</c> within the declaration of a
    /// protected internal C# element.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a protected internal element's access modifiers are written as
    /// <c>internal protected</c>. In reality, an element with the keywords <c>protected internal</c> will have the same
    /// access level as an element with the keywords <c>internal protected</c>. To make the code easier to read and more
    /// consistent, StyleCop standardizes the ordering of these keywords, so that a protected internal element will
    /// always be described as such, and never as internal protected. This can help to reduce confusion about whether
    /// these access levels are indeed the same.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1207ProtectedMustComeBeforeInternal : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1207ProtectedMustComeBeforeInternal"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1207";
        private const string Title = "Protected must come before internal";
        private const string MessageFormat = "The keyword 'protected' must come before 'internal'.";
        private const string Description = "The keyword 'protected' is positioned after the keyword 'internal' within the declaration of a protected internal C# element.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1207.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(
                this.HandleDeclaration,
                SyntaxKind.ClassDeclaration,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.StructDeclaration);
        }

        private void HandleDeclaration(SyntaxNodeAnalysisContext context)
        {
            // TODO: Should we do null checking? It's not covered in a test, because (this version of) Roslyn never returns null.
            var childTokens = context.Node?.ChildTokens()?.ToArray();
            if (childTokens == null)
            {
                return;
            }

            var firstProtectedKeyworkIndex = Array.FindIndex(childTokens, token => token.IsKind(SyntaxKind.ProtectedKeyword));
            if (firstProtectedKeyworkIndex < 0)
            {
                return;
            }

            var firstInternalKeyworkIndex = Array.FindIndex(childTokens, token => token.IsKind(SyntaxKind.InternalKeyword));
            if (firstInternalKeyworkIndex < 0)
            {
                return;
            }

            if (firstProtectedKeyworkIndex > firstInternalKeyworkIndex)
            {
                var firstProtectedKeyword = childTokens[firstProtectedKeyworkIndex];
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstProtectedKeyword.GetLocation()));
            }
        }
    }
}
