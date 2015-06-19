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
                return ImmutableArray.Create(MetaCompilationAnalyzer.MissingId, 
                    MetaCompilationAnalyzer.MissingInit, 
                    MetaCompilationAnalyzer.MissingRegisterStatement,
                    MetaCompilationAnalyzer.TooManyInitStatements,
                    MetaCompilationAnalyzer.InvalidStatement,
                    MetaCompilationAnalyzer.IfStatementIncorrect);
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
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Each diagnostic must have a unique id identifying it from other diagnostics",
                        c => MissingIdAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingInit))
                {
                    ClassDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Each analyzer must have an Initialize method to register actions to be performed when changes occur", c => MissingInitAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingRegisterStatement))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The Initialize method must register an action to be performed when changes occur", c => MissingRegisterAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TooManyInitStatements))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The Initialize method must not contain multiple actions to register (for the purpose of this tutorial)", c => MultipleStatementsAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.InvalidStatement))
                {
                    StatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("The Initialize method can only register actions, all other statements are invalid", c => InvalidStatementAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfStatementIncorrect))
                {
                    StatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("The first statement of the analyzer must access the node to be analyzed", c => IncorrectIfAsync(context.Document, declaration, c)), diagnostic);
                }
            }
        }

        private async Task<Document> MissingInitAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var type = SyntaxFactory.ParseTypeName("AnalysisContext");
            var parameters = new[] { generator.ParameterDeclaration("context", type) };
            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            var statements = new[] { generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)) };
            var initializeDeclaration = generator.MethodDeclaration("Initialize", parameters: parameters,
                accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, statements: statements);

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

        private async Task<Document> MultipleStatementsAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            var statements = declaration.Body.Statements;

            var newBlock = declaration.Body;

            foreach (ExpressionStatementSyntax statement in statements)
            {
                var expression = statement.Expression as InvocationExpressionSyntax;
                var expressionStart = expression.Expression as MemberAccessExpressionSyntax;
                if (expressionStart == null || expressionStart.Name == null ||
                    expressionStart.Name.ToString() != "RegisterSyntaxNodeAction")
                {
                    statements = statements.Remove(statement);
                }

                if (expression.ArgumentList == null || expression.ArgumentList.Arguments.Count() != 2)
                {
                    statements = statements.Remove(statement);
                }
                var argumentMethod = expression.ArgumentList.Arguments[0].Expression as IdentifierNameSyntax;
                var argumentKind = expression.ArgumentList.Arguments[1].Expression as MemberAccessExpressionSyntax;
                var preArgumentKind = argumentKind.Expression as IdentifierNameSyntax;
                if (argumentMethod.Identifier == null || argumentKind.Name == null || preArgumentKind.Identifier == null ||
                    argumentMethod.Identifier.ValueText != "AnalyzeIfStatement" || argumentKind.Name.ToString() != "IfStatement" ||
                    preArgumentKind.Identifier.ValueText != "SyntaxKind")
                {
                    statements = statements.Remove(statement);
                }
            }

            newBlock = newBlock.WithStatements(statements);
            var newDeclaration = declaration.WithBody(newBlock);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private async Task<Document> InvalidStatementAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            BlockSyntax initializeCodeBlock = declaration.Parent as BlockSyntax;
            MethodDeclarationSyntax initializeDeclaration = initializeCodeBlock.Parent as MethodDeclarationSyntax;

            BlockSyntax newCodeBlock = initializeCodeBlock.WithStatements(initializeCodeBlock.Statements.Remove(declaration));
            MethodDeclarationSyntax newInitializeDeclaration = initializeDeclaration.WithBody(newCodeBlock);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(initializeDeclaration, newInitializeDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private async Task<Document> IncorrectIfAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var type = SyntaxFactory.ParseTypeName("IfStatementSyntax");
            var expression = generator.IdentifierName("context");
            var memberAccessExpression = generator.MemberAccessExpression(expression, "Node");
            var initializer = generator.CastExpression(type, memberAccessExpression);
            var ifStatement = generator.LocalDeclarationStatement("ifStatement", initializer);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, ifStatement);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}