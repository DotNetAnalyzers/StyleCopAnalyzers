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
                //TODO: should be 47 when done
                return ImmutableArray.Create(MetaCompilationAnalyzer.MissingId, 
                                             MetaCompilationAnalyzer.MissingInit, 
                                             MetaCompilationAnalyzer.MissingRegisterStatement,
                                             MetaCompilationAnalyzer.TooManyInitStatements,
                                             MetaCompilationAnalyzer.InvalidStatement,
                                             MetaCompilationAnalyzer.IfStatementIncorrect,
                                             MetaCompilationAnalyzer.IfKeywordIncorrect,
                                             MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                                             MetaCompilationAnalyzer.TrailingTriviaVarMissing,
                                             MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                                             MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                                             MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                                             MetaCompilationAnalyzer.ReturnStatementIncorrect,
                                             MetaCompilationAnalyzer.TooManyStatements,
                                             MetaCompilationAnalyzer.LocationMissing,
                                             MetaCompilationAnalyzer.LocationIncorrect,
                                             MetaCompilationAnalyzer.SpanMissing,
                                             MetaCompilationAnalyzer.SpanIncorrect,
                                             MetaCompilationAnalyzer.EndSpanIncorrect,
                                             MetaCompilationAnalyzer.EndSpanMissing,
                                             MetaCompilationAnalyzer.StartSpanIncorrect,
                                             MetaCompilationAnalyzer.StartSpanMissing,
                                             MetaCompilationAnalyzer.OpenParenIncorrect,
                                             MetaCompilationAnalyzer.OpenParenMissing,
                                             MetaCompilationAnalyzer.MissingAnalysisMethod,
                                             MetaCompilationAnalyzer.DiagnosticMissing,
                                             MetaCompilationAnalyzer.DiagnosticIncorrect,
                                             MetaCompilationAnalyzer.DiagnosticReportIncorrect,
                                             MetaCompilationAnalyzer.DiagnosticReportMissing);
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
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The Initialize method can only register actions, all other statements are invalid", c => InvalidStatementAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfStatementIncorrect))
                {
                    StatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The first statement of the analyzer must access the node to be analyzed", c => IncorrectIfAsync(context.Document, declaration, c)), diagnostic);
                }
                
                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfKeywordIncorrect))
                {
                    StatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The second statement of the analyzer must access the keyword from the node being analyzed", c => IncorrectKeywordAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect))
                {
                    StatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The third statement of the analyzer must be an if statement checking the trailing trivia of the node being analyzed", c => TrailingCheckIncorrectAsync(context.Document, declaration, c)), diagnostic);
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

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.LocationMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create a diagnostic location. This is where the red squiggle will appear in the code that you are analyzing", c => AddLocationAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.LocationIncorrect))
                {
                    try
                    {
                        LocalDeclarationStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create a diagnostic location. This is where the red squiggle will appear in the code that you are analyzing", c => ReplaceLocationAsync(context.Document, declaration, c)), diagnostic);
                    }
                    catch
                    {
                        return;
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.SpanMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create a diagnostic span. This is where the red squiggle will appear in the code that you are analyzing", c => AddSpanAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.SpanIncorrect))
                {
                    try
                    {
                        LocalDeclarationStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create a diagnostic span. This is where the red squiggle will appear in the code that you are analyzing", c => ReplaceSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                    catch
                    {
                        return;
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.EndSpanMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create an int that is the end of the diagnostic span", c => AddEndSpanAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.EndSpanIncorrect))
                {
                    try
                    {
                        LocalDeclarationStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create an int that is the end of the diagnostic span", c => ReplaceEndSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                    catch
                    {
                        return;
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.StartSpanMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create an int that is the start of the diagnostic span", c => AddStartSpanAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.StartSpanIncorrect))
                {
                    try
                    {
                        LocalDeclarationStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create an int that is the start of the diagnostic span", c => ReplaceStartSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                    catch
                    {
                        return;
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.OpenParenMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Extract the open parenthesis from the if statement", c => AddOpenParenAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.OpenParenIncorrect))
                {
                    try
                    {
                        LocalDeclarationStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Extract the open parenthesis from the if statement", c => ReplaceOpenParenAsync(context.Document, declaration, c)), diagnostic);
                    }
                    catch
                    {
                        return;
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticMissing))
                {
                    ClassDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create the diagnostic that is going to be reported", c => AddDiagnosticAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticIncorrect))
                {
                    try
                    {
                        LocalDeclarationStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create the diagnostic that is going to be reported", c => ReplaceDiagnosticAsync(context.Document, declaration, c)), diagnostic);
                    }
                    catch
                    {
                        return;
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticReportMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Report the diagnostic to the context of the if statement in question", c => AddDiagnosticReportAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticReportIncorrect))
                {
                    try
                    {
                        ExpressionStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ExpressionStatementSyntax>().First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Report the diagnostic to the context of the if statement in question", c => ReplaceDiagnosticReportAsync(context.Document, declaration, c)), diagnostic);
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }

        private async Task<Document> ReplaceDiagnosticReportAsync(Document document, ExpressionStatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();

            SyntaxNode diagnosticReport = CreateDiagnosticReport(document, methodDeclaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, diagnosticReport);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> AddDiagnosticReportAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode diagnosticReport = CreateDiagnosticReport(document, declaration);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(diagnosticReport);
            var newMethod = generator.WithStatements(declaration, newStatements);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newMethod);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private SyntaxNode CreateDiagnosticReport(Document document, MethodDeclarationSyntax declaration)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string argumentName = (declaration.Body.Statements[8] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            var argumentExpression = generator.IdentifierName(argumentName);
            var argument = generator.Argument(argumentExpression);

            string contextName = (declaration.ParameterList.Parameters[0].Identifier.Text);
            var identifier = generator.IdentifierName(contextName);
            var memberExpression = generator.MemberAccessExpression(identifier, "ReportDiagnostic");
            var expression = generator.InvocationExpression(memberExpression, argument);

            SyntaxNode expressionStatement = generator.ExpressionStatement(expression);
            return expressionStatement;
        }

        private async Task<Document> ReplaceDiagnosticAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            var classDeclaration = methodDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().First();

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string ruleName = GetFirstRuleName(classDeclaration);

            var diagnostic = CreateDiagnostic(document, methodDeclaration, ruleName);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, diagnostic);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> AddDiagnosticAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string ruleName = GetFirstRuleName(declaration);
            MethodDeclarationSyntax analysis = GetAnalysis(declaration);

            var diagnostic = CreateDiagnostic(document, analysis, ruleName);

            var oldStatements = (SyntaxList<SyntaxNode>)analysis.Body.Statements;
            var newStatements = oldStatements.Add(diagnostic);
            var newMethod = generator.WithStatements(analysis, newStatements);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(analysis, newMethod);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private string GetFirstRuleName(ClassDeclarationSyntax declaration)
        {
            SyntaxList<MemberDeclarationSyntax> members = declaration.Members;
            FieldDeclarationSyntax rule = null;

            foreach (var member in members)
            {
                rule = member as FieldDeclarationSyntax;
                if (rule != null && rule.Declaration.Type.ToString() == "DiagnosticDescriptor")
                {
                    break;
                }
            }

            return rule.Declaration.Variables[0].Identifier.Text;
        }

        private MethodDeclarationSyntax GetAnalysis(ClassDeclarationSyntax declaration)
        {
            SyntaxList<MemberDeclarationSyntax> members = declaration.Members;
            MethodDeclarationSyntax analysis = null;

            foreach (var member in members)
            {
                analysis = member as MethodDeclarationSyntax;
                if (analysis != null && analysis.Identifier.Text != "Initialize")
                {
                    break;
                }
            }

            return analysis;
        }

        private SyntaxNode CreateDiagnostic(Document document, MethodDeclarationSyntax declaration, string ruleName)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var identifier = generator.IdentifierName("Diagnostic");
            var expression = generator.MemberAccessExpression(identifier, "Create");

            SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();

            var ruleExpression = generator.IdentifierName(ruleName);
            var ruleArg = generator.Argument(ruleExpression);

            string locationName = (declaration.Body.Statements[7] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            var locationExpression = generator.IdentifierName(locationName);
            var locationArg = generator.Argument(locationExpression);

            var messageExpression = generator.MemberAccessExpression(ruleExpression, "MessageFormat");
            var messageArg = generator.Argument(messageExpression);

            arguments = arguments.Add(ruleArg);
            arguments = arguments.Add(locationArg);
            arguments = arguments.Add(messageArg);

            string name = "diagnostic";
            var initializer = generator.InvocationExpression(expression, arguments);
            SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

            return localDeclaration;
        }


        //replaces an incorrect open parenthsis statement
        private async Task<Document> ReplaceOpenParenAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();

            SyntaxNode openParen = CreateOpenParen(document, methodDeclaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, openParen);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //adds the open parenthesis statement
        private async Task<Document> AddOpenParenAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode openParen = CreateOpenParen(document, declaration);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(openParen);
            var newMethod = generator.WithStatements(declaration, newStatements);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newMethod);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //creates the open parenthesis statement
        private SyntaxNode CreateOpenParen(Document document, MethodDeclarationSyntax declaration)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            string name = "openParen";

            string expressionString = (declaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            var expression = generator.IdentifierName(expressionString);

            var initializer = generator.MemberAccessExpression(expression, "OpenParenToken");
            SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

            return localDeclaration;
        }

        //replaces an incorrect start span statement
        private async Task<Document> ReplaceStartSpanAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();

            SyntaxNode startSpan = CreateEndOrStartSpan(document, methodDeclaration, "startDiagnosticSpan");

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, startSpan);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //adds a start span statement
        private async Task<Document> AddStartSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode startSpan = CreateEndOrStartSpan(document, declaration, "startDiagnosticSpan");
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(startSpan);
            var newMethod = generator.WithStatements(declaration, newStatements);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newMethod);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //replace an incorrect end span statement
        private async Task<Document> ReplaceEndSpanAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();

            SyntaxNode endSpan = CreateEndOrStartSpan(document, methodDeclaration, "endDiagnosticSpan");

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, endSpan);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //adds an end span statement
        private async Task<Document> AddEndSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode endSpan = CreateEndOrStartSpan(document, declaration, "endDiagnosticSpan");
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(endSpan);
            var newMethod = generator.WithStatements(declaration, newStatements);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newMethod);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //creates an end or start span statement
        private SyntaxNode CreateEndOrStartSpan(Document document, MethodDeclarationSyntax declaration, string variableName)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode identifier = null;
            if (variableName == "startDiagnosticSpan")
            {
                string identifierString = (declaration.Body.Statements[1] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
                identifier = generator.IdentifierName(identifierString);
            }
            else if (variableName == "endDiagnosticSpan")
            {
                string identifierString = (declaration.Body.Statements[3] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
                identifier = generator.IdentifierName(identifierString);
            }

            SyntaxNode expression = generator.MemberAccessExpression(identifier, "Span");
            SyntaxNode initializer = generator.MemberAccessExpression(expression, "Start");

            SyntaxNode localDeclaration = generator.LocalDeclarationStatement(variableName, initializer);

            return localDeclaration;
        }

        //replaces an incorrect diagnostic span statement
        private async Task<Document> ReplaceSpanAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();

            SyntaxNode span = CreateSpan(document, methodDeclaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, span);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //adds the diagnostic span statement
        private async Task<Document> AddSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode span = CreateSpan(document, declaration);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(span);
            var newMethod = generator.WithStatements(declaration, newStatements);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newMethod);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //creates the diagnostic span statement
        private SyntaxNode CreateSpan(Document document, MethodDeclarationSyntax declaration)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            string name = "diagnosticSpan";
            SyntaxNode memberIdentifier = generator.IdentifierName("TextSpan");
            SyntaxNode memberName = generator.IdentifierName("FromBounds");
            SyntaxNode expression = generator.MemberAccessExpression(memberIdentifier, memberName);

            SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();
            string startIdentifier = (declaration.Body.Statements[4] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            var startSpanIdentifier = generator.IdentifierName(startIdentifier);

            string endIdentifier = (declaration.Body.Statements[5] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            var endSpanIdentifier = generator.IdentifierName(endIdentifier);

            arguments = arguments.Add(startSpanIdentifier);
            arguments = arguments.Add(endSpanIdentifier);

            SyntaxNode initializer = generator.InvocationExpression(expression, arguments);
            SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

            return localDeclaration;
        }

        //replace an incorrect diagnostic location statement
        private async Task<Document> ReplaceLocationAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();

            SyntaxNode location = CreateLocation(document, methodDeclaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, location);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //adds the diagnostic location statement
        private async Task<Document> AddLocationAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode location = CreateLocation(document, declaration);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(location);
            var newMethod = generator.WithStatements(declaration, newStatements);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, newMethod);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        //creates the diagnostic location statement
        private SyntaxNode CreateLocation(Document document, MethodDeclarationSyntax declaration)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            string name = "diagnosticLocation";
            SyntaxNode memberIdentifier = generator.IdentifierName("Location");
            SyntaxNode memberName = generator.IdentifierName("Create");
            SyntaxNode expression = generator.MemberAccessExpression(memberIdentifier, memberName);

            SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();
            string ifStatementIdentifier = (declaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            var treeIdentifier = generator.IdentifierName(ifStatementIdentifier);
            var treeArgExpression = generator.MemberAccessExpression(treeIdentifier, "SyntaxTree");
            var treeArg = generator.Argument(treeArgExpression);

            string spanIdentifier = (declaration.Body.Statements[6] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            var spanArgIdentifier = generator.IdentifierName(spanIdentifier);
            var spanArg = generator.Argument(spanArgIdentifier);

            arguments = arguments.Add(treeArg);
            arguments = arguments.Add(spanArg);

            SyntaxNode initializer = generator.InvocationExpression(expression, arguments);
            SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

            return localDeclaration;
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

        private async Task<Document> IncorrectIfAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var ifStatement = IfHelper(document);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, ifStatement);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private async Task<Document> IncorrectKeywordAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var ifKeyword = KeywordHelper(document, declaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, ifKeyword);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private async Task<Document> TrailingCheckIncorrectAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var ifStatement = TriviaCheckHelper(document, declaration);

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(declaration, ifStatement);
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

        private SyntaxNode TriviaCheckHelper(Document document, StatementSyntax declaration)
        {
            var methodBlock = declaration.Parent as BlockSyntax;
            var secondStatement = methodBlock.Statements[1] as LocalDeclarationStatementSyntax;

            var generator = SyntaxGenerator.GetGenerator(document);
            var variableName = generator.IdentifierName(secondStatement.Declaration.Variables[0].Identifier.ValueText);
            var conditional = generator.MemberAccessExpression(variableName, "HasTrailingTrivia");
            var trueStatements = new SyntaxList<SyntaxNode>();
            var ifStatement = generator.IfStatement(conditional, trueStatements);

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