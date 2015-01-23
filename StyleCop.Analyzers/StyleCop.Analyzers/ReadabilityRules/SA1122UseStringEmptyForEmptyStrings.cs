namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;





    /// <summary>
    /// The C# code includes an empty string, written as <c>""</c>.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains an empty string. For example:</para>
    ///
    /// <code language="csharp">
    /// string s = "";
    /// </code>
    ///
    /// <para>This will cause the compiler to embed an empty string into the compiled code. Rather than including a
    /// hard-coded empty string, use the static <see cref="string.Empty"/> field:</para>
    ///
    /// <code language="csharp">
    /// string s = string.Empty;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1122UseStringEmptyForEmptyStrings : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1122";
        internal const string Title = "Use string.Empty for empty strings";
        internal const string MessageFormat = "Use string.Empty for empty strings";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "The C# code includes an empty string, written as \"\".";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1122.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

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
            context.RegisterSyntaxNodeAction(HandleStringLiteral, SyntaxKind.StringLiteralExpression);
        }

        private void HandleStringLiteral(SyntaxNodeAnalysisContext context)
        {
            LiteralExpressionSyntax literalExpression = context.Node as LiteralExpressionSyntax;

            if (literalExpression != null)
            {
                var token = literalExpression.Token;
                if (token.IsKind(SyntaxKind.StringLiteralToken))
                {
                    if (HasToBeConstant(literalExpression))
                        return;

                    if (token.ValueText == string.Empty)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, literalExpression.GetLocation()));
                    }
                }
            }
        }

        private bool HasToBeConstant(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression.Parent.IsKind(SyntaxKind.AttributeArgument))
                return true;
            var fieldDeclarationSyntax = FindFieldDeclarationSyntax(literalExpression);
            return fieldDeclarationSyntax != null && fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ConstKeyword);
        }

        private FieldDeclarationSyntax FindFieldDeclarationSyntax(SyntaxNode node)
        {
            if (node == null)
                return null;
            if (node is FieldDeclarationSyntax)
                return node as FieldDeclarationSyntax;
            return FindFieldDeclarationSyntax(node.Parent);
        }
    }
}
