// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/>
    /// and <see cref="SA1643DestructorSummaryDocumentationMustBeginWithStandardText"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add the standard documentation text.
    /// above.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1642SA1643CodeFixProvider))]
    [Shared]
    internal class SA1642SA1643CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId,
                SA1643DestructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Properties.ContainsKey(StandardTextDiagnosticBase.NoCodeFixKey))
                {
                    continue;
                }

                var node = root.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);

                if (node is XmlElementSyntax xmlElementSyntax)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.SA1642SA1643CodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, root, xmlElementSyntax, cancellationToken),
                            nameof(SA1642SA1643CodeFixProvider)),
                        diagnostic);
                }
                else
                {
                    var xmlEmptyElementSyntax = (XmlEmptyElementSyntax)node;
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.SA1642SA1643CodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, root, xmlEmptyElementSyntax),
                            nameof(SA1642SA1643CodeFixProvider)),
                        diagnostic);
                }
            }
        }

        internal static ImmutableArray<string> GenerateStandardText(Document document, BaseMethodDeclarationSyntax methodDeclaration, BaseTypeDeclarationSyntax typeDeclaration, CancellationToken cancellationToken)
        {
            bool isStruct = typeDeclaration.IsKind(SyntaxKind.StructDeclaration);
            var settings = document.Project.AnalyzerOptions.GetStyleCopSettings(methodDeclaration.SyntaxTree, cancellationToken);
            var culture = new CultureInfo(settings.DocumentationRules.DocumentationCulture);
            var resourceManager = DocumentationResources.ResourceManager;

            if (methodDeclaration is ConstructorDeclarationSyntax)
            {
                var typeKindText = resourceManager.GetString(isStruct ? nameof(DocumentationResources.TypeTextStruct) : nameof(DocumentationResources.TypeTextClass), culture);
                if (methodDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    return ImmutableArray.Create(
                        string.Format(resourceManager.GetString(nameof(DocumentationResources.StaticConstructorStandardTextFirstPart), culture), typeKindText),
                        string.Format(resourceManager.GetString(nameof(DocumentationResources.StaticConstructorStandardTextSecondPart), culture), typeKindText));
                }
                else
                {
                    // Prefer to insert the "non-private" wording for all constructors, even though both are considered
                    // acceptable for private constructors by the diagnostic.
                    // https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/413
                    return ImmutableArray.Create(
                        string.Format(resourceManager.GetString(nameof(DocumentationResources.NonPrivateConstructorStandardTextFirstPart), culture), typeKindText),
                        string.Format(resourceManager.GetString(nameof(DocumentationResources.NonPrivateConstructorStandardTextSecondPart), culture), typeKindText));
                }
            }
            else if (methodDeclaration is DestructorDeclarationSyntax)
            {
                return ImmutableArray.Create(
                    resourceManager.GetString(nameof(DocumentationResources.DestructorStandardTextFirstPart), culture),
                    resourceManager.GetString(nameof(DocumentationResources.DestructorStandardTextSecondPart), culture));
            }
            else
            {
                throw new InvalidOperationException("XmlElementSyntax has invalid method as its parent");
            }
        }

        internal static SyntaxList<XmlNodeSyntax> BuildStandardTextSyntaxList(BaseTypeDeclarationSyntax typeDeclaration, string preText, string postText)
        {
            TypeParameterListSyntax typeParameterList = GetTypeParameterList(typeDeclaration);

            return XmlSyntaxFactory.List(
                XmlSyntaxFactory.Text(preText),
                BuildSeeElement(typeDeclaration.Identifier, typeParameterList),
                XmlSyntaxFactory.Text(postText.EndsWith(".") ? postText : (postText + ".")));
        }

        internal static SyntaxList<XmlNodeSyntax> BuildStandardTextSyntaxList(BaseTypeDeclarationSyntax typeDeclaration, string newLineText, string preText, string postText)
        {
            TypeParameterListSyntax typeParameterList = GetTypeParameterList(typeDeclaration);

            return XmlSyntaxFactory.List(
                XmlSyntaxFactory.NewLine(newLineText),
                XmlSyntaxFactory.Text(preText),
                BuildSeeElement(typeDeclaration.Identifier, typeParameterList),
                XmlSyntaxFactory.Text(postText.EndsWith(".") ? postText : (postText + ".")));
        }

        private static TypeParameterListSyntax GetTypeParameterList(BaseTypeDeclarationSyntax typeDeclaration)
        {
            if (typeDeclaration is ClassDeclarationSyntax classDeclaration)
            {
                return classDeclaration.TypeParameterList;
            }

            return (typeDeclaration as StructDeclarationSyntax)?.TypeParameterList;
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, XmlElementSyntax node, CancellationToken cancellationToken)
        {
            var typeDeclaration = node.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();
            var declarationSyntax = node.FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();

            var standardText = GenerateStandardText(document, declarationSyntax, typeDeclaration, cancellationToken);

            string trailingString = string.Empty;

            var newContent = RemoveMalformattedStandardText(node.Content, standardText[0], standardText[1], ref trailingString);

            if (newContent.Count == 1 && newContent[0] is XmlTextSyntax xmlText)
            {
                if (string.IsNullOrWhiteSpace(xmlText.ToString()))
                {
                    newContent = default;
                }
            }

            SyntaxList<XmlNodeSyntax> list;
            if (IsMultiLine(node))
            {
                string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
                list = BuildStandardTextSyntaxList(typeDeclaration, newLineText, standardText[0], standardText[1] + trailingString);
            }
            else
            {
                list = BuildStandardTextSyntaxList(typeDeclaration, standardText[0], standardText[1] + trailingString);
            }

            newContent = newContent.InsertRange(0, list);

            newContent = RemoveTrailingEmptyLines(newContent);

            var newNode = node.WithContent(newContent).AdjustDocumentationCommentNewLineTrivia();

            var newRoot = root.ReplaceNode(node, newNode);

            var newDocument = document.WithSyntaxRoot(newRoot);

            return Task.FromResult(newDocument);
        }

        private static bool IsMultiLine(XmlElementSyntax node)
        {
            var lineSpan = node.GetLineSpan();
            return lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line;
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, XmlEmptyElementSyntax node)
        {
            var typeDeclaration = node.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();

            TypeParameterListSyntax typeParameterList;
            if (typeDeclaration is ClassDeclarationSyntax classDeclaration)
            {
                typeParameterList = classDeclaration.TypeParameterList;
            }
            else
            {
                typeParameterList = (typeDeclaration as StructDeclarationSyntax)?.TypeParameterList;
            }

            var newRoot = root.ReplaceNode(node, BuildSeeElement(typeDeclaration.Identifier, typeParameterList));

            var newDocument = document.WithSyntaxRoot(newRoot);

            return Task.FromResult(newDocument);
        }

        private static SyntaxList<XmlNodeSyntax> RemoveMalformattedStandardText(SyntaxList<XmlNodeSyntax> content, string preText, string postText, ref string trailingString)
        {
            var regex = new Regex(@"^\s*" + Regex.Escape(preText) + "[^ ]+" + Regex.Escape(postText));
            var item = content.OfType<XmlTextSyntax>().FirstOrDefault();

            if (item == null)
            {
                return content;
            }

            int index = -1;
            foreach (var token in item.TextTokens)
            {
                index++;

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }
                else if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                {
                    string value = token.ValueText.Trim(null);

                    Match match = regex.Match(value);

                    if (!match.Success)
                    {
                        return content;
                    }
                    else if (match.Length == value.Length)
                    {
                        // Remove the token
                        var tokens = item.TextTokens;

                        while (index >= 0)
                        {
                            tokens = tokens.RemoveAt(0);
                            index--;
                        }

                        var newContent = item.WithTextTokens(tokens);

                        return content.Replace(item, newContent);
                    }
                    else
                    {
                        // Remove the tokens before
                        var tokens = item.TextTokens;

                        while (index >= 0)
                        {
                            tokens = tokens.RemoveAt(0);
                            index--;
                        }

                        trailingString = value.Substring(match.Length);

                        var newContent = item.WithTextTokens(tokens);

                        return content.Replace(item, newContent);
                    }
                }
                else
                {
                    return content;
                }
            }

            return content;
        }

        private static XmlEmptyElementSyntax BuildSeeElement(SyntaxToken identifier, TypeParameterListSyntax typeParameters)
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

            return XmlSyntaxFactory.SeeElement(SyntaxFactory.TypeCref(identifierName));
        }

        private static TypeArgumentListSyntax ParameterToArgumentListSyntax(TypeParameterListSyntax typeParameters)
        {
            var list = SyntaxFactory.SeparatedList<TypeSyntax>();
            list = list.AddRange(typeParameters.Parameters.Select(p => SyntaxFactory.ParseName(p.ToString()).WithTriviaFrom(p)));

            for (int i = 0; i < list.SeparatorCount; i++)
            {
                // Make sure the parameter list looks nice
                var separator = list.GetSeparator(i);
                list = list.ReplaceSeparator(separator, separator.WithTrailingTrivia(SyntaxFactory.Space));
            }

            return SyntaxFactory.TypeArgumentList(list);
        }

        private static SyntaxList<XmlNodeSyntax> RemoveTrailingEmptyLines(SyntaxList<XmlNodeSyntax> content)
        {
            if (!(content[content.Count - 1] is XmlTextSyntax xmlText))
            {
                return content;
            }

            // skip the last token, as it contains the documentation comment for the closing tag, which needs to remain.
            var firstEmptyToken = -1;
            for (var j = xmlText.TextTokens.Count - 2; j >= 0; j--)
            {
                var textToken = xmlText.TextTokens[j];

                if (textToken.IsXmlWhitespace())
                {
                    firstEmptyToken = j;
                }
                else if (textToken.IsXmlNewLine() && textToken.LeadingTrivia.Any(SyntaxKind.DocumentationCommentExteriorTrivia))
                {
                    // Skip completely blank lines
                    firstEmptyToken = j;
                }
                else if (textToken.IsKind(SyntaxKind.XmlTextLiteralToken) && !string.IsNullOrWhiteSpace(textToken.Text))
                {
                    break;
                }
            }

            if (firstEmptyToken > -1)
            {
                var newContent = content.RemoveAt(content.Count - 1);
                newContent = newContent
                    .Add(XmlSyntaxFactory.Text(xmlText.TextTokens.Take(firstEmptyToken).ToArray()))
                    .Add(XmlSyntaxFactory.Text(xmlText.TextTokens.Last()));
                return newContent;
            }

            return content;
        }
    }
}
