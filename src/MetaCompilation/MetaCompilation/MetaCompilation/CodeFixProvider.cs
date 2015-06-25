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
                                             MetaCompilationAnalyzer.IncorrectInitSig,
                                             MetaCompilationAnalyzer.InvalidStatement,
                                             MetaCompilationAnalyzer.MissingSuppDiag,
                                             MetaCompilationAnalyzer.IncorrectSigSuppDiag,
                                             MetaCompilationAnalyzer.MissingAccessor,
                                             MetaCompilationAnalyzer.TooManyAccessors,
                                             MetaCompilationAnalyzer.IncorrectAccessorReturn,
                                             MetaCompilationAnalyzer.SuppDiagReturnValue,
                                             MetaCompilationAnalyzer.SupportedRules,
                                             MetaCompilationAnalyzer.IdDeclTypeError,
                                             MetaCompilationAnalyzer.MissingIdDeclaration,
                                             MetaCompilationAnalyzer.DefaultSeverityError,
                                             MetaCompilationAnalyzer.EnabledByDefaultError,
                                             MetaCompilationAnalyzer.InternalAndStaticError,
                                             MetaCompilationAnalyzer.MissingRule,
                                             MetaCompilationAnalyzer.MissingAnalysisMethod,
                                             MetaCompilationAnalyzer.IfStatementMissing,
                                             MetaCompilationAnalyzer.IfStatementIncorrect,
                                             MetaCompilationAnalyzer.IfKeywordMissing,
                                             MetaCompilationAnalyzer.IfKeywordIncorrect,
                                             MetaCompilationAnalyzer.TrailingTriviaCheckMissing,
                                             MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                                             MetaCompilationAnalyzer.TrailingTriviaVarMissing,
                                             MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                                             MetaCompilationAnalyzer.WhitespaceCheckMissing,
                                             MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                                             MetaCompilationAnalyzer.ReturnStatementMissing,
                                             MetaCompilationAnalyzer.ReturnStatementIncorrect,
                                             MetaCompilationAnalyzer.OpenParenIncorrect,
                                             MetaCompilationAnalyzer.OpenParenMissing,
                                             MetaCompilationAnalyzer.StartSpanIncorrect,
                                             MetaCompilationAnalyzer.StartSpanMissing,
                                             MetaCompilationAnalyzer.EndSpanIncorrect,
                                             MetaCompilationAnalyzer.EndSpanMissing,
                                             MetaCompilationAnalyzer.SpanIncorrect,
                                             MetaCompilationAnalyzer.SpanMissing,
                                             MetaCompilationAnalyzer.LocationIncorrect,
                                             MetaCompilationAnalyzer.LocationMissing,
                                             MetaCompilationAnalyzer.MissingAnalysisMethod,
                                             MetaCompilationAnalyzer.TooManyStatements,
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

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.InternalAndStaticError))
                {
                    FieldDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Rules must be declared as both internal and static.", c => InternalStaticAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.EnabledByDefaultError))
                {
                    LiteralExpressionSyntax literalExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Rules should be enabled by default.", c => EnabledByDefaultAsync(context.Document, literalExpression, c)), diagnostic);
                }

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.DefaultSeverityError))
                {
                    MemberAccessExpressionSyntax memberAccessExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: defaultSeverity should be set to \"Error\" if something is not allowed by the language authorities.", c => DiagnosticSeverityError(context.Document, memberAccessExpression, c)), diagnostic);
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: defaultSeverity should be set to \"Warning\" if something is suspicious but allowed.", c => DiagnosticSeverityWarning(context.Document, memberAccessExpression, c)), diagnostic);
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: defaultSeverity should be set to \"Hidden\" if something is an issue, but is not surfaced by normal means.", c => DiagnosticSeverityHidden(context.Document, memberAccessExpression, c)), diagnostic);
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: defaultSeverity should be set to \"Info\" for information that does not indicate a problem.", c => DiagnosticSeverityInfo(context.Document, memberAccessExpression, c)), diagnostic);
                }

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.MissingIdDeclaration))
                {
                    VariableDeclaratorSyntax ruleDeclarationField = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Generate a public field for this rule id.", c => MissingIdDeclarationAsync(context.Document, ruleDeclarationField, c)), diagnostic);
                }

                if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.IdDeclTypeError))
                {
                    LiteralExpressionSyntax literalExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Rule ids should not be string literals.", c => IdDeclTypeAsync(context.Document, literalExpression, c)), diagnostic);
                }
           
                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfStatementIncorrect))
                {
                    StatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The first statement of the analyzer must access the node to be analyzed", c => IncorrectIfAsync(context.Document, declaration, c)), diagnostic);
                }
                
                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfStatementMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The first statement of the analyzer must access the node to be analyzed", c => MissingIfAsync(context.Document, declaration, c)), diagnostic);
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

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfKeywordMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The second statement of the analyzer must access the keyword from the node being analyzed", c => MissingKeywordAsync(context.Document, declaration, c)), diagnostic);
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

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaKindCheckMissing))
                {
                    IfStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The third statement of the analyzer must be an if statement checking the trailing trivia of the node being analyzed", c => TrailingKindCheckMissingAsync(context.Document, declaration, c)), diagnostic);
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
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: There are too many statments within this if block; its only purpose is to return if the statement is formatted properly", c => TooManyStatementsAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.ReturnStatementMissing))
                {
                    IfStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: There must be a return statement indicating that the spacing for the if statement is correct", c => ReturnMissingAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.WhitespaceCheckMissing))
                {
                    IfStatementSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: The sixth statement of the analyzer should be a check to ensure the whitespace after the if statement keyword is correct", c => WhitespaceCheckMissingAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.LocationMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create a diagnostic location. This is where the red squiggle will appear in the code that you are analyzing", c => AddLocationAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.LocationIncorrect))
                {
                    IEnumerable<LocalDeclarationStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        LocalDeclarationStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create a diagnostic location. This is where the red squiggle will appear in the code that you are analyzing", c => ReplaceLocationAsync(context.Document, declaration, c)), diagnostic);
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.SpanMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create a diagnostic span. This is where the red squiggle will appear in the code that you are analyzing", c => AddSpanAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.SpanIncorrect))
                {
                    IEnumerable<LocalDeclarationStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        LocalDeclarationStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create a diagnostic span. This is where the red squiggle will appear in the code that you are analyzing", c => ReplaceSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.EndSpanMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create an int that is the end of the diagnostic span", c => AddEndSpanAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.EndSpanIncorrect))
                {
                    IEnumerable<LocalDeclarationStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        LocalDeclarationStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create an int that is the end of the diagnostic span", c => ReplaceEndSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.StartSpanMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create an int that is the start of the diagnostic span", c => AddStartSpanAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.StartSpanIncorrect))
                {
                    IEnumerable<LocalDeclarationStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        LocalDeclarationStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create an int that is the start of the diagnostic span", c => ReplaceStartSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.OpenParenMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Extract the open parenthesis from the if statement", c => AddOpenParenAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.OpenParenIncorrect))
                {
                    IEnumerable<LocalDeclarationStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        LocalDeclarationStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Extract the open parenthesis from the if statement", c => ReplaceOpenParenAsync(context.Document, declaration, c)), diagnostic);
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticMissing))
                {
                    ClassDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Create the diagnostic that is going to be reported", c => AddDiagnosticAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticIncorrect))
                {
                    IEnumerable<LocalDeclarationStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        LocalDeclarationStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Create the diagnostic that is going to be reported", c => ReplaceDiagnosticAsync(context.Document, declaration, c)), diagnostic);
                    }
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticReportMissing))
                {
                    MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                    context.RegisterCodeFix(CodeAction.Create("Tutorial: Report the diagnostic to the context of the if statement in question", c => AddDiagnosticReportAsync(context.Document, declaration, c)), diagnostic);
                }

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticReportIncorrect))
                {
                    IEnumerable<ExpressionStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ExpressionStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ExpressionStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create("Tutorial: Report the diagnostic to the context of the if statement in question", c => ReplaceDiagnosticReportAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
            }
        }

        private async Task<Document> ReplaceNode(SyntaxNode oldNode, SyntaxNode newNode, Document document)
        {
            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(oldNode, newNode);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> ReplaceDiagnosticReportAsync(Document document, ExpressionStatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            string argumentName = (methodDeclaration.Body.Statements[8] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string contextName = (methodDeclaration.ParameterList.Parameters[0].Identifier.Text);

            SyntaxNode diagnosticReport = CodeFixNodeCreator.CreateDiagnosticReport(generator, argumentName, contextName);

            return await ReplaceNode(declaration, diagnosticReport, document);
        }

        private async Task<Document> AddDiagnosticReportAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            string argumentName = (declaration.Body.Statements[8] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string contextName = (declaration.ParameterList.Parameters[0].Identifier.Text);
            SyntaxNode diagnosticReport = CodeFixNodeCreator.CreateDiagnosticReport(generator, argumentName, contextName);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(diagnosticReport);
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        private async Task<Document> ReplaceDiagnosticAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            var classDeclaration = methodDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().First();

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            string locationName = (methodDeclaration.Body.Statements[7] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            string ruleName = CodeFixNodeCreator.GetFirstRuleName(classDeclaration);

            var diagnostic = CodeFixNodeCreator.CreateDiagnostic(generator, locationName, ruleName);

            return await ReplaceNode(declaration, diagnostic, document);
        }

        private async Task<Document> AddDiagnosticAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string ruleName = CodeFixNodeCreator.GetFirstRuleName(declaration);
            MethodDeclarationSyntax analysis = CodeFixNodeCreator.GetAnalysis(declaration);
            string locationName = (analysis.Body.Statements[7] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            var diagnostic = CodeFixNodeCreator.CreateDiagnostic(generator, locationName, ruleName);

            var oldStatements = (SyntaxList<SyntaxNode>)analysis.Body.Statements;
            var newStatements = oldStatements.Add(diagnostic);
            var newMethod = generator.WithStatements(analysis, newStatements);

            return await ReplaceNode(analysis, newMethod, document);
        }

        //replaces an incorrect open parenthsis statement
        private async Task<Document> ReplaceOpenParenAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string expressionString = (methodDeclaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode openParen = CodeFixNodeCreator.CreateOpenParen(generator, expressionString);

            return await ReplaceNode(declaration, openParen, document);
        }

        //adds the open parenthesis statement
        private async Task<Document> AddOpenParenAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string expressionString = (declaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode openParen = CodeFixNodeCreator.CreateOpenParen(generator, expressionString);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(openParen);
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        //replaces an incorrect start span statement
        private async Task<Document> ReplaceStartSpanAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string identifierString = (methodDeclaration.Body.Statements[1] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode startSpan = CodeFixNodeCreator.CreateEndOrStartSpan(generator, identifierString, "startDiagnosticSpan");

            return await ReplaceNode(declaration, startSpan, document);
        }

        //adds a start span statement
        private async Task<Document> AddStartSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string identifierString = (declaration.Body.Statements[1] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode startSpan = CodeFixNodeCreator.CreateEndOrStartSpan(generator, identifierString, "startDiagnosticSpan");
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(startSpan);
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        //replace an incorrect end span statement
        private async Task<Document> ReplaceEndSpanAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string identifierString = (methodDeclaration.Body.Statements[3] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode endSpan = CodeFixNodeCreator.CreateEndOrStartSpan(generator, identifierString, "endDiagnosticSpan");

            return await ReplaceNode(declaration, endSpan, document);
        }

        //adds an end span statement
        private async Task<Document> AddEndSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string identifierString = (declaration.Body.Statements[3] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode endSpan = CodeFixNodeCreator.CreateEndOrStartSpan(generator, identifierString, "endDiagnosticSpan");
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(endSpan);
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        //replaces an incorrect diagnostic span statement
        private async Task<Document> ReplaceSpanAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string startIdentifier = (methodDeclaration.Body.Statements[4] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string endIdentifier = (methodDeclaration.Body.Statements[5] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode span = CodeFixNodeCreator.CreateSpan(generator, startIdentifier, endIdentifier);

            return await ReplaceNode(declaration, span, document);
        }

        //adds the diagnostic span statement
        private async Task<Document> AddSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string startIdentifier = (declaration.Body.Statements[4] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string endIdentifier = (declaration.Body.Statements[5] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode span = CodeFixNodeCreator.CreateSpan(generator, startIdentifier, endIdentifier);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(span);
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        //replace an incorrect diagnostic location statement
        private async Task<Document> ReplaceLocationAsync(Document document, LocalDeclarationStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string ifStatementIdentifier = (methodDeclaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string spanIdentifier = (methodDeclaration.Body.Statements[6] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode location = CodeFixNodeCreator.CreateLocation(generator, ifStatementIdentifier, spanIdentifier);

            return await ReplaceNode(declaration, location, document);
        }

        //adds the diagnostic location statement
        private async Task<Document> AddLocationAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string ifStatementIdentifier = (declaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string spanIdentifier = (declaration.Body.Statements[6] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode location = CodeFixNodeCreator.CreateLocation(generator, ifStatementIdentifier, spanIdentifier);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(location);
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        #region id code fix
        private async Task<Document> MissingIdAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            var idToken = SyntaxFactory.ParseToken("spacingRuleId");
            var expressionKind = SyntaxFactory.ParseExpression("\"IfSpacing\"") as ExpressionSyntax;
            var newClassDeclaration = CodeFixNodeCreator.NewIdCreator(idToken, expressionKind, declaration);

            return await ReplaceNode(declaration, newClassDeclaration, document);
        }
        #endregion

        #region initialize code fix
        private async Task<Document> MissingInitAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            var initializeDeclaration = CodeFixNodeCreator.BuildInitialize(generator, notImplementedException);

            var newClassDeclaration = generator.AddMembers(declaration, initializeDeclaration);

            return await ReplaceNode(declaration, newClassDeclaration, document);
        }

        private async Task<Document> MissingRegisterAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            var registerExpression = SyntaxFactory.ExpressionStatement(SyntaxFactory.ParseExpression("context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement)"));

            var newInitBlock = SyntaxFactory.Block(registerExpression);
            var newInitDeclaration = declaration.WithBody(newInitBlock);

            return await ReplaceNode(declaration, newInitDeclaration, document);
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

            return await ReplaceNode(declaration, newDeclaration, document);
        }

        private async Task<Document> InvalidStatementAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            BlockSyntax initializeCodeBlock = declaration.Parent as BlockSyntax;
            MethodDeclarationSyntax initializeDeclaration = initializeCodeBlock.Parent as MethodDeclarationSyntax;

            BlockSyntax newCodeBlock = initializeCodeBlock.WithStatements(initializeCodeBlock.Statements.Remove(declaration));
            MethodDeclarationSyntax newInitializeDeclaration = initializeDeclaration.WithBody(newCodeBlock);

            return await ReplaceNode(initializeDeclaration, newInitializeDeclaration, document);
        }



        private async Task<Document> IncorrectSigAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            var initializeDeclaration = CodeFixNodeCreator.BuildInitialize(generator, notImplementedException);

            return await ReplaceNode(declaration, initializeDeclaration, document);
        }



        private async Task<Document> IncorrectIfAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var ifStatement = CodeFixNodeCreator.IfHelper(generator);

            return await ReplaceNode(declaration, ifStatement, document);
        }



        private async Task<Document> MissingIfAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            StatementSyntax ifStatement = CodeFixNodeCreator.IfHelper(generator) as StatementSyntax;

            var oldBlock = declaration.Body as BlockSyntax;
            var newBlock = oldBlock.AddStatements(ifStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
        }
        #endregion

        #region rule code fix
        private async Task<Document> InternalStaticAsync(Document document, FieldDeclarationSyntax declaration, CancellationToken c)
        {
            var whiteSpace = SyntaxFactory.Whitespace(" ");
            var internalKeyword = SyntaxFactory.ParseToken("internal").WithTrailingTrivia(whiteSpace);
            var staticKeyword = SyntaxFactory.ParseToken("static").WithTrailingTrivia(whiteSpace);
            var modifierList = SyntaxFactory.TokenList(internalKeyword, staticKeyword);
            var newFieldDeclaration = declaration.WithModifiers(modifierList).WithLeadingTrivia(declaration.GetLeadingTrivia()).WithTrailingTrivia(whiteSpace);

            return await ReplaceNode(declaration, newFieldDeclaration, document);
        }

        private async Task<Document> EnabledByDefaultAsync(Document document, LiteralExpressionSyntax literalExpression, CancellationToken c)
        {
            var newLiteralExpression = (SyntaxFactory.ParseExpression("true").WithLeadingTrivia(literalExpression.GetLeadingTrivia()).WithTrailingTrivia(literalExpression.GetTrailingTrivia())) as LiteralExpressionSyntax;

            return await ReplaceNode(literalExpression, newLiteralExpression, document);
        }

        private async Task<Document> DiagnosticSeverityWarning(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken c)
        {
            var newMemberAccessExpressionName = SyntaxFactory.ParseName("Warning");

            return await ReplaceNode(memberAccessExpression.Name, newMemberAccessExpressionName, document);
        }

        private async Task<Document> DiagnosticSeverityHidden(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken c)
        {
            var newMemberAccessExpressionName = SyntaxFactory.ParseName("Hidden");

            return await ReplaceNode(memberAccessExpression.Name, newMemberAccessExpressionName, document);
        }

        private async Task<Document> DiagnosticSeverityInfo(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken c)
        {
            var newMemberAccessExpressionName = SyntaxFactory.ParseName("Info");

            return await ReplaceNode(memberAccessExpression.Name, newMemberAccessExpressionName, document);
        }

        private async Task<Document> DiagnosticSeverityError(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken c)
        {
            var newMemberAccessExpressionName = SyntaxFactory.ParseName("Error");

            return await ReplaceNode(memberAccessExpression.Name, newMemberAccessExpressionName, document);
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
            var newClassDeclaration = CodeFixNodeCreator.NewIdCreator(idToken, expressionKind, classDeclaration);

            return await ReplaceNode(classDeclaration, newClassDeclaration, document);
        }

        private async Task<Document> IdDeclTypeAsync(Document document, LiteralExpressionSyntax literalExpression, CancellationToken c)
        {
            var idName = SyntaxFactory.ParseName(literalExpression.Token.Value.ToString()) as IdentifierNameSyntax;

            return await ReplaceNode(literalExpression, idName, document);
        }

        #endregion

        #region supported diagnostics code fix
        private async Task<Document> IncorrectSigSuppDiagAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            var whiteSpace = SyntaxFactory.Whitespace(" ");
            var newIdentifier = SyntaxFactory.ParseToken("SupportedDiagnostics").WithLeadingTrivia(whiteSpace);
            var publicKeyword = SyntaxFactory.ParseToken("public").WithTrailingTrivia(whiteSpace);
            var overrideKeyword = SyntaxFactory.ParseToken("override").WithTrailingTrivia(whiteSpace);
            var modifierList = SyntaxFactory.TokenList(publicKeyword, overrideKeyword);
            var newPropertyDeclaration = declaration.WithIdentifier(newIdentifier).WithModifiers(modifierList).WithLeadingTrivia(declaration.GetLeadingTrivia()).WithTrailingTrivia(whiteSpace);

            return await ReplaceNode(declaration, newPropertyDeclaration, document);
        }

        private async Task<Document> MissingAccessorAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);
            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            var throwStatement = new[] { generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)) };
            var type = generator.GetType(declaration);
            var newPropertyDeclaration = generator.PropertyDeclaration("SupportedDiagnostics", type,
                Accessibility.Public, DeclarationModifiers.Override, throwStatement) as PropertyDeclarationSyntax;

            newPropertyDeclaration = newPropertyDeclaration.RemoveNode(newPropertyDeclaration.AccessorList.Accessors[1], 0);

            return await ReplaceNode(declaration, newPropertyDeclaration, document);
        }

        private async Task<Document> TooManyAccessorsAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            var allAccessors = declaration.AccessorList.Accessors.OfType<AccessorDeclarationSyntax>();
            bool foundGetAccessor = false;
            AccessorDeclarationSyntax accessorToKeep = null;
            var accessorList = declaration.AccessorList;

            foreach (AccessorDeclarationSyntax accessor in allAccessors)
            {
                var keyword = accessor.Keyword.ValueText;
                if (keyword == "get" && !foundGetAccessor)
                {
                    accessorToKeep = accessor;
                    foundGetAccessor = true;
                }
                else
                {
                    accessorList = accessorList.RemoveNode(accessor, 0);
                }
            }

            if (!foundGetAccessor)
            {
                var newStatements = SyntaxFactory.ParseStatement("");
                var newBody = SyntaxFactory.Block(newStatements);
                accessorToKeep = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, newBody);
                accessorList = accessorList.AddAccessors(accessorToKeep);
            }

            var newPropertyDeclaration = declaration.WithAccessorList(accessorList);

            return await ReplaceNode(declaration, newPropertyDeclaration, document);
        }

        private async Task<Document> AccessorReturnValueAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);
            var expressionString = generator.IdentifierName("ImmutableArray");
            var identifierString = generator.IdentifierName("Create");
            var expression = generator.MemberAccessExpression(expressionString, identifierString);
            var invocationExpression = generator.InvocationExpression(expression);
            var returnStatement = generator.ReturnStatement(invocationExpression) as ReturnStatementSyntax; //SyntaxFactory.ParseStatement("return ImmutableArray.Create();") as ReturnStatementSyntax;

            var firstAccessor = declaration.AccessorList.Accessors.First();
            var oldBody = firstAccessor.Body as BlockSyntax;
            var oldReturnStatement = oldBody.Statements.First();

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root;

            if (oldReturnStatement == null)
            {
                var newAccessorDeclaration = firstAccessor.AddBodyStatements(returnStatement);
                newRoot = root.ReplaceNode(firstAccessor, newAccessorDeclaration);
            }
            else
            {
                newRoot = root.ReplaceNode(oldReturnStatement, returnStatement);
            }
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> SupportedRulesAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            List<string> ruleNames = new List<string>();
            var fieldMembers = declaration.Members.OfType<FieldDeclarationSyntax>();
            foreach (FieldDeclarationSyntax fieldSyntax in fieldMembers)
            {
                var fieldType = fieldSyntax.Declaration.Type;
                if (fieldType != null && fieldType.ToString() == "DiagnosticDescriptor")
                {
                    var ruleName = fieldSyntax.Declaration.Variables[0].Identifier.Text;
                    ruleNames.Add(ruleName);
                }
            }

            var propertyMembers = declaration.Members.OfType<PropertyDeclarationSyntax>();
            foreach (PropertyDeclarationSyntax propertySyntax in propertyMembers)
            {
                if (propertySyntax.Identifier.Text != "SupportedDiagnostics") continue;

                AccessorDeclarationSyntax getAccessor = propertySyntax.AccessorList.Accessors.First();
                var returnStatement = getAccessor.Body.Statements.First() as ReturnStatementSyntax;
                var invocationExpression = returnStatement.Expression as InvocationExpressionSyntax;
                var oldArgumentList = invocationExpression.ArgumentList as ArgumentListSyntax;

                string argumentListString = "";
                foreach (string ruleName in ruleNames)
                {
                    if (ruleName == ruleNames.First()) argumentListString += ruleName;
                    else argumentListString += ", " + ruleName;
                }

                var argumentListSyntax = SyntaxFactory.ParseArgumentList("(" + argumentListString + ")");

                return await ReplaceNode(oldArgumentList, argumentListSyntax, document);
            }

            return document;
        }
        #endregion

        private async Task<Document> IncorrectKeywordAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var block = declaration.Parent as BlockSyntax;
            var ifKeyword = CodeFixNodeCreator.KeywordHelper(generator, block);

            return await ReplaceNode(declaration, ifKeyword, document);
        }
        
        private async Task<Document> TrailingCheckIncorrectAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifBlockStatements = new SyntaxList<StatementSyntax>();
            if (declaration.Body.Statements[2].Kind() == SyntaxKind.IfStatement)
            {
                var ifDeclaration = declaration.Body.Statements[2] as IfStatementSyntax;
                var ifBlock = ifDeclaration.Statement as BlockSyntax;
                ifBlockStatements = ifBlock.Statements;
            }
            StatementSyntax ifStatement = CodeFixNodeCreator.TriviaCheckHelper(generator, declaration.Body, ifBlockStatements) as StatementSyntax;

            var oldBlock = declaration.Body;
            var newBlock = declaration.Body.WithStatements(declaration.Body.Statements.Replace(declaration.Body.Statements[2], ifStatement));

            return await ReplaceNode(oldBlock, newBlock, document);
        }
        
        private async Task<Document> MissingKeywordAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var methodBlock = declaration.Body as BlockSyntax;
            var ifKeyword = CodeFixNodeCreator.KeywordHelper(generator, methodBlock) as StatementSyntax;
            var newBlock = methodBlock.AddStatements(ifKeyword);

            return await ReplaceNode(methodBlock, newBlock, document);
        }

        private async Task<Document> TrailingCheckMissingAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            var ifBlockStatements = new SyntaxList<StatementSyntax>();
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            StatementSyntax ifStatement = CodeFixNodeCreator.TriviaCheckHelper(generator, declaration.Body, ifBlockStatements) as StatementSyntax;

            var oldBlock = declaration.Body;
            var newBlock = declaration.Body.WithStatements(declaration.Body.Statements.Replace(declaration.Body.Statements[2], ifStatement));
            return await ReplaceNode(oldBlock, newBlock, document);
        }
        
        private async Task<Document> TrailingVarMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var localDeclaration = new SyntaxList<SyntaxNode>().Add(CodeFixNodeCreator.TriviaVarMissingHelper(generator, declaration));

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(localDeclaration);

            return await ReplaceNode(oldBlock, newBlock, document);
        }
        
        private async Task<Document> TrailingVarIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var localDeclaration = CodeFixNodeCreator.TriviaVarMissingHelper(generator, declaration) as LocalDeclarationStatementSyntax;

            var oldBlock = declaration.Statement as BlockSyntax;
            var oldStatement = oldBlock.Statements[0];
            var newStatements = oldBlock.Statements.Replace(oldStatement, localDeclaration);
            var newBlock = oldBlock.WithStatements(newStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> TrailingKindCheckIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

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

            var newIfStatement = CodeFixNodeCreator.TriviaKindCheckHelper(generator, ifStatement, ifBlockStatements) as StatementSyntax;

            var oldBlock = ifStatement.Statement as BlockSyntax;
            var oldStatement = oldBlock.Statements[1];
            var newStatements = oldBlock.Statements.Replace(oldStatement, newIfStatement);
            var newBlock = oldBlock.WithStatements(newStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> TrailingKindCheckMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var ifBlockStatements = new SyntaxList<SyntaxNode>();
            var newIfStatement = new SyntaxList<SyntaxNode>().Add(CodeFixNodeCreator.TriviaKindCheckHelper(generator, declaration, ifBlockStatements) as StatementSyntax);

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(newIfStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> WhitespaceCheckIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

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

            var newIfStatement = CodeFixNodeCreator.WhitespaceCheckHelper(generator, ifStatement, ifBlockStatements) as StatementSyntax;

            var oldBlock = ifStatement.Statement as BlockSyntax;
            var oldStatement = oldBlock.Statements[0];
            var newStatement = oldBlock.Statements.Replace(oldStatement, newIfStatement);
            var newBlock = oldBlock.WithStatements(newStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
        }
        
        private async Task<Document> WhitespaceCheckMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var ifBlockStatements = new SyntaxList<SyntaxNode>();
            var newIfStatement = new SyntaxList<SyntaxNode>().Add(CodeFixNodeCreator.WhitespaceCheckHelper(generator, declaration, ifBlockStatements) as StatementSyntax);

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(newIfStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
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

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> ReturnMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);
            var returnStatement = new SyntaxList<SyntaxNode>().Add(generator.ReturnStatement() as ReturnStatementSyntax);

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(returnStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> TooManyStatementsAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            var oldBlock = declaration.Statement as BlockSyntax;
            var onlyStatement = new SyntaxList<StatementSyntax>().Add(oldBlock.Statements[0]);
            var newBlock = oldBlock.WithStatements(onlyStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        class CodeFixNodeCreator
        {
            internal static SyntaxNode IfHelper(SyntaxGenerator generator)
            {
                var type = SyntaxFactory.ParseTypeName("IfStatementSyntax");
                var expression = generator.IdentifierName("context");
                var memberAccessExpression = generator.MemberAccessExpression(expression, "Node");
                var initializer = generator.CastExpression(type, memberAccessExpression);
                var ifStatement = generator.LocalDeclarationStatement("ifStatement", initializer);

                return ifStatement;
            }
            
            internal static SyntaxNode KeywordHelper(SyntaxGenerator generator, BlockSyntax methodBlock)
            {
                var firstStatement = methodBlock.Statements[0] as LocalDeclarationStatementSyntax;
                var variableName = generator.IdentifierName(firstStatement.Declaration.Variables[0].Identifier.ValueText);
                var initializer = generator.MemberAccessExpression(variableName, "IfKeyword");
                var ifKeyword = generator.LocalDeclarationStatement("ifKeyword", initializer);

                return ifKeyword;
            }
            
            internal static SyntaxNode TriviaCheckHelper(SyntaxGenerator generator, BlockSyntax methodBlock, SyntaxList<StatementSyntax> ifBlockStatements)
            {
                var secondStatement = methodBlock.Statements[1] as LocalDeclarationStatementSyntax;

                var variableName = generator.IdentifierName(secondStatement.Declaration.Variables[0].Identifier.ValueText);
                var conditional = generator.MemberAccessExpression(variableName, "HasTrailingTrivia");
                var ifStatement = generator.IfStatement(conditional, ifBlockStatements);

                return ifStatement;
            }

            internal static SyntaxNode TriviaVarMissingHelper(SyntaxGenerator generator, IfStatementSyntax declaration)
            {
                var methodBlock = declaration.Parent as BlockSyntax;
                var secondStatement = methodBlock.Statements[1] as LocalDeclarationStatementSyntax;

                var variableName = generator.IdentifierName(secondStatement.Declaration.Variables[0].Identifier.ValueText);

                var ifTrailing = generator.MemberAccessExpression(variableName, "TrailingTrivia");
                var fullVariable = generator.MemberAccessExpression(ifTrailing, "Last");
                var parameters = new SyntaxList<SyntaxNode>();
                var variableExpression = generator.InvocationExpression(fullVariable, parameters);

                var localDeclaration = generator.LocalDeclarationStatement("trailingTrivia", variableExpression);

                return localDeclaration;
            }

            internal static SyntaxNode TriviaKindCheckHelper(SyntaxGenerator generator, IfStatementSyntax ifStatement, SyntaxList<SyntaxNode> ifBlockStatements)
            {
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

            internal static SyntaxNode WhitespaceCheckHelper(SyntaxGenerator generator, IfStatementSyntax ifStatement, SyntaxList<SyntaxNode> ifBlockStatements)
            {
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

            internal static SyntaxNode BuildInitialize(SyntaxGenerator generator, INamedTypeSymbol notImplementedException)
            {
                var type = SyntaxFactory.ParseTypeName("AnalysisContext");
                var parameters = new[] { generator.ParameterDeclaration("context", type) };

                var statements = new[] { generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)) };
                var initializeDeclaration = generator.MethodDeclaration("Initialize", parameters: parameters, accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, statements: statements);

                return initializeDeclaration;
            }

            internal static ClassDeclarationSyntax NewIdCreator(SyntaxToken idToken, ExpressionSyntax expressionKind, ClassDeclarationSyntax declaration)
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

            //creates the diagnostic location statement
            internal static SyntaxNode CreateLocation(SyntaxGenerator generator, string ifStatementIdentifier, string spanIdentifier)
            {
                string name = "diagnosticLocation";
                SyntaxNode memberIdentifier = generator.IdentifierName("Location");
                SyntaxNode memberName = generator.IdentifierName("Create");
                SyntaxNode expression = generator.MemberAccessExpression(memberIdentifier, memberName);

                SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();

                var treeIdentifier = generator.IdentifierName(ifStatementIdentifier);
                var treeArgExpression = generator.MemberAccessExpression(treeIdentifier, "SyntaxTree");
                var treeArg = generator.Argument(treeArgExpression);

                var spanArgIdentifier = generator.IdentifierName(spanIdentifier);
                var spanArg = generator.Argument(spanArgIdentifier);

                arguments = arguments.Add(treeArg);
                arguments = arguments.Add(spanArg);

                SyntaxNode initializer = generator.InvocationExpression(expression, arguments);
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

                return localDeclaration;
            }

            //creates the diagnostic span statement
            internal static SyntaxNode CreateSpan(SyntaxGenerator generator, string startIdentifier, string endIdentifier)
            {
                string name = "diagnosticSpan";
                SyntaxNode memberIdentifier = generator.IdentifierName("TextSpan");
                SyntaxNode memberName = generator.IdentifierName("FromBounds");
                SyntaxNode expression = generator.MemberAccessExpression(memberIdentifier, memberName);

                SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();

                var startSpanIdentifier = generator.IdentifierName(startIdentifier);
                var endSpanIdentifier = generator.IdentifierName(endIdentifier);

                arguments = arguments.Add(startSpanIdentifier);
                arguments = arguments.Add(endSpanIdentifier);

                SyntaxNode initializer = generator.InvocationExpression(expression, arguments);
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

                return localDeclaration;
            }

            //creates an end or start span statement
            internal static SyntaxNode CreateEndOrStartSpan(SyntaxGenerator generator, string identifierString, string variableName)
            {
                SyntaxNode identifier = null;

                identifier = generator.IdentifierName(identifierString);

                SyntaxNode expression = generator.MemberAccessExpression(identifier, "Span");
                SyntaxNode initializer = generator.MemberAccessExpression(expression, "Start");

                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(variableName, initializer);

                return localDeclaration;
            }

            //creates the open parenthesis statement
            internal static SyntaxNode CreateOpenParen(SyntaxGenerator generator, string expressionString)
            {
                string name = "openParen";

                var expression = generator.IdentifierName(expressionString);

                var initializer = generator.MemberAccessExpression(expression, "OpenParenToken");
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

                return localDeclaration;
            }

            internal static SyntaxNode CreateDiagnostic(SyntaxGenerator generator, string locationName, string ruleName)
            {
                var identifier = generator.IdentifierName("Diagnostic");
                var expression = generator.MemberAccessExpression(identifier, "Create");

                SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();

                var ruleExpression = generator.IdentifierName(ruleName);
                var ruleArg = generator.Argument(ruleExpression);

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

            internal static string GetFirstRuleName(ClassDeclarationSyntax declaration)
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

            internal static MethodDeclarationSyntax GetAnalysis(ClassDeclarationSyntax declaration)
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

            internal static SyntaxNode CreateDiagnosticReport(SyntaxGenerator generator, string argumentName, string contextName)
            {
                var argumentExpression = generator.IdentifierName(argumentName);
                var argument = generator.Argument(argumentExpression);

                var identifier = generator.IdentifierName(contextName);
                var memberExpression = generator.MemberAccessExpression(identifier, "ReportDiagnostic");
                var expression = generator.InvocationExpression(memberExpression, argument);

                SyntaxNode expressionStatement = generator.ExpressionStatement(expression);
                return expressionStatement;
            }
        }
    }
}