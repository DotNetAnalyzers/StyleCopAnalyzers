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
                    MetaCompilationAnalyzer.InternalAndStaticError,
                    MetaCompilationAnalyzer.EnabledByDefaultError,
                    MetaCompilationAnalyzer.DefaultSeverityError,
                    MetaCompilationAnalyzer.MissingIdDeclaration,
                    MetaCompilationAnalyzer.IdDeclTypeError,
                    MetaCompilationAnalyzer.IncorrectInitSig,
                    MetaCompilationAnalyzer.IfStatementIncorrect,
                    MetaCompilationAnalyzer.IfKeywordIncorrect,
                    MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                    MetaCompilationAnalyzer.TrailingTriviaCheckMissing,
                    MetaCompilationAnalyzer.TrailingTriviaVarMissing,
                    MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                    MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                    MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                    MetaCompilationAnalyzer.ReturnStatementIncorrect,
                    MetaCompilationAnalyzer.TooManyStatements);
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
                
                //TODO: change this to else if once we are done (creates less merge conflicts without else if)
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

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.InternalAndStaticError))
                {
                    FieldDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Rules must be declared as both internal and static.", c => InternalStaticAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.EnabledByDefaultError))
                {
                    LiteralExpressionSyntax literalExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Rules should be enabled by default.", c => EnabledByDefaultAsync(context.Document, literalExpression, c)), diagnostic);
                }

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.DefaultSeverityError))
                {
                    MemberAccessExpressionSyntax memberAccessExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("defaultSeverity should be set to \"Error\" if something is not allowed by the language authorities.", c => DiagnosticSeverityError(context.Document, memberAccessExpression, c)), diagnostic);
                    context.RegisterCodeFix(CodeAction.Create("defaultSeverity should be set to \"Warning\" if something is suspicious but allowed.", c => DiagnosticSeverityWarning(context.Document, memberAccessExpression, c)), diagnostic);
                    context.RegisterCodeFix(CodeAction.Create("defaultSeverity should be set to \"Hidden\" if something is an issue, but is not surfaced by normal means.", c => DiagnosticSeverityHidden(context.Document, memberAccessExpression, c)), diagnostic);
                    context.RegisterCodeFix(CodeAction.Create("defaultSeverity should be set to \"Info\" for information that does not indicate a problem.", c => DiagnosticSeverityInfo(context.Document, memberAccessExpression, c)), diagnostic);
                }

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.MissingIdDeclaration))
                {
                    VariableDeclaratorSyntax ruleDeclarationField = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Generate a public field for this rule id.", c => MissingIdDeclarationAsync(context.Document, ruleDeclarationField, c)), diagnostic);
                }

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.IdDeclTypeError))
                {
                    LiteralExpressionSyntax literalExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Rule ids should not be string literals.", c => IdDeclTypeAsync(context.Document, literalExpression, c)), diagnostic);
                }
           
                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfStatementIncorrect))
                {
                    StatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The first statement of the analyzer must access the node to be analyzed", c => IncorrectIfAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IncorrectInitSig))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The initialize method must have the correct signature to be called", c => IncorrectSigAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfKeywordIncorrect))
                {
                    StatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The second statement of the analyzer must access the keyword from the node being analyzed", c => IncorrectKeywordAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The third statement of the analyzer must be an if statement checking the trailing trivia of the node being analyzed", c => TrailingCheckIncorrectAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaCheckMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The third statement of the analyzer must be an if statement checking the trailing trivia of the node being analyzed", c => TrailingCheckMissingAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaVarMissing))
                {
                    IfStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The fourth statement of the analyzer should store the last trailing trivia of the if keyword", c => TrailingVarMissingAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaVarIncorrect))
                {
                    IfStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The fourth statement of the analyzer should store the last trailing trivia of the if keyword", c => TrailingVarIncorrectAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect))
                {
                    IfStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The fifth statement of the analyzer should be a check of the kind of trivia following the if keyword", c => TrailingKindCheckIncorrectAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.WhitespaceCheckIncorrect))
                {
                    IfStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The sixth statement of the analyzer should be a check to ensure the whitespace after if statement keyword is correct", c => WhitespaceCheckIncorrectAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.ReturnStatementIncorrect))
                {
                    IfStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The seventh step of the analyzer should quit the analysis (if the if statement is formatted properly)", c => ReturnIncorrectAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TooManyStatements))
                {
                    IfStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Thre are too many statments within this if block; its only purpose is to return if the statement is formatted properly", c => TooManyStatementsAsync(context.Document, declaration, c)), diagnostic);
                }
            }
        }

        #region id code fix
        private async Task<Document> MissingIdAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            var idToken = SyntaxFactory.ParseToken("spacingRuleId");
            var expressionKind = SyntaxFactory.ParseExpression("\"IfSpacing\"") as ExpressionSyntax;
            var newClassDeclaration = newIdCreator(idToken, expressionKind, declaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newClassDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private ClassDeclarationSyntax newIdCreator(SyntaxToken idToken, ExpressionSyntax expressionKind, ClassDeclarationSyntax declaration)
        {
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

            return newClassDeclaration;
        }
        #endregion

        #region initialize code fix
        private async Task<Document> MissingInitAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            var initializeDeclaration = BuildInitialize(document, semanticModel);

            var newClassDeclaration = generator.AddMembers(declaration, initializeDeclaration);

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
            SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();
            SyntaxList<StatementSyntax> initializeStatements = declaration.Body.Statements;

            var newBlock = declaration.Body;

            foreach (ExpressionStatementSyntax statement in initializeStatements)
            {
                var expression = statement.Expression as InvocationExpressionSyntax;
                var expressionStart = expression.Expression as MemberAccessExpressionSyntax;
                if (expressionStart == null || expressionStart.Name == null ||
                    expressionStart.Name.ToString() != "RegisterSyntaxNodeAction")
                {
                    continue;
                }

                if (expression.ArgumentList == null || expression.ArgumentList.Arguments.Count() != 2)
                {
                    continue;
                }
                var argumentMethod = expression.ArgumentList.Arguments[0].Expression as IdentifierNameSyntax;
                var argumentKind = expression.ArgumentList.Arguments[1].Expression as MemberAccessExpressionSyntax;
                var preArgumentKind = argumentKind.Expression as IdentifierNameSyntax;
                if (argumentMethod.Identifier == null || argumentKind.Name == null || preArgumentKind.Identifier == null ||
                    argumentMethod.Identifier.ValueText != "AnalyzeIfStatement" || argumentKind.Name.ToString() != "IfStatement" ||
                    preArgumentKind.Identifier.ValueText != "SyntaxKind")
                {
                    continue;
                }
                statements = statements.Add(statement);
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


        private async Task<Document> InternalStaticAsync(Document document, FieldDeclarationSyntax declaration, CancellationToken c)
        {
            var whiteSpace = SyntaxFactory.Whitespace(" ");
            var internalKeyword = SyntaxFactory.ParseToken("internal").WithTrailingTrivia(whiteSpace);
            var staticKeyword = SyntaxFactory.ParseToken("static").WithTrailingTrivia(whiteSpace);
            var modifierList = SyntaxFactory.TokenList(internalKeyword, staticKeyword);
            var newFieldDeclaration = declaration.WithModifiers(modifierList).WithLeadingTrivia(declaration.GetLeadingTrivia()).WithTrailingTrivia(whiteSpace);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newFieldDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> IncorrectSigAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            var initializeDeclaration = BuildInitialize(document, semanticModel);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, initializeDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }


        private async Task<Document> EnabledByDefaultAsync(Document document, LiteralExpressionSyntax literalExpression, CancellationToken c)
        {
            var newLiteralExpression = (SyntaxFactory.ParseExpression("true").WithLeadingTrivia(literalExpression.GetLeadingTrivia()).WithTrailingTrivia(literalExpression.GetTrailingTrivia())) as LiteralExpressionSyntax;

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(literalExpression, newLiteralExpression);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> IncorrectIfAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var ifStatement = IfHelper(document);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, ifStatement);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }


        private async Task<Document> DiagnosticSeverityError(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken c)
        {
            var newMemberAccessExpressionName = SyntaxFactory.ParseName("Error");

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(memberAccessExpression.Name, newMemberAccessExpressionName);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        #endregion

        #region helper functions
        private SyntaxNode BuildInitialize(Document document, SemanticModel semanticModel)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var type = SyntaxFactory.ParseTypeName("AnalysisContext");
            var parameters = new[] { generator.ParameterDeclaration("context", type) };
            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            var statements = new[] { generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)) };
            var initializeDeclaration = generator.MethodDeclaration("Initialize", parameters: parameters,
                accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, statements: statements);

            return initializeDeclaration;
        }

        private async Task<Document> IncorrectKeywordAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var ifKeyword = KeywordHelper(document, declaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, ifKeyword);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> DiagnosticSeverityWarning(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken c)
        {
            var newMemberAccessExpressionName = SyntaxFactory.ParseName("Warning");

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(memberAccessExpression.Name, newMemberAccessExpressionName);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> TrailingCheckIncorrectAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            var ifBlockStatements = new SyntaxList<StatementSyntax>();
            if (declaration.Body.Statements[2].Kind() == SyntaxKind.IfStatement)
            {
                var ifDeclaration = declaration.Body.Statements[2] as IfStatementSyntax;
                var ifBlock = ifDeclaration.Statement as BlockSyntax;
                ifBlockStatements = ifBlock.Statements;
            }
            var ifStatement = TriviaCheckHelper(document, declaration.Body, ifBlockStatements) as StatementSyntax;
            var oldBlock = declaration.Body;
            var newBlock = declaration.Body.WithStatements(declaration.Body.Statements.Replace(declaration.Body.Statements[2], ifStatement));

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(oldBlock, newBlock);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> TrailingCheckMissingAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            var ifBlockStatements = new SyntaxList<StatementSyntax>();

            var ifStatement = TriviaCheckHelper(document, declaration.Body, ifBlockStatements);
            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, ifStatement);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> DiagnosticSeverityHidden(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken c)
        {
            var newMemberAccessExpressionName = SyntaxFactory.ParseName("Hidden");

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(memberAccessExpression.Name, newMemberAccessExpressionName);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> TrailingVarMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            var localDeclaration = new SyntaxList<SyntaxNode>().Add(TriviaVarMissingHelper(document, declaration));

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(localDeclaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(oldBlock, newBlock);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }


        private async Task<Document> DiagnosticSeverityInfo(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken c)
        {
            var newMemberAccessExpressionName = SyntaxFactory.ParseName("Info");

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(memberAccessExpression.Name, newMemberAccessExpressionName);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> TrailingVarIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            var localDeclaration = TriviaVarMissingHelper(document, declaration) as LocalDeclarationStatementSyntax;

            var oldBlock = declaration.Statement as BlockSyntax;
            var oldStatement = oldBlock.Statements[0];
            var newStatements = oldBlock.Statements.Replace(oldStatement, localDeclaration);
            var newBlock = oldBlock.WithStatements(newStatements);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(oldBlock, newBlock);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private async Task<Document> TrailingKindCheckIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            IfStatementSyntax ifStatement;
            var ifBlockStatements = new SyntaxList<SyntaxNode>();
            if (declaration.Parent.Parent.Kind() == SyntaxKind.MethodDeclaration)
            {
                ifStatement = declaration as IfStatementSyntax;
            }
            else
            {
                ifStatement = declaration.Parent.Parent as IfStatementSyntax;
                var ifBlock = declaration.Statement as BlockSyntax;
                ifBlockStatements = ifBlock.Statements;
            }

            var newIfStatement = TriviaKindCheckHelper(document, ifStatement, ifBlockStatements) as StatementSyntax;

            var oldBlock = ifStatement.Statement as BlockSyntax;
            var oldStatement = oldBlock.Statements[1];
            var newStatements = oldBlock.Statements.Replace(oldStatement, newIfStatement);
            var newBlock = oldBlock.WithStatements(newStatements);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(oldBlock, newBlock);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }


        private async Task<Document> MissingIdDeclarationAsync(Document document, VariableDeclaratorSyntax ruleDeclarationField, CancellationToken c)
        {
            var classDeclaration = ruleDeclarationField.Parent.Parent.Parent as ClassDeclarationSyntax;
            var objectCreationSyntax = ruleDeclarationField.Initializer.Value as ObjectCreationExpressionSyntax;
            var ruleArgumentList = objectCreationSyntax.ArgumentList;

            string currentRuleId = null;
            for (int i = 0; i < ruleArgumentList.Arguments.Count; i++)
            {
                var currentArg = ruleArgumentList.Arguments[i];
                string currentArgName = currentArg.NameColon.Name.Identifier.Text;
                if (currentArgName == "id")
                {
                    currentRuleId = currentArg.Expression.ToString();
                    break;
                }
            }

            var idToken = SyntaxFactory.ParseToken(currentRuleId);
            var expressionKind = SyntaxFactory.ParseExpression("\"DescriptiveId\"") as ExpressionSyntax;
            var newClassDeclaration = newIdCreator(idToken, expressionKind, classDeclaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> WhitespaceCheckIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            IfStatementSyntax ifStatement;
            var ifBlockStatements = new SyntaxList<SyntaxNode>();

            if (declaration.Parent.Parent.Parent.Parent.Kind() == SyntaxKind.MethodDeclaration)
            {
                ifStatement = declaration as IfStatementSyntax;
            }
            else
            {
                ifStatement = declaration.Parent.Parent as IfStatementSyntax;
                var ifBlock = declaration.Statement as BlockSyntax;
                ifBlockStatements = ifBlock.Statements;
            }

            var newIfStatement = WhitespaceCheckHelper(document, ifStatement, ifBlockStatements) as StatementSyntax;

            var oldBlock = ifStatement.Statement as BlockSyntax;
            var oldStatement = oldBlock.Statements[0];
            var newStatement = oldBlock.Statements.Replace(oldStatement, newIfStatement);
            var newBlock = oldBlock.WithStatements(newStatement);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(oldBlock, newBlock);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }


        private async Task<Document> IdDeclTypeAsync(Document document, LiteralExpressionSyntax literalExpression, CancellationToken c)
        {
            var idName = SyntaxFactory.ParseName(literalExpression.Token.Value.ToString()) as IdentifierNameSyntax;

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(literalExpression, idName);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }
        private async Task<Document> ReturnIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            IfStatementSyntax ifStatement;
            if (declaration.Parent.Parent.Parent.Parent.Parent.Parent.Kind() != SyntaxKind.MethodDeclaration)
            {
                ifStatement = declaration.Parent.Parent as IfStatementSyntax;
            }
            else
            {
                ifStatement = declaration;
            }

            var generator = SyntaxGenerator.GetGenerator(document);
            var returnStatement = generator.ReturnStatement() as ReturnStatementSyntax;

            var oldBlock = ifStatement.Statement as BlockSyntax;
            var newStatement = oldBlock.Statements.Replace(oldBlock.Statements[0], returnStatement);
            var newBlock = oldBlock.WithStatements(newStatement);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(oldBlock, newBlock);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private async Task<Document> TooManyStatementsAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            var oldBlock = declaration.Statement as BlockSyntax;
            var onlyStatement = new SyntaxList<StatementSyntax>().Add(oldBlock.Statements[0]);
            var newBlock = oldBlock.WithStatements(onlyStatement);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(oldBlock, newBlock);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        #endregion

        #region Helper functions
        private SyntaxNode IfHelper(Document document)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var type = SyntaxFactory.ParseTypeName("IfStatementSyntax");
            var expression = generator.IdentifierName("context");
            var memberAccessExpression = generator.MemberAccessExpression(expression, "Node");
            var initializer = generator.CastExpression(type, memberAccessExpression);
            var ifStatement = generator.LocalDeclarationStatement("ifStatement", initializer);

            return ifStatement;
        }

        private SyntaxNode KeywordHelper(Document document, StatementSyntax declaration)
        {
            var methodBlock = declaration.Parent as BlockSyntax;
            var firstStatement = methodBlock.Statements[0] as LocalDeclarationStatementSyntax;

            var generator = SyntaxGenerator.GetGenerator(document);
            var variableName = generator.IdentifierName(firstStatement.Declaration.Variables[0].Identifier.ValueText);
            var initializer = generator.MemberAccessExpression(variableName, "IfKeyword");
            var ifKeyword = generator.LocalDeclarationStatement("ifKeyword", initializer);

            return ifKeyword;
        }

        private SyntaxNode TriviaCheckHelper(Document document, BlockSyntax methodBlock, SyntaxList<StatementSyntax> ifBlockStatements)
        {
            var secondStatement = methodBlock.Statements[1] as LocalDeclarationStatementSyntax;

            var generator = SyntaxGenerator.GetGenerator(document);
            var variableName = generator.IdentifierName(secondStatement.Declaration.Variables[0].Identifier.ValueText);
            var conditional = generator.MemberAccessExpression(variableName, "HasTrailingTrivia");
            var ifStatement = generator.IfStatement(conditional, ifBlockStatements);

            return ifStatement;
        }

        private SyntaxNode TriviaVarMissingHelper(Document document, IfStatementSyntax declaration)
        {
            var methodBlock = declaration.Parent as BlockSyntax;
            var secondStatement = methodBlock.Statements[1] as LocalDeclarationStatementSyntax;

            var generator = SyntaxGenerator.GetGenerator(document);
            var variableName = generator.IdentifierName(secondStatement.Declaration.Variables[0].Identifier.ValueText);

            var ifTrailing = generator.MemberAccessExpression(variableName, "TrailingTrivia");
            var fullVariable = generator.MemberAccessExpression(ifTrailing, "Last");
            var parameters = new SyntaxList<SyntaxNode>();
            var variableExpression = generator.InvocationExpression(fullVariable, parameters);

            var localDeclaration = generator.LocalDeclarationStatement("trailingTrivia", variableExpression);

            return localDeclaration;
        }

        private SyntaxNode TriviaKindCheckHelper(Document document, IfStatementSyntax ifStatement, SyntaxList<SyntaxNode> ifBlockStatements)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var ifOneBlock = ifStatement.Statement as BlockSyntax;

            var trailingTriviaDeclaration = ifOneBlock.Statements[0] as LocalDeclarationStatementSyntax;
            var trailingTrivia = generator.IdentifierName(trailingTriviaDeclaration.Declaration.Variables[0].Identifier.ValueText);
            var arguments = new SyntaxList<SyntaxNode>();
            var trailingTriviaKind = generator.InvocationExpression(generator.MemberAccessExpression(trailingTrivia, "Kind"), arguments);

            var whitespaceTrivia = generator.MemberAccessExpression(generator.IdentifierName("SyntaxKind"), "WhitespaceTrivia");

            var equalsExpression = generator.ValueEqualsExpression(trailingTriviaKind, whitespaceTrivia);

            var newIfStatement = generator.IfStatement(equalsExpression, ifBlockStatements);

            return newIfStatement;
        }

        private SyntaxNode WhitespaceCheckHelper(Document document, IfStatementSyntax ifStatement, SyntaxList<SyntaxNode> ifBlockStatements)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var ifOneBlock = ifStatement.Parent as BlockSyntax;
            var ifTwoBlock = ifStatement.Statement as BlockSyntax;

            var trailingTriviaDeclaration = ifOneBlock.Statements[0] as LocalDeclarationStatementSyntax;
            var trailingTrivia = generator.IdentifierName(trailingTriviaDeclaration.Declaration.Variables[0].Identifier.ValueText);
            var arguments = new SyntaxList<SyntaxNode>();

            var trailingTriviaToString = generator.InvocationExpression(generator.MemberAccessExpression(trailingTrivia, "ToString"), arguments);
            var rightSide = generator.LiteralExpression(" ");
            var equalsExpression = generator.ValueEqualsExpression(trailingTriviaToString, rightSide);

            var newIfStatement = generator.IfStatement(equalsExpression, ifBlockStatements);

            return newIfStatement;
        }
        #endregion

    }
}