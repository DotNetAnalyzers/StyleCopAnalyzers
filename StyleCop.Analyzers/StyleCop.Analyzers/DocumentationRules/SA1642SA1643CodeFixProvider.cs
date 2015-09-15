// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using SpacingRules;

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
    public class SA1642SA1643CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId, SA1643DestructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

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
                if (!FixableDiagnostics.Contains(diagnostic.Id))
                {
                    continue;
                }

                var node = root.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true) as XmlElementSyntax;
                if (node == null)
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(DocumentationResources.SA1642SA1643CodeFix, token => GetTransformedDocumentAsync(context.Document, root, node), equivalenceKey: nameof(SA1642SA1643CodeFixProvider)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, XmlElementSyntax node)
        {
            var typeDeclaration = node.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();
            var declarationSyntax = node.FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();
            bool isStruct = typeDeclaration.IsKind(SyntaxKind.StructDeclaration);

            TypeParameterListSyntax typeParameterList;
            ClassDeclarationSyntax classDeclaration = typeDeclaration as ClassDeclarationSyntax;
            if (classDeclaration != null)
            {
                typeParameterList = classDeclaration.TypeParameterList;
            }
            else
            {
                typeParameterList = (typeDeclaration as StructDeclarationSyntax)?.TypeParameterList;
            }

            ImmutableArray<string> standardText;
            if (declarationSyntax is ConstructorDeclarationSyntax)
            {
                if (declarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    if (isStruct)
                    {
                        standardText = ImmutableArray.Create(SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.StaticConstructorStandardText, " struct.");
                    }
                    else
                    {
                        standardText = ImmutableArray.Create(SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.StaticConstructorStandardText, " class.");
                    }
                }
                else
                {
                    // Prefer to insert the "non-private" wording for all constructors, even though both are considered
                    // acceptable for private constructors by the diagnostic.
                    // https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/413
                    if (isStruct)
                    {
                        standardText = ImmutableArray.Create(SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.NonPrivateConstructorStandardText, " struct.");
                    }
                    else
                    {
                        standardText = ImmutableArray.Create(SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.NonPrivateConstructorStandardText, " class.");
                    }
                }
            }
            else if (declarationSyntax is DestructorDeclarationSyntax)
            {
                standardText = SA1643DestructorSummaryDocumentationMustBeginWithStandardText.DestructorStandardText;
            }
            else
            {
                throw new InvalidOperationException("XmlElementSyntax has invalid method as its parent");
            }

            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
            var list = BuildStandardText(typeDeclaration.Identifier, typeParameterList, newLineText, standardText[0], standardText[1]);

            var newContent = node.Content.InsertRange(0, list);
            var newNode = node.WithContent(newContent).AdjustDocumentationCommentNewLineTrivia();

            var newRoot = root.ReplaceNode(node, newNode);

            var newDocument = document.WithSyntaxRoot(newRoot);

            return Task.FromResult(newDocument);
        }

        private static SyntaxList<XmlNodeSyntax> BuildStandardText(SyntaxToken identifier, TypeParameterListSyntax typeParameters, string newLineText, string preText, string postText)
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

            return XmlSyntaxFactory.List(
                XmlSyntaxFactory.NewLine(newLineText),
                XmlSyntaxFactory.Text(preText),
                XmlSyntaxFactory.SeeElement(SyntaxFactory.TypeCref(identifierName)),
                XmlSyntaxFactory.Text(postText.EndsWith(".") ? postText : (postText + ".")));
        }

        private static TypeArgumentListSyntax ParameterToArgumentListSyntax(TypeParameterListSyntax typeParameters)
        {
            var list = new SeparatedSyntaxList<TypeSyntax>();
            list = list.AddRange(typeParameters.Parameters.Select(p => SyntaxFactory.ParseName(p.ToString()).WithTriviaFrom(p)));

            for (int i = 0; i < list.SeparatorCount; i++)
            {
                // Make sure the parameter list looks nice
                var separator = list.GetSeparator(i);
                list = list.ReplaceSeparator(separator, separator.WithTrailingTrivia(SyntaxFactory.Space));
            }

            return SyntaxFactory.TypeArgumentList(list);
        }
    }
}
