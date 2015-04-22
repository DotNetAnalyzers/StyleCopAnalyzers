namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A call to an instance member of the local class or a base class is not prefixed with ‘this.’, within a C# code
    /// file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a call to an instance member of the local class
    /// or a base class which is not prefixed with <c>this.</c>. An exception to this rule occurs when there is a local
    /// override of a base class member, and the code intends to call the base class member directly, bypassing the
    /// local override. In this case the call can be prefixed with <c>base.</c> rather than <c>this.</c>.</para>
    ///
    /// <para>By default, StyleCop disallows the use of underscores or <c>m_</c> to mark local class fields, in favor of
    /// the <c>this.</c> prefix. The advantage of using <c>this.</c> is that it applies equally to all element types
    /// including methods, properties, etc., and not just fields, making all calls to class members instantly
    /// recognizable, regardless of which editor is being used to view the code. Another advantage is that it creates a
    /// quick, recognizable differentiation between instance members and static members, which are not prefixed.</para>
    ///
    /// <para>A final advantage of using the <c>this.</c> prefix is that typing <c>this.</c> will cause Visual Studio to
    /// show the IntelliSense pop-up, making it quick and easy for the developer to choose the class member to
    /// call.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1101PrefixLocalCallsWithThis : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1101PrefixLocalCallsWithThis"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1101";
        private const string Title = "Prefix local calls with this";
        private const string MessageFormat = "Prefix local calls with this";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "A call to an instance member of the local class or a base class is not prefixed with 'this.', within a C# code file.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1101.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleIdentifierName, SyntaxKind.IdentifierName);
        }

        /// <summary>
        /// <see cref="SyntaxKind.SimpleMemberAccessExpression"/> is handled separately so only <c>X</c> is evaluated in
        /// the expression <c>X.Y.Z.A.B.C</c>.
        /// </summary>
        private void HandleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            MemberAccessExpressionSyntax syntax = (MemberAccessExpressionSyntax)context.Node;
            IdentifierNameSyntax nameExpression = syntax.Expression as IdentifierNameSyntax;
            this.HandleIdentifierNameImpl(context, nameExpression);
        }

        private void HandleIdentifierName(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node?.Parent?.Kind() ?? SyntaxKind.None)
            {
            case SyntaxKind.SimpleMemberAccessExpression:
                // this is handled separately
                return;

            case SyntaxKind.PointerMemberAccessExpression:
                // this doesn't need to be handled
                return;

            case SyntaxKind.QualifiedCref:
            case SyntaxKind.NameMemberCref:
                // documentation comments don't use 'this.'
                return;

            case SyntaxKind.SimpleAssignmentExpression:
                if (((AssignmentExpressionSyntax)context.Node.Parent).Left == context.Node
                    && (context.Node.Parent.Parent?.IsKind(SyntaxKind.ObjectInitializerExpression) ?? true))
                {
                    // Handle 'X' in:
                    //   new TypeName() { X = 3 }
                    return;
                }

                break;

            case SyntaxKind.NameEquals:
                if (((NameEqualsSyntax)context.Node.Parent).Name != context.Node)
                {
                    break;
                }

                switch (context.Node?.Parent?.Parent?.Kind() ?? SyntaxKind.None)
                {
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.AnonymousObjectMemberDeclarator:
                    return;

                default:
                    break;
                }

                break;

            case SyntaxKind.MemberBindingExpression:
                // this doesn't need to be handled
                return;

            default:
                break;
            }

            this.HandleIdentifierNameImpl(context, (IdentifierNameSyntax)context.Node);
        }

        private void HandleIdentifierNameImpl(SyntaxNodeAnalysisContext context, IdentifierNameSyntax nameExpression)
        {
            if (nameExpression == null)
            {
                return;
            }

            if (!this.HasThis(nameExpression))
            {
                return;
            }

            SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(nameExpression, context.CancellationToken);
            ImmutableArray<ISymbol> symbolsToAnalyze;
            if (symbolInfo.Symbol != null)
            {
                symbolsToAnalyze = ImmutableArray.Create(symbolInfo.Symbol);
            }
            else if (symbolInfo.CandidateReason == CandidateReason.MemberGroup)
            {
                // analyze the complete set of candidates, and use 'this.' if it applies to all
                symbolsToAnalyze = symbolInfo.CandidateSymbols;
            }
            else
            {
                return;
            }

            foreach (ISymbol symbol in symbolsToAnalyze)
            {
                if (symbol is ITypeSymbol)
                {
                    return;
                }

                if (symbol.IsStatic)
                {
                    return;
                }

                if (!(symbol.ContainingSymbol is ITypeSymbol))
                {
                    // covers local variables, parameters, etc.
                    return;
                }

                IMethodSymbol methodSymbol = symbol as IMethodSymbol;
                if (methodSymbol != null && methodSymbol.MethodKind == MethodKind.Constructor)
                {
                    return;
                }
            }

            // Prefix local calls with this
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, nameExpression.GetLocation()));
        }

        private bool HasThis(SyntaxNode node)
        {
            for (; node != null; node = node.Parent)
            {
                switch (node.Kind())
                {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                    return false;

                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return false;

                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    return false;

                case SyntaxKind.EventDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                    BasePropertyDeclarationSyntax basePropertySyntax = (BasePropertyDeclarationSyntax)node;
                    return !basePropertySyntax.Modifiers.Any(SyntaxKind.StaticKeyword);

                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.MethodDeclaration:
                    BaseMethodDeclarationSyntax baseMethodSyntax = (BaseMethodDeclarationSyntax)node;
                    return !baseMethodSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);

                case SyntaxKind.Attribute:
                    return false;

                default:
                    continue;
                }
            }

            return false;
        }
    }
}
