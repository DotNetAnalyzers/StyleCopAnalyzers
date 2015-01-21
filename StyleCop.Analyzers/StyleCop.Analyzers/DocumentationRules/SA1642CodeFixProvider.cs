namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Helpers;
    using SpacingRules;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// Implements a code fix for <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add the standard documentation text.
    /// above.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1642ConstructorSummaryDocumentationMustBeginWithStandardText), LanguageNames.CSharp)]
    [Shared]
    public class SA1642CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return _fixableDiagnostics;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                var node = root.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true) as XmlElementSyntax;
                if (node == null)
                    continue;
                var classDeclaration = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                var constructorDeclarationSyntax = node.FirstAncestorOrSelf<ConstructorDeclarationSyntax>();

                string[] standardText;

                if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    standardText = SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.StaticConstructorStandardText;
                }
                else if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    standardText = SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.PrivateConstructorStandardText;
                }
                else
                {
                    standardText = SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.NonPrivateConstructorStandardText;
                }

                var list = this.BuildStandardText(classDeclaration.Identifier, classDeclaration.TypeParameterList, standardText[0], standardText[1]);

                var newContent = node.Content.InsertRange(0, list);
                var newNode = node.WithContent(newContent);

                var newRoot = root.ReplaceNode(node, newNode);

                var newDocument = context.Document.WithSyntaxRoot(newRoot);
                context.RegisterFix(CodeAction.Create("Add standard text.", newDocument), diagnostic);
            }
        }

        private SyntaxList<XmlNodeSyntax> BuildStandardText(SyntaxToken identifier, TypeParameterListSyntax typeParameters, string preText, string postText)
        {
            TypeSyntax identifierName;

            // Get a TypeSyntax representing the class name with its type parameters
            if (typeParameters == null || !typeParameters.Parameters.Any())
            {
                identifierName = SyntaxFactory.IdentifierName(identifier.WithoutTrivia());
            }
            else
            {
                identifierName = SyntaxFactory.GenericName(identifier.WithoutTrivia(), ParameterToArgumentListSyntax(typeParameters));
            }
            var list = new SyntaxList<XmlNodeSyntax>();

            list = list.Add(XmlNewLine());
            list = list.Add(this.CreateTextSyntax(preText).WithLeadingTrivia(XmlLineStart()));
            list = list.Add(CreateSeeSyntax(identifierName));
            list = list.Add(CreateTextSyntax(postText));

            return list;
        }

        private static SyntaxTriviaList XmlLineStart()
        {
            return SyntaxFactory.TriviaList(
                SyntaxFactory.ElasticMarker,
                SyntaxFactory.DocumentationCommentExterior("/// "));
        }

        private static XmlTextSyntax XmlNewLine()
        {
            var tokenList = new SyntaxTokenList();
            tokenList = tokenList.Add(SyntaxFactory.XmlTextNewLine(default(SyntaxTriviaList), "\r\n", "\r\n", default(SyntaxTriviaList)));

            return SyntaxFactory.XmlText(tokenList);
        }

        private TypeArgumentListSyntax ParameterToArgumentListSyntax(TypeParameterListSyntax typeParameters)
        {
            var list = new SeparatedSyntaxList<TypeSyntax>();
            list = list.AddRange(typeParameters.Parameters.Select(p => SyntaxFactory.ParseName(p.ToString()).WithTriviaFrom(p)));

            for (int i = 0; i < list.SeparatorCount; i++)
            {
                var separator = list.GetSeparator(i);
                // Make sure the parameter list looks nice
                list = list.ReplaceSeparator(separator, separator.WithTrailingTrivia(SyntaxFactory.Whitespace(" ")));
            }

            return SyntaxFactory.TypeArgumentList(list);
        }

        private XmlNodeSyntax CreateSeeSyntax(TypeSyntax identifier)
        {
            NameMemberCrefSyntax cref;

            var genericName = identifier as GenericNameSyntax;

            if (genericName != null)
            {
                // In Xml a type argument list is enclosed in braces, not greater/less than tokens.
                var lessThanToken = SyntaxFactory.Token(default(SyntaxTriviaList), SyntaxKind.LessThanToken, "{", "{", default(SyntaxTriviaList));
                var greaterThanToken = SyntaxFactory.Token(default(SyntaxTriviaList), SyntaxKind.GreaterThanToken, "}", "}", default(SyntaxTriviaList));
                var newList = SyntaxFactory.TypeArgumentList(lessThanToken, genericName.TypeArgumentList.Arguments, greaterThanToken);
                genericName = genericName.WithTypeArgumentList(newList);
                cref = SyntaxFactory.NameMemberCref(genericName);
            }
            else
            {
                cref = SyntaxFactory.NameMemberCref(identifier).WithoutFormatting();
            }

            var attributes = new SyntaxList<XmlAttributeSyntax>();

            attributes = attributes.Add(SyntaxFactory.XmlCrefAttribute(SyntaxFactory.XmlName(XmlCommentHelper.CrefArgumentName), SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken), cref, SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken)));

            return SyntaxFactory.XmlEmptyElement(SyntaxFactory.XmlName(XmlCommentHelper.SeeXmlTag).WithTrailingTrivia(SyntaxFactory.ElasticSpace), attributes);
        }

        private XmlTextSyntax CreateTextSyntax(string text)
        {
            var tokenList = new SyntaxTokenList();
            tokenList = tokenList.Add(XmlTextLiteral(text));

            return SyntaxFactory.XmlText(tokenList);
        }

        private static SyntaxToken XmlTextLiteral(string text)
        {
            return SyntaxFactory.XmlTextLiteral(default(SyntaxTriviaList), text, text, default(SyntaxTriviaList));
        }
    }
}
