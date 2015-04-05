namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The <see cref="Nullable{T}"/> type has been defined not using the C# shorthand. For example,
    /// <c>Nullable&lt;DateTime&gt;</c> has been used instead of the preferred <c>DateTime?</c>.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the <see cref="Nullable{T}"/> type has been defined without using
    /// the shorthand C# style.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1125UseShorthandForNullableTypes : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1125UseShorthandForNullableTypes"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1125";
        private const string Title = "Use shorthand for nullable types";
        private const string MessageFormat = "Use shorthand for nullable types";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "The Nullable<T> type has been defined not using the C# shorthand. For example, Nullable<DateTime> has been used instead of the preferred DateTime?";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1125.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            context.RegisterSyntaxNodeAction(this.HandleGenericNameSyntax, SyntaxKind.GenericName);
        }

        private void HandleGenericNameSyntax(SyntaxNodeAnalysisContext context)
        {
            GenericNameSyntax genericNameSyntax = context.Node as GenericNameSyntax;
            if (genericNameSyntax == null)
            {
                return;
            }

            if (genericNameSyntax.Identifier.IsMissing || genericNameSyntax.Identifier.Text != "Nullable")
            {
                return;
            }

            // This covers the specific forms in an XML comment which cannot be simplified
            if (genericNameSyntax.Parent is NameMemberCrefSyntax)
            {
                // cref="Nullable{T}"
                return;
            }
            else if (genericNameSyntax.Parent is QualifiedCrefSyntax)
            {
                // cref="Nullable{T}.Value"
                return;
            }
            else if (genericNameSyntax.Parent is QualifiedNameSyntax && genericNameSyntax.Parent.Parent is QualifiedCrefSyntax)
            {
                // cref="System.Nullable{T}.Value"
                return;
            }

            // The shorthand syntax is not available in using directives (covers standard, alias, and static)
            if (genericNameSyntax.FirstAncestorOrSelf<UsingDirectiveSyntax>() != null)
            {
                return;
            }

            SemanticModel semanticModel = context.SemanticModel;
            INamedTypeSymbol symbol = semanticModel.GetSymbolInfo(genericNameSyntax, context.CancellationToken).Symbol as INamedTypeSymbol;
            if (symbol?.OriginalDefinition?.SpecialType != SpecialType.System_Nullable_T)
            {
                return;
            }

            if (symbol.IsUnboundGenericType)
            {
                // There is never a shorthand syntax for the open generic Nullable<>
                return;
            }

            SyntaxNode locationNode = genericNameSyntax;
            if (genericNameSyntax.Parent is QualifiedNameSyntax)
            {
                locationNode = genericNameSyntax.Parent;
            }

            // Use shorthand for nullable types
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, locationNode.GetLocation()));
        }
    }
}
