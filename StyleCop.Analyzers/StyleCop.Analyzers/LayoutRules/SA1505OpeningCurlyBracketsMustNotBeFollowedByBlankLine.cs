namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An opening curly bracket within a C# element, statement, or expression is followed by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when an opening curly bracket is followed by a blank line. For
    /// example:</para>
    ///
    /// <code>
    /// public bool Enabled
    /// {
    ///
    ///     get
    ///     {
    ///
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate two instances of this violation, since there are two places where opening
    /// curly brackets are followed by blank lines.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1505OpeningCurlyBracketsMustNotBeFollowedByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1505OpeningCurlyBracketsMustNotBeFollowedByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1505";
        private const string Title = "Opening curly brackets must not be followed by blank line";
        private const string MessageFormat = "An opening curly bracket must not be followed by a blank line.";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "An opening curly bracket within a C# element, statement, or expression is followed by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1505.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleBlock, SyntaxKind.Block);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleInitializers, SyntaxKind.ObjectInitializerExpression, SyntaxKind.CollectionInitializerExpression, SyntaxKind.ArrayInitializerExpression, SyntaxKind.ComplexElementInitializerExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAnonymousObjectCreation, SyntaxKind.AnonymousObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleSwitchStatement, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAccessorList, SyntaxKind.AccessorList);
        }

        private void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;
            this.AnalyzeOpenBrace(context, block.OpenBraceToken);
        }

        private void HandleInitializers(SyntaxNodeAnalysisContext context)
        {
            var expression = (InitializerExpressionSyntax)context.Node;
            this.AnalyzeOpenBrace(context, expression.OpenBraceToken);
        }

        private void HandleAnonymousObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var expression = (AnonymousObjectCreationExpressionSyntax)context.Node;
            this.AnalyzeOpenBrace(context, expression.OpenBraceToken);
        }

        private void HandleSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;
            this.AnalyzeOpenBrace(context, switchStatement.OpenBraceToken);
        }

        private void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
            this.AnalyzeOpenBrace(context, namespaceDeclaration.OpenBraceToken);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (BaseTypeDeclarationSyntax)context.Node;
            this.AnalyzeOpenBrace(context, typeDeclaration.OpenBraceToken);
        }

        private void HandleAccessorList(SyntaxNodeAnalysisContext context)
        {
            var accessorList = (AccessorListSyntax)context.Node;
            this.AnalyzeOpenBrace(context, accessorList.OpenBraceToken);
        }

        private void AnalyzeOpenBrace(SyntaxNodeAnalysisContext context, SyntaxToken openBraceToken)
        {
            var nextToken = openBraceToken.GetNextToken();
            if ((GetLine(nextToken) - GetLine(openBraceToken)) < 2)
            {
                // there will be no blank lines when the opening brace and the following token are not at least two lines apart.
                return;
            }

            var separatingTrivia = openBraceToken.TrailingTrivia.AddRange(nextToken.LeadingTrivia);

            // skip everything until the first end of line (as this is considered part of the line of the opening brace)
            var startIndex = separatingTrivia.IndexOf(SyntaxKind.EndOfLineTrivia) + 1;

            var done = false;
            var eolCount = 0;
            for (var i = startIndex; !done && (i < separatingTrivia.Count); i++)
            {
                switch (separatingTrivia[i].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    break;
                default:
                    done = true;
                    break;
                }
            }

            if (eolCount > 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, openBraceToken.GetLocation()));
            }
        }

        private static int GetLine(SyntaxToken token)
        {
            return token.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
    }
}
