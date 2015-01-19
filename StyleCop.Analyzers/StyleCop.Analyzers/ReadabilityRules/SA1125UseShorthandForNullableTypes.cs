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
        public const string DiagnosticId = "SA1125";
        internal const string Title = "Use shorthand for nullable types";
        internal const string MessageFormat = "Use shorthand for nullable types";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "The Nullable<T> type has been defined not using the C# shorthand. For example, Nullable<DateTime> has been used instead of the preferred DateTime?";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1125.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleGenericNameSyntax, SyntaxKind.GenericName);
        }

        private void HandleGenericNameSyntax(SyntaxNodeAnalysisContext context)
        {
            GenericNameSyntax genericNameSyntax = context.Node as GenericNameSyntax;
            if (genericNameSyntax == null)
                return;

            if (genericNameSyntax.Identifier.IsMissing || genericNameSyntax.Identifier.Text != "Nullable")
                return;

            // The shorthand syntax is not available for the unbound generic form, e.g. typeof(Nullable<>)
            if (genericNameSyntax?.TypeArgumentList?.Arguments.Count == 0)
                return;

            // This covers the specific form in an XML comment which cannot be simplified
            if (genericNameSyntax.Parent is NameMemberCrefSyntax)
                return;

            // The shorthand syntax is not available in using directives (covers standard, alias, and static)
            if (genericNameSyntax.FirstAncestorOrSelf<UsingDirectiveSyntax>() != null)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            INamedTypeSymbol symbol = semanticModel.GetSymbolInfo(genericNameSyntax, context.CancellationToken).Symbol as INamedTypeSymbol;
            if (symbol?.OriginalDefinition?.SpecialType != SpecialType.System_Nullable_T)
                return;

            SyntaxNode locationNode = genericNameSyntax;
            if (genericNameSyntax.Parent is QualifiedNameSyntax)
                locationNode = genericNameSyntax.Parent;

            // Use shorthand for nullable types
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, locationNode.GetLocation()));
        }
    }
}
