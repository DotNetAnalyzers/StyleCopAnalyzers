// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Editing;

namespace MetaCompilation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MetaCompilationCodeFixProvider)), Shared]
    public class MetaCompilationCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                //TODO: add any new rules
                return ImmutableArray.Create(MetaCompilationAnalyzer.MissingId, MetaCompilationAnalyzer.MissingInit, MetaCompilationAnalyzer.MissingRegisterStatement);
            }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;
                
                //TODO: if statements for each diagnostic id, to register a code fix
                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingId))
                {
                    ClassDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Each diagnostic must have a unique id identifying it from other diagnostics",
                        c => MissingIdAsync(context.Document, declaration, c)), diagnostic);

                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingInit))
                {
                    ClassDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Each analyzer must have an Initialize method to register actions to be performed when changes occur", c => MissingInitAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingRegisterStatement))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("The Initialize method must register an action to be performed when changes occur", c => MissingRegisterAsync(context.Document, declaration, c)), diagnostic);
                }
            }
        }

        private async Task<Document> MissingInitAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var type = SyntaxFactory.ParseTypeName("AnalysisContext");
            List<SyntaxNode> parameter = new List<SyntaxNode>();
            parameter.Add(generator.ParameterDeclaration("context", type));
            var voidDeclaration = SyntaxFactory.PredefinedType(SyntaxFactory.ParseToken("void"));
            var exceptionStatement = SyntaxFactory.ParseExpression("new NotImplementedException()");
            List<SyntaxNode> throwStatement = new List<SyntaxNode>();
            throwStatement.Add(generator.ThrowStatement(exceptionStatement));
            var initializeDeclaration = generator.MethodDeclaration("Initialize", parameters: parameter, returnType: voidDeclaration,
                accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, statements: throwStatement);

            var newClassDeclaration = generator.AddMembers(declaration, initializeDeclaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newClassDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private async Task<Document> MissingIdAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            var idToken = SyntaxFactory.ParseToken("spacingRuleId");
            
            var expressionKind = SyntaxFactory.ParseExpression("\"IfSpacing\"") as ExpressionSyntax;
            
            var equalsValueClause = SyntaxFactory.EqualsValueClause(expressionKind);
            var idDeclarator = SyntaxFactory.VariableDeclarator(idToken, null, equalsValueClause);

            var type = SyntaxFactory.ParseTypeName("string");

            var idDeclaratorList = new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add(idDeclarator);
            var idDeclaration = SyntaxFactory.VariableDeclaration(type, idDeclaratorList);

            var whiteSpace = SyntaxFactory.Whitespace("");
            var publicModifier = SyntaxFactory.ParseToken("public").WithLeadingTrivia(whiteSpace).WithTrailingTrivia(whiteSpace);
            var constModifier = SyntaxFactory.ParseToken("const").WithLeadingTrivia(whiteSpace).WithTrailingTrivia(whiteSpace);
            var modifierList = SyntaxFactory.TokenList(publicModifier, constModifier);

            var attributeList = new SyntaxList<AttributeListSyntax>();
            var fieldDeclaration = SyntaxFactory.FieldDeclaration(attributeList, modifierList, idDeclaration);
            var memberList = new SyntaxList<MemberDeclarationSyntax>().Add(fieldDeclaration);

            var newClassDeclaration = declaration.WithMembers(memberList);
            foreach (MemberDeclarationSyntax member in declaration.Members)
            {
                newClassDeclaration = newClassDeclaration.AddMembers(member);
            }

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newClassDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private async Task<Document> MissingRegisterAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            var registerExpression = SyntaxFactory.ExpressionStatement(SyntaxFactory.ParseExpression("context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement)"));

            var newInitBlock = SyntaxFactory.Block(registerExpression);
            var newInitDeclaration = declaration.WithBody(newInitBlock);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newInitDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}