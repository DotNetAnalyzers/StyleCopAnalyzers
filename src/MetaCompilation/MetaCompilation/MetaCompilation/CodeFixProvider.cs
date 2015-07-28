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
        public const string MessagePrefix = "T: ";

        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(MetaCompilationAnalyzer.MissingId,
                                             MetaCompilationAnalyzer.MissingInit,
                                             MetaCompilationAnalyzer.MissingRegisterStatement,
                                             MetaCompilationAnalyzer.TooManyInitStatements,
                                             MetaCompilationAnalyzer.IncorrectInitSig,
                                             MetaCompilationAnalyzer.InvalidStatement,
                                             MetaCompilationAnalyzer.MissingAnalysisMethod,
                                             MetaCompilationAnalyzer.IncorrectAnalysisAccessibility,
                                             MetaCompilationAnalyzer.IncorrectAnalysisParameter,
                                             MetaCompilationAnalyzer.IncorrectAnalysisReturnType,
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
                                             MetaCompilationAnalyzer.DiagnosticMissing,
                                             MetaCompilationAnalyzer.DiagnosticIncorrect,
                                             MetaCompilationAnalyzer.DiagnosticReportIncorrect,
                                             MetaCompilationAnalyzer.DiagnosticReportMissing,
                                             MetaCompilationAnalyzer.TrailingTriviaKindCheckMissing,
                                             MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                                             MetaCompilationAnalyzer.MissingSuppDiag,
                                             MetaCompilationAnalyzer.IncorrectKind,
                                             MetaCompilationAnalyzer.IncorrectRegister,
                                             MetaCompilationAnalyzer.IncorrectArguments,
                                             MetaCompilationAnalyzer.TrailingTriviaCountMissing,
                                             MetaCompilationAnalyzer.TrailingTriviaCountIncorrect,
                                             MetaCompilationAnalyzer.IdStringLiteral);
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

                switch (diagnostic.Id)
                {
                    case MetaCompilationAnalyzer.MissingId:
                        IEnumerable<ClassDeclarationSyntax> idDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                        if (idDeclarations.Count() != 0)
                        {
                            ClassDeclarationSyntax declaration = idDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Give the diagnostic a unique string ID distinguishing it from other diagnostics", c => MissingIdAsync(context.Document, declaration, c), "Give the diagnostic a unique string ID distinguishing it from other diagnostics"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.MissingInit:
                        IEnumerable<ClassDeclarationSyntax> initDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                        if (initDeclarations.Count() != 0)
                        {
                            ClassDeclarationSyntax declaration = initDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Insert the missing Initialize method", c => MissingInitAsync(context.Document, declaration, c), "Insert the missing Initialize method"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.MissingRegisterStatement:
                        IEnumerable<MethodDeclarationSyntax> registerDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (registerDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = registerDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Register an action to analyze code when changes occur", c => MissingRegisterAsync(context.Document, declaration, c), "Register an action to analyze code when changes occur"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TooManyInitStatements:
                        IEnumerable<MethodDeclarationSyntax> manyDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (manyDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = manyDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Remove multiple registered actions from the Initialize method", c => MultipleStatementsAsync(context.Document, declaration, c), "Remove multiple registered actions from the Initialize method"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.InvalidStatement:
                        IEnumerable<StatementSyntax> invalidDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (invalidDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = invalidDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Remove invalid statements from the Initialize method", c => InvalidStatementAsync(context.Document, declaration, c), "Remove invalid statements from the Initialize method"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.MissingAnalysisMethod:
                        IEnumerable<MethodDeclarationSyntax> analysisDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (analysisDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = analysisDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Generate the method called by actions registered in Initialize", c => MissingAnalysisMethodAsync(context.Document, declaration, c), "Generate the method called by actions registered in Initialize"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IncorrectAnalysisAccessibility:
                        IEnumerable<MethodDeclarationSyntax> incorrectDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (incorrectDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = incorrectDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Add the private keyword to this method", c => IncorrectAnalysisAccessibilityAsync(context.Document, declaration, c), "Add the private keyword to this method"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IncorrectAnalysisReturnType:
                        IEnumerable<MethodDeclarationSyntax> returnDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (returnDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = returnDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Declare a void return type for this method", c => IncorrectAnalysisReturnTypeAsync(context.Document, declaration, c), "Declare a void return type for this method"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IncorrectAnalysisParameter:
                        IEnumerable<MethodDeclarationSyntax> parameterDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (parameterDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = parameterDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Have this method take one parameter of type SyntaxNodeAnalysisContext", c => IncorrectAnalysisParameterAsync(context.Document, declaration, c), "Have this method take one parameter of type SyntaxNodeAnalysisContext"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.InternalAndStaticError:
                        IEnumerable<FieldDeclarationSyntax> internalDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>();
                        if (internalDeclarations.Count() != 0)
                        {
                            FieldDeclarationSyntax declaration = internalDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Make the DiagnosticDescriptor rule both internal and static", c => InternalStaticAsync(context.Document, declaration, c), "Make the DiagnosticDescriptor rule both internal and static"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.EnabledByDefaultError:
                        IEnumerable<ArgumentSyntax> enabledDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ArgumentSyntax>();
                        if (enabledDeclarations.Count() != 0)
                        {
                            ArgumentSyntax declaration = enabledDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Set 'isEnabledByDefault' parameter to true", c => EnabledByDefaultAsync(context.Document, declaration, c), "Set 'isEnabledByDefault' parameter to true"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.DefaultSeverityError:
                        IEnumerable<ArgumentSyntax> severityDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ArgumentSyntax>();
                        if (severityDeclarations.Count() != 0)
                        {
                            ArgumentSyntax declaration = severityDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Set the severity to 'Error' if something is not allowed", c => DiagnosticSeverityError(context.Document, declaration, c), "Set the severity to 'Error' if something is not allowed"), diagnostic);
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Set the severity to 'Warning' if something is suspicious but allowed", c => DiagnosticSeverityWarning(context.Document, declaration, c), "Set the severity to 'Warning' if something is suspicious but allowed"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.MissingIdDeclaration:
                        IEnumerable<VariableDeclaratorSyntax> missingIdDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>();
                        if (missingIdDeclarations.Count() != 0)
                        {
                            VariableDeclaratorSyntax declaration = missingIdDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Declare the diagnostic ID as a public constant string", c => MissingIdDeclarationAsync(context.Document, declaration, c), "Declare the diagnostic ID as a public constant string"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IdStringLiteral:
                    case MetaCompilationAnalyzer.IdDeclTypeError:
                        IEnumerable<ClassDeclarationSyntax> declDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                        if (declDeclarations.Count() != 0)
                        {
                            ClassDeclarationSyntax classDeclaration = declDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Declare the diagnostic ID as a public constant string", c => IdDeclTypeAsync(context.Document, classDeclaration, c), "Declare the diagnostic ID as a public constant string"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IfStatementIncorrect:
                        IEnumerable<StatementSyntax> ifDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (ifDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = ifDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the IfStatementSyntax Node from the context", c => IncorrectIfAsync(context.Document, declaration, c), "Extract the IfStatementSyntax Node from the context"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IfStatementMissing:
                        IEnumerable<MethodDeclarationSyntax> ifMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (ifMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = ifMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the IfStatementSyntax Node from the context", c => MissingIfAsync(context.Document, declaration, c), "Extract the IfStatementSyntax Node from the context"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IncorrectInitSig:
                        IEnumerable<MethodDeclarationSyntax> initSigDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (initSigDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = initSigDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Implement the correct signature for the Initialize method", c => IncorrectSigAsync(context.Document, declaration, c), "Implement the correct signature for the Initialize method"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IfKeywordIncorrect:
                        IEnumerable<StatementSyntax> keywordDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (keywordDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = keywordDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the if-keyword from the if-statement", c => IncorrectKeywordAsync(context.Document, declaration, c), "Extract the if-keyword from the if-statement"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IfKeywordMissing:
                        IEnumerable<MethodDeclarationSyntax> keywordMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (keywordMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = keywordMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the if-keyword from the if-statement", c => MissingKeywordAsync(context.Document, declaration, c), "Extract the if-keyword from the if-statement"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect:
                        IEnumerable<MethodDeclarationSyntax> checkDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (checkDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = checkDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check if the if-keyword has trailing trivia", c => TrailingCheckIncorrectAsync(context.Document, declaration, c), "Check if the if-keyword has trailing trivia"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaCheckMissing:
                        IEnumerable<MethodDeclarationSyntax> checkMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (checkMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = checkMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check if the if-keyword has trailing trivia", c => TrailingCheckMissingAsync(context.Document, declaration, c), "Check if the if-keyword has trailing trivia"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaVarMissing:
                        IEnumerable<IfStatementSyntax> varMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (varMissingDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = varMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the last trailing trivia into a variable", c => TrailingVarMissingAsync(context.Document, declaration, c), "Extract the last trailing trivia into a variable"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaVarIncorrect:
                        IEnumerable<IfStatementSyntax> varDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (varDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = varDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the last trailing trivia into a variable", c => TrailingVarIncorrectAsync(context.Document, declaration, c), "Extract the last trailing trivia into a variable"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect:
                        IEnumerable<IfStatementSyntax> kindDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (kindDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = kindDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check the kind of the last trailing trivia", c => TrailingKindCheckIncorrectAsync(context.Document, declaration, c), "Check the kind of the last trailing trivia"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaKindCheckMissing:
                        IEnumerable<IfStatementSyntax> kindMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (kindMissingDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = kindMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check the kind of the last trailing trivia", c => TrailingKindCheckMissingAsync(context.Document, declaration, c), "Check the kind of the last trailing trivia"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.WhitespaceCheckIncorrect:
                        IEnumerable<IfStatementSyntax> whitespaceDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (whitespaceDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = whitespaceDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check if the whitespace is a single space", c => WhitespaceCheckIncorrectAsync(context.Document, declaration, c), "Check if the whitespace is a single space"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.ReturnStatementIncorrect:
                        IEnumerable<IfStatementSyntax> statementDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (statementDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = statementDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Return from the method", c => ReturnIncorrectAsync(context.Document, declaration, c), "Return from the method"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.ReturnStatementMissing:
                        IEnumerable<IfStatementSyntax> statementMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (statementMissingDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = statementMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Return from the method", c => ReturnMissingAsync(context.Document, declaration, c), "Return from the method"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.WhitespaceCheckMissing:
                        IEnumerable<IfStatementSyntax> whitespaceMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (whitespaceMissingDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = whitespaceMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check if the whitespace is a single space", c => WhitespaceCheckMissingAsync(context.Document, declaration, c), "Check if the whitespace is a single space"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.LocationMissing:
                        IEnumerable<MethodDeclarationSyntax> locationMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (locationMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = locationMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a diagnostic location", c => AddLocationAsync(context.Document, declaration, c), "Create a diagnostic location"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.LocationIncorrect:
                        IEnumerable<StatementSyntax> locationDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (locationDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = locationDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a diagnostic location", c => ReplaceLocationAsync(context.Document, declaration, c), "Create a diagnostic location"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.SpanMissing:
                        IEnumerable<MethodDeclarationSyntax> spanMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (spanMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = spanMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a diagnostic span", c => AddSpanAsync(context.Document, declaration, c), "Create a diagnostic span"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.SpanIncorrect:
                        IEnumerable<StatementSyntax> spanDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (spanDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = spanDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a diagnostic span", c => ReplaceSpanAsync(context.Document, declaration, c), "Create a diagnostic span"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.EndSpanMissing:
                        IEnumerable<MethodDeclarationSyntax> endMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (endMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = endMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a variable representing the end of the diagnostic span", c => AddEndSpanAsync(context.Document, declaration, c), "Create a variable representing the end of the diagnostic span"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.EndSpanIncorrect:
                        IEnumerable<StatementSyntax> endDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (endDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = endDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a variable representing the end of the diagnostic span", c => ReplaceEndSpanAsync(context.Document, declaration, c), "Create a variable representing the end of the diagnostic span"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.StartSpanMissing:
                        IEnumerable<MethodDeclarationSyntax> startMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (startMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = startMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a variable representing the start of the diagnostic span", c => AddStartSpanAsync(context.Document, declaration, c), "Create a variable representing the start of the diagnostic span"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.StartSpanIncorrect:
                        IEnumerable<StatementSyntax> startDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (startDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = startDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a variable representing the start of the diagnostic span", c => ReplaceStartSpanAsync(context.Document, declaration, c), "Create a variable representing the start of the diagnostic span"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.OpenParenMissing:
                        IEnumerable<MethodDeclarationSyntax> openMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (openMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = openMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the open parenthesis from the if-statement", c => AddOpenParenAsync(context.Document, declaration, c), "Extract the open parenthesis from the if-statement"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.OpenParenIncorrect:
                        IEnumerable<StatementSyntax> openDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (openDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = openDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the open parenthesis from the if-statement", c => ReplaceOpenParenAsync(context.Document, declaration, c), "Extract the open parenthesis from the if-statement"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.DiagnosticMissing:
                        IEnumerable<ClassDeclarationSyntax> diagnosticMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                        if (diagnosticMissingDeclarations.Count() != 0)
                        {
                            ClassDeclarationSyntax declaration = diagnosticMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create the diagnostic that is going to be reported", c => AddDiagnosticAsync(context.Document, declaration, c), "Create the diagnostic that is going to be reported"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.DiagnosticIncorrect:
                        IEnumerable<StatementSyntax> diagnosticDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (diagnosticDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = diagnosticDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create the diagnostic that is going to be reported", c => ReplaceDiagnosticAsync(context.Document, declaration, c), "Create the diagnostic that is going to be reported"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.DiagnosticReportMissing:
                        IEnumerable<MethodDeclarationSyntax> reportMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (reportMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = reportMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Report the diagnostic to the SyntaxNodeAnalysisContext", c => AddDiagnosticReportAsync(context.Document, declaration, c), "Report the diagnostic to the SyntaxNodeAnalysisContext"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.DiagnosticReportIncorrect:
                        IEnumerable<StatementSyntax> reportDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                        if (reportDeclarations.Count() != 0)
                        {
                            StatementSyntax declaration = reportDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Report the diagnostic to the SyntaxNodeAnalysisContext", c => ReplaceDiagnosticReportAsync(context.Document, declaration, c), "Report the diagnostic to the SyntaxNodeAnalysisContext"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IncorrectSigSuppDiag:
                        IEnumerable<PropertyDeclarationSyntax> sigSuppDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>();
                        if (sigSuppDeclarations.Count() != 0)
                        {
                            PropertyDeclarationSyntax declaration = sigSuppDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Implement the correct signature for the SupportedDiagnostics property", c => IncorrectSigSuppDiagAsync(context.Document, declaration, c), "Implement the correct signature for the SupportedDiagnostics property"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.MissingAccessor:
                        IEnumerable<PropertyDeclarationSyntax> accessorDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>();
                        if (accessorDeclarations.Count() != 0)
                        {
                            PropertyDeclarationSyntax declaration = accessorDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Add a get-accessor to the SupportedDiagnostics property", c => MissingAccessorAsync(context.Document, declaration, c), "Add a get-accessor to the SupportedDiagnostics property"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TooManyAccessors:
                        IEnumerable<PropertyDeclarationSyntax> manyAccessorsDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>();
                        if (manyAccessorsDeclarations.Count() != 0)
                        {
                            PropertyDeclarationSyntax declaration = manyAccessorsDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Remove unnecessary accessors from the SupportedDiagnostics property", c => TooManyAccessorsAsync(context.Document, declaration, c), "Remove unnecessary accessors from the SupportedDiagnostics property"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.SuppDiagReturnValue:
                    case MetaCompilationAnalyzer.IncorrectAccessorReturn:
                        IEnumerable<PropertyDeclarationSyntax> incorrectAccessorDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>();
                        if (incorrectAccessorDeclarations.Count() != 0)
                        {
                            PropertyDeclarationSyntax declaration = incorrectAccessorDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Return an ImmutableArray of DiagnosticDescriptors from SupportedDiagnostics", c => AccessorReturnValueAsync(context.Document, declaration, c), "Return an ImmutableArray of DiagnosticDescriptors from SupportedDiagnostics"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.SupportedRules:
                        IEnumerable<ClassDeclarationSyntax> rulesDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                        if (rulesDeclarations.Count() != 0)
                        {
                            ClassDeclarationSyntax declaration = rulesDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Return a list of all diagnostics that can be reported by this analyzer", c => SupportedRulesAsync(context.Document, declaration, c), "Return a list of all diagnostics that can be reported by this analyzer"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.MissingSuppDiag:
                        IEnumerable<ClassDeclarationSyntax> suppDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                        if (suppDeclarations.Count() != 0)
                        {
                            ClassDeclarationSyntax declaration = suppDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Add the required SupportedDiagnostics property", c => AddSuppDiagAsync(context.Document, declaration, c), "Add the required SupportedDiagnostics property"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.MissingRule:
                        IEnumerable<ClassDeclarationSyntax> missingRuleDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                        if (missingRuleDeclarations.Count() != 0)
                        {
                            ClassDeclarationSyntax declaration = missingRuleDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Add a DiagnosticDescriptor rule to create the diagnostic", c => AddRuleAsync(context.Document, declaration, c), "Add a DiagnosticDescriptor rule to create the diagnostic"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IncorrectKind:
                        IEnumerable<ArgumentListSyntax> incorrectKindDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ArgumentListSyntax>();
                        if (incorrectKindDeclarations.Count() != 0)
                        {
                            ArgumentListSyntax declaration = incorrectKindDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Analyze the correct SyntaxKind", c => CorrectKindAsync(context.Document, declaration, c), "Analyze the correct SyntaxKind"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IncorrectRegister:
                        IEnumerable<IdentifierNameSyntax> incorrectRegisterDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IdentifierNameSyntax>();
                        if (incorrectRegisterDeclarations.Count() != 0)
                        {
                            IdentifierNameSyntax declaration = incorrectRegisterDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Register an action of kind SyntaxNodeAnalysis", c => CorrectRegisterAsync(context.Document, declaration, c), "Register an action of kind SyntaxNodeAnalysis"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IncorrectArguments:
                        IEnumerable<InvocationExpressionSyntax> argsDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>();
                        if (argsDeclarations.Count() != 0)
                        {
                            InvocationExpressionSyntax declaration = argsDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Add the correct arguments to the Initialize method", c => CorrectArgumentsAsync(context.Document, declaration, c), "Add the correct arguments to the Initialize method"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaCountMissing:
                        IEnumerable<MethodDeclarationSyntax> countMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (countMissingDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = countMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check the amount of trailing trivia", c => TriviaCountMissingAsync(context.Document, declaration, c), "Check the amount of trailing trivia"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaCountIncorrect:
                        IEnumerable<MethodDeclarationSyntax> countIncorrectDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                        if (countIncorrectDeclarations.Count() != 0)
                        {
                            MethodDeclarationSyntax declaration = countIncorrectDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check the amount of trailing trivia", c => TriviaCountIncorrectAsync(context.Document, declaration, c), "Check the amount of trailing trivia"), diagnostic);
                        }
                        break;
                }
            }
        }

        //sets the analysis method to take a parameter of type SyntaxNodeAnalysisContext
        private async Task<Document> IncorrectAnalysisParameterAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var newDeclaration = CodeFixHelper.CreateMethodWithContextParameter(generator, declaration);

            return await ReplaceNode(declaration, newDeclaration, document);
        }

        private async Task<Document> IncorrectAnalysisReturnTypeAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string methodName = declaration.Identifier.Text;
            var parameters = declaration.ParameterList.Parameters;
            var returnType = SyntaxFactory.ParseTypeName("void");
            var statements = declaration.Body.Statements;

            var newDeclaration = generator.MethodDeclaration(methodName, parameters, returnType: returnType, accessibility: Accessibility.Private, statements: statements);

            return await ReplaceNode(declaration, newDeclaration, document);
        }

        private async Task<Document> IncorrectAnalysisAccessibilityAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            MethodDeclarationSyntax newDeclaration = generator.WithAccessibility(declaration, Accessibility.Private) as MethodDeclarationSyntax;

            return await ReplaceNode(declaration, newDeclaration, document);
        }

        private async Task<Document> MissingAnalysisMethodAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var statements = declaration.Body.Statements.First() as ExpressionStatementSyntax;
            var invocationExpression = statements.Expression as InvocationExpressionSyntax;
            string methodName = invocationExpression.ArgumentList.Arguments[0].ToString();

            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            SyntaxNode newAnalysisMethod = CodeFixHelper.CreateAnalysisMethod(generator, methodName, semanticModel);

            ClassDeclarationSyntax classDeclaration = declaration.Parent as ClassDeclarationSyntax;
            ClassDeclarationSyntax newClassDecl = classDeclaration as ClassDeclarationSyntax;

            if (newAnalysisMethod != null)
            {
                newClassDecl = generator.AddMembers(classDeclaration, newAnalysisMethod) as ClassDeclarationSyntax;
            }

            return await ReplaceNode(classDeclaration, newClassDecl, document);
        }

        private async Task<Document> CorrectArgumentsAsync(Document document, InvocationExpressionSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string methodName = "AnalyzeIfStatement";
            bool useExistingAnalysis = false;

            var argList = declaration.ArgumentList;
            if (argList != null)
            {
                var args = argList.Arguments;
                if (args != null)
                {
                    if (args.Count > 0)
                    {
                        var nameArg = args[0];
                        var name = nameArg.Expression as IdentifierNameSyntax;
                        if (name != null)
                        {
                            methodName = name.Identifier.Text;
                        }
                        else
                        {
                            useExistingAnalysis = true;
                        }
                    }
                    else if (args.Count == 0)
                    {
                        useExistingAnalysis = true;
                    }
                }
            }
            if (useExistingAnalysis)
            {
                ClassDeclarationSyntax classDeclaration = declaration.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
                methodName = CodeFixHelper.ExistingAnalysisMethod(classDeclaration);
            }
            if (methodName == null)
            {
                methodName = "AnalyzeIfStatement";
            }

            SyntaxNode statement = CodeFixHelper.CreateRegister(generator, declaration.Parent.Parent.Parent as MethodDeclarationSyntax, methodName);
            SyntaxNode expression = generator.ExpressionStatement(statement);

            return await ReplaceNode(declaration.Parent, expression.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> CorrectRegisterAsync(Document document, IdentifierNameSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode newRegister = generator.IdentifierName("RegisterSyntaxNodeAction");
            SyntaxNode newMemberExpr = generator.MemberAccessExpression((declaration.Parent as MemberAccessExpressionSyntax).Expression, newRegister);
            SyntaxNode newInvocationExpr = generator.InvocationExpression(newMemberExpr, ((declaration.Parent as MemberAccessExpressionSyntax).Parent as InvocationExpressionSyntax).ArgumentList.Arguments);
            SyntaxNode newExpression = generator.ExpressionStatement(newInvocationExpr);

            return await ReplaceNode(declaration.FirstAncestorOrSelf<ExpressionStatementSyntax>(), newExpression.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> CorrectKindAsync(Document document, ArgumentListSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            ArgumentSyntax argument = CodeFixHelper.CreateSyntaxKindIfStatement(generator);
            SeparatedSyntaxList<ArgumentSyntax> arguments = declaration.Arguments;
            if (arguments.Count < 2)
            {
                arguments = arguments.Add(argument);
            }
            else
            {
                arguments = arguments.Replace(arguments[1], argument);
            }

            var argList = SyntaxFactory.ArgumentList(arguments);

            SyntaxNode newRegister = generator.IdentifierName("RegisterSyntaxNodeAction");
            SyntaxNode newMemberExpr = generator.MemberAccessExpression(((declaration.Parent as InvocationExpressionSyntax).Expression as MemberAccessExpressionSyntax).Expression, newRegister);
            SyntaxNode newInvocationExpr = generator.InvocationExpression(newMemberExpr, argList.Arguments);
            SyntaxNode newExpression = generator.ExpressionStatement(newInvocationExpr);

            return await ReplaceNode(declaration.Ancestors().OfType<ExpressionStatementSyntax>().First(), newExpression.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.Whitespace("            "), SyntaxFactory.ParseLeadingTrivia("// Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("            "))), document);
        }

        private async Task<Document> AddRuleAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxList<MemberDeclarationSyntax> members = declaration.Members;
            PropertyDeclarationSyntax insertPoint = null;

            foreach (MemberDeclarationSyntax member in members)
            {
                insertPoint = member as PropertyDeclarationSyntax;
                if (insertPoint == null || insertPoint.Identifier.Text != "SupportedDiagnostics")
                {
                    insertPoint = null;
                    continue;
                }
                else
                {
                    break;
                }
            }

            SyntaxNode insertPointNode = insertPoint as SyntaxNode;

            FieldDeclarationSyntax fieldDeclaration = CodeFixHelper.CreateEmptyRule(generator);

            var newNode = new SyntaxList<SyntaxNode>();
            newNode = newNode.Add(fieldDeclaration.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseLeadingTrivia("// If the analyzer finds an issue, it will report the DiagnosticDescriptor rule").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "))));

            var root = await document.GetSyntaxRootAsync();
            if (insertPointNode != null)
            {
                var newRoot = root.InsertNodesBefore(insertPointNode, newNode);
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
            else
            {
                var newRoot = root.ReplaceNode(declaration, declaration.AddMembers(fieldDeclaration.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// If the analyzer finds an issue, it will report the DiagnosticDescriptor rule").ElementAt(0), SyntaxFactory.EndOfLine("\r\n")))));
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
        }

        private async Task<Document> AddSuppDiagAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxList<MemberDeclarationSyntax> members = declaration.Members;
            MethodDeclarationSyntax insertPoint = null;

            foreach (MemberDeclarationSyntax member in members)
            {
                insertPoint = member as MethodDeclarationSyntax;
                if (insertPoint == null || insertPoint.Identifier.Text != "Initialize")
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            SyntaxNode insertPointNode = insertPoint as SyntaxNode;

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var semanticModel = await document.GetSemanticModelAsync();

            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            PropertyDeclarationSyntax propertyDeclaration = CodeFixHelper.CreateSupportedDiagnostics(generator, notImplementedException);

            var newNodes = new SyntaxList<SyntaxNode>();
            newNodes = newNodes.Add(propertyDeclaration);

            var root = await document.GetSyntaxRootAsync();
            if (insertPoint != null)
            {
                var newRoot = root.InsertNodesBefore(insertPointNode, newNodes);
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
            else
            {
                var newRoot = root.ReplaceNode(declaration, declaration.AddMembers(propertyDeclaration));
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
        }

        // replaces a node in the document
        private async Task<Document> ReplaceNode(SyntaxNode oldNode, SyntaxNode newNode, Document document)
        {
            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(oldNode, newNode);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private async Task<Document> ReplaceDiagnosticReportAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string argumentName = (methodDeclaration.Body.Statements[8] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string contextName = (methodDeclaration.ParameterList.Parameters[0].Identifier.Text);

            SyntaxNode diagnosticReport = CodeFixHelper.CreateDiagnosticReport(generator, argumentName, contextName);

            return await ReplaceNode(declaration, diagnosticReport.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Sends diagnostic information to the IDE to be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> AddDiagnosticReportAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string argumentName = (declaration.Body.Statements[8] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string contextName = (declaration.ParameterList.Parameters[0].Identifier.Text);
            SyntaxNode diagnosticReport = CodeFixHelper.CreateDiagnosticReport(generator, argumentName, contextName);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(diagnosticReport.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Sends diagnostic information to the IDE to be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        private async Task<Document> ReplaceDiagnosticAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            var classDeclaration = methodDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().First();

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string locationName = (methodDeclaration.Body.Statements[7] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string ruleName = CodeFixHelper.GetFirstRuleName(classDeclaration);

            var diagnostic = CodeFixHelper.CreateDiagnostic(generator, locationName, ruleName);

            return await ReplaceNode(declaration, diagnostic.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Holds the diagnostic and all necessary information to be reported").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> AddDiagnosticAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string ruleName = CodeFixHelper.GetFirstRuleName(declaration);
            MethodDeclarationSyntax analysis = CodeFixHelper.GetAnalysis(declaration);
            string locationName = (analysis.Body.Statements[7] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            var diagnostic = CodeFixHelper.CreateDiagnostic(generator, locationName, ruleName);

            var oldStatements = (SyntaxList<SyntaxNode>)analysis.Body.Statements;
            var newStatements = oldStatements.Add(diagnostic.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Holds the diagnostic and all necessary information to be reported").ElementAt(0), SyntaxFactory.EndOfLine(" \r\n"))));
            var newMethod = generator.WithStatements(analysis, newStatements);

            return await ReplaceNode(analysis, newMethod, document);
        }

        private async Task<Document> ReplaceOpenParenAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string expressionString = (methodDeclaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode openParen = CodeFixHelper.CreateOpenParen(generator, expressionString);

            return await ReplaceNode(declaration, openParen.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Extracts the opening parenthesis of the if-statement condition").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> AddOpenParenAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string expressionString = (declaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode openParen = CodeFixHelper.CreateOpenParen(generator, expressionString);

            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(openParen.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Extracts the opening parenthesis of the if-statement condition").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        private async Task<Document> ReplaceStartSpanAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string identifierString = (methodDeclaration.Body.Statements[1] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode startSpan = CodeFixHelper.CreateEndOrStartSpan(generator, identifierString, "startDiagnosticSpan");

            return await ReplaceNode(declaration, startSpan.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Determines the start of the span of the diagnostic that will be reported, ie the start of the squiggle").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> AddStartSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string identifierString = (declaration.Body.Statements[1] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode startSpan = CodeFixHelper.CreateEndOrStartSpan(generator, identifierString, "startDiagnosticSpan");
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(startSpan.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Determines the start of the span of the diagnostic that will be reported, ie the start of the squiggle").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        private async Task<Document> ReplaceEndSpanAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string identifierString = (methodDeclaration.Body.Statements[3] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode endSpan = CodeFixHelper.CreateEndOrStartSpan(generator, identifierString, "endDiagnosticSpan");

            return await ReplaceNode(declaration, endSpan.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Determines the end of the span of the diagnostic that will be reported").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> AddEndSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string identifierString = (declaration.Body.Statements[3] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode endSpan = CodeFixHelper.CreateEndOrStartSpan(generator, identifierString, "endDiagnosticSpan");
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(endSpan.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Determines the end of the span of the diagnostic that will be reported").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        private async Task<Document> ReplaceSpanAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string startIdentifier = (methodDeclaration.Body.Statements[4] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string endIdentifier = (methodDeclaration.Body.Statements[5] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode span = CodeFixHelper.CreateSpan(generator, startIdentifier, endIdentifier);

            return await ReplaceNode(declaration, span.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// The span is the range of integers that define the position of the characters the red squiggle will underline").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> AddSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string startIdentifier = (declaration.Body.Statements[4] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string endIdentifier = (declaration.Body.Statements[5] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode span = CodeFixHelper.CreateSpan(generator, startIdentifier, endIdentifier);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(span.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// The span is the range of integers that define the position of the characters the red squiggle will underline").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        private async Task<Document> ReplaceLocationAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string ifStatementIdentifier = (methodDeclaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string spanIdentifier = (methodDeclaration.Body.Statements[6] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;

            SyntaxNode location = CodeFixHelper.CreateLocation(generator, ifStatementIdentifier, spanIdentifier);

            return await ReplaceNode(declaration, location.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Uses the span created above to create a location for the diagnostic squiggle to appear within the syntax tree passed in as an argument").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> AddLocationAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string ifStatementIdentifier = (declaration.Body.Statements[0] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            string spanIdentifier = (declaration.Body.Statements[6] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
            SyntaxNode location = CodeFixHelper.CreateLocation(generator, ifStatementIdentifier, spanIdentifier);
            var oldStatements = (SyntaxList<SyntaxNode>)declaration.Body.Statements;
            var newStatements = oldStatements.Add(location.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Uses the span created above to create a location for the diagnostic squiggle to appear within the syntax tree passed in as an argument").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            var newMethod = generator.WithStatements(declaration, newStatements);

            return await ReplaceNode(declaration, newMethod, document);
        }

        #region id code fix
        private async Task<Document> MissingIdAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var expressionKind = SyntaxFactory.ParseExpression("\"IfSpacing001\"") as ExpressionSyntax;

            var newField = CodeFixHelper.NewIdCreator(generator, "spacingRuleId", expressionKind).WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs").ElementAt(0), SyntaxFactory.EndOfLine("\r\n")));
            var newClass = generator.InsertMembers(declaration, 0, newField) as ClassDeclarationSyntax;
            var triviaClass = newClass.ReplaceNode(newClass.Members[0], newField);

            return await ReplaceNode(declaration, triviaClass, document);
        }
        #endregion

        #region initialize code fix
        // adds the Initialize method
        private async Task<Document> MissingInitAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SemanticModel semanticModel = await document.GetSemanticModelAsync();

            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();
            string name = "context";
            var initializeDeclaration = CodeFixHelper.BuildInitialize(generator, notImplementedException, statements, name);
            var newClassDeclaration = generator.AddMembers(declaration, initializeDeclaration);

            return await ReplaceNode(declaration, newClassDeclaration, document);
        }

        private async Task<Document> MissingRegisterAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            ClassDeclarationSyntax classDeclaration = declaration.Parent as ClassDeclarationSyntax;
            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            bool newAnalysisRequired = true;

            string methodName = CodeFixHelper.ExistingAnalysisMethod(classDeclaration);

            if (methodName == null)
            { 
                methodName = "AnalyzeIfStatement";
            }
            else
            {
                newAnalysisRequired = false;
            }

            SyntaxNode invocationExpression = CodeFixHelper.CreateRegister(generator, declaration, methodName);
            SyntaxList<SyntaxNode> statements = new SyntaxList<SyntaxNode>().Add(invocationExpression.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            SyntaxNode newInitializeMethod = generator.MethodDeclaration("Initialize", declaration.ParameterList.Parameters, accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, statements: statements);
            ClassDeclarationSyntax newClassDecl = classDeclaration.ReplaceNode(declaration, newInitializeMethod);

            if (newAnalysisRequired)
            {
            SyntaxNode newAnalysisMethod = CodeFixHelper.CreateAnalysisMethod(generator, methodName, semanticModel);
            newClassDecl = generator.AddMembers(newClassDecl, newAnalysisMethod) as ClassDeclarationSyntax;
            }

            return await ReplaceNode(classDeclaration, newClassDecl, document);
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
                if (argumentMethod.Identifier == null || argumentKind.Name == null || preArgumentKind.Identifier == null || argumentKind.Name.Identifier.Text != "IfStatement" ||
                    preArgumentKind.Identifier.ValueText != "SyntaxKind")
                {
                    continue;
                }

                statements = statements.Add(statement);
            }

            SyntaxList<StatementSyntax> statementsToAdd = new SyntaxList<StatementSyntax>();
            statementsToAdd = statementsToAdd.Add(statements[0]);

            newBlock = newBlock.WithStatements(statementsToAdd);
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

            SyntaxNode initializeDeclaration = null;
            SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();
            string name = declaration.ParameterList.Parameters[0].Identifier.ValueText;
            statements = declaration.Body.Statements;
            initializeDeclaration = CodeFixHelper.BuildInitialize(generator, null, statements, name);

            return await ReplaceNode(declaration, initializeDeclaration, document);
        }

        private async Task<Document> IncorrectIfAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            MethodDeclarationSyntax methodDeclaration = declaration.Parent.Parent as MethodDeclarationSyntax;
            string name = methodDeclaration.ParameterList.Parameters[0].Identifier.ValueText as string;
            var ifStatement = CodeFixHelper.IfHelper(generator, name);

            return await ReplaceNode(declaration, ifStatement.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> MissingIfAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string name = declaration.ParameterList.Parameters[0].Identifier.ValueText as string;
            StatementSyntax ifStatement = CodeFixHelper.IfHelper(generator, name) as StatementSyntax;

            var oldBlock = declaration.Body as BlockSyntax;
            var newBlock = oldBlock.AddStatements(ifStatement.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(oldBlock, newBlock, document);
        }
        #endregion

        #region rule code fix
        private async Task<Document> InternalStaticAsync(Document document, FieldDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            var newFieldDecl = generator.FieldDeclaration(declaration.Declaration.Variables[0].Identifier.Text, generator.IdentifierName("DiagnosticDescriptor"), accessibility: Accessibility.Internal, modifiers: DeclarationModifiers.Static, initializer: declaration.Declaration.Variables[0].Initializer.Value as SyntaxNode).WithLeadingTrivia(declaration.GetLeadingTrivia()).WithTrailingTrivia(declaration.GetTrailingTrivia());
            return await ReplaceNode(declaration, newFieldDecl, document);
        }

        private async Task<Document> EnabledByDefaultAsync(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var newRule = rule.ReplaceNode(argument.Expression, generator.LiteralExpression(true));

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// isEnabledByDefault: Determines whether the analyzer is enabled by default or if the user must manually enable it. Generally set to true").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
        }

        private async Task<Document> DiagnosticSeverityWarning(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            var newExpression = generator.MemberAccessExpression(expression, "Warning") as ExpressionSyntax;
            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var newRule = rule.ReplaceNode(argument.Expression, newExpression);

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
        }

        private async Task<Document> DiagnosticSeverityHidden(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            var newExpression = generator.MemberAccessExpression(expression, "Hidden") as ExpressionSyntax;
            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var newRule = rule.ReplaceNode(argument.Expression, newExpression);

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
        }

        private async Task<Document> DiagnosticSeverityInfo(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            var newExpression = generator.MemberAccessExpression(expression, "Info") as ExpressionSyntax;
            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var newRule = rule.ReplaceNode(argument.Expression, newExpression);

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
        }

        private async Task<Document> DiagnosticSeverityError(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            var newExpression = generator.MemberAccessExpression(expression, "Error") as ExpressionSyntax;
            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var newRule = rule.ReplaceNode(argument.Expression, newExpression);

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
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

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var expressionKind = SyntaxFactory.ParseExpression("\"DescriptiveId\"") as ExpressionSyntax;
            var newField = CodeFixHelper.NewIdCreator(generator, currentRuleId, expressionKind).WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs").ElementAt(0), SyntaxFactory.EndOfLine("\r\n")));

            var newClass = generator.InsertMembers(classDeclaration, 0, newField) as ClassDeclarationSyntax;
            var triviaClass = newClass.ReplaceNode(newClass.Members[0], newField);

            return await ReplaceNode(classDeclaration, triviaClass, document);
        }

        private async Task<Document> IdDeclTypeAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var members = classDeclaration.Members;
            ExpressionSyntax oldIdName = null;
            IdentifierNameSyntax newIdName = null;
            FieldDeclarationSyntax rule = null;

            foreach (MemberDeclarationSyntax memberSyntax in members)
            {
                var fieldDeclaration = memberSyntax as FieldDeclarationSyntax;
                if (fieldDeclaration == null)
                {
                    continue;
                }

                if (fieldDeclaration.Declaration.Type != null && fieldDeclaration.Declaration.Type.ToString() == "DiagnosticDescriptor")
                {
                    rule = fieldDeclaration;

                    var declaratorSyntax = fieldDeclaration.Declaration.Variables[0] as VariableDeclaratorSyntax;
                    var objectCreationSyntax = declaratorSyntax.Initializer.Value as ObjectCreationExpressionSyntax;
                    var ruleArgumentList = objectCreationSyntax.ArgumentList;

                    for (int i = 0; i < ruleArgumentList.Arguments.Count; i++)
                    {
                        var currentArg = ruleArgumentList.Arguments[i];
                        string currentArgName = currentArg.NameColon.Name.Identifier.Text;
                        if (currentArgName == "id")
                        {
                            oldIdName = currentArg.Expression;
                            break;
                        }
                    }

                    continue;
                }

                var modifiers = fieldDeclaration.Modifiers;
                if (modifiers == null)
                {
                    continue;
                }

                bool isPublic = false;
                bool isConst = false;

                foreach (var modifier in modifiers)
                {
                    if (modifier.Text == "public")
                    {
                        isPublic = true;
                    }

                    if (modifier.Text == "const")
                    {
                        isConst = true;
                    }
                }

                if (isPublic && isConst)
                {
                    var ruleIdSymbol = fieldDeclaration;
                    var ruleIdSyntax = ruleIdSymbol.Declaration.Variables[0] as VariableDeclaratorSyntax;
                    var newIdIdentifier = ruleIdSyntax.Identifier.ToString();
                    newIdName = generator.IdentifierName(newIdIdentifier) as IdentifierNameSyntax;
                }
            }

            var newRule = rule.ReplaceNode(oldIdName, newIdName);

            return await ReplaceNode(rule, newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// id: Identifies each rule. Same as the public constant declared above").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(rule.GetLeadingTrivia()), document);
        }
        #endregion

        #region supported diagnostics code fix
        private async Task<Document> IncorrectSigSuppDiagAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var type = generator.IdentifierName("ImmutableArray<DiagnosticDescriptor>");
            var getAccessorStatements = new SyntaxList<SyntaxNode>();
            foreach (var accessor in declaration.AccessorList.Accessors)
            {
                if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                {
                    getAccessorStatements = accessor.Body.Statements;
                    break;
                }
            }

            var newPropertyDecl = generator.PropertyDeclaration("SupportedDiagnostics", type, accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, getAccessorStatements: getAccessorStatements).WithLeadingTrivia(declaration.GetLeadingTrivia()).WithTrailingTrivia(declaration.GetTrailingTrivia());
            newPropertyDecl = newPropertyDecl.RemoveNode((newPropertyDecl as PropertyDeclarationSyntax).AccessorList.Accessors[1], 0);
            
            return await ReplaceNode(declaration, newPropertyDecl, document);
        }

        private async Task<Document> MissingAccessorAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            SemanticModel semanticModel = await document.GetSemanticModelAsync();

            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            var throwStatement = new[] { generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)) };
            var type = generator.GetType(declaration);
            var newPropertyDeclaration = generator.PropertyDeclaration("SupportedDiagnostics", type, Accessibility.Public, DeclarationModifiers.Override, throwStatement) as PropertyDeclarationSyntax;
            newPropertyDeclaration = newPropertyDeclaration.RemoveNode(newPropertyDeclaration.AccessorList.Accessors[1], 0);

            return await ReplaceNode(declaration, newPropertyDeclaration, document);
        }

        private async Task<Document> TooManyAccessorsAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var allAccessors = declaration.AccessorList.Accessors.OfType<AccessorDeclarationSyntax>();
            bool foundGetAccessor = false;
            AccessorDeclarationSyntax accessorToKeep = null;
            var accessorList = declaration.AccessorList;

            foreach (AccessorDeclarationSyntax accessor in allAccessors)
            {
                var keyword = accessor.Keyword;
                if (keyword.IsKind(SyntaxKind.GetKeyword) && !foundGetAccessor)
                {
                    accessorToKeep = accessor;
                    foundGetAccessor = true;
                }
                else
                {
                    accessorList = accessorList.RemoveNode(accessor, 0);
                }
            }

            var block = SyntaxFactory.Block(new StatementSyntax[0]);
            if (accessorToKeep == null)
            {
                accessorToKeep = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, block);
            }
            
            SyntaxList<SyntaxNode> accessorsToAdd = new SyntaxList<SyntaxNode>();
            accessorsToAdd = accessorsToAdd.Add(accessorToKeep);
            var newPropertyDeclaration = declaration.WithAccessorList(null);
            newPropertyDeclaration = generator.AddAccessors(newPropertyDeclaration, accessorsToAdd) as PropertyDeclarationSyntax;

            return await ReplaceNode(declaration, newPropertyDeclaration, document);
        }

        private async Task<Document> AccessorReturnValueAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var expressionString = generator.IdentifierName("ImmutableArray");
            var identifierString = generator.IdentifierName("Create");
            var expression = generator.MemberAccessExpression(expressionString, identifierString);
            var invocationExpression = generator.InvocationExpression(expression);
            var returnStatement = (generator.ReturnStatement(invocationExpression) as ReturnStatementSyntax).WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// This array contains all the diagnostics that can be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n")));

            SyntaxList<AccessorDeclarationSyntax> accessors = declaration.AccessorList.Accessors;
            if (accessors == null || accessors.Count == 0)
            {
                return document;
            }

            var firstAccessor = declaration.AccessorList.Accessors.First();
            if (firstAccessor == null || !firstAccessor.Keyword.IsKind(SyntaxKind.GetKeyword))
            {
                return document;
            }

            var oldBody = firstAccessor.Body as BlockSyntax;
            SyntaxList<StatementSyntax> oldStatements = oldBody.Statements;
            StatementSyntax oldStatement = null;
            if (oldStatements.Count != 0)
            {
                oldStatement = oldStatements.First();
            }

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root;

            if (oldStatement == null)
            {
                var newAccessorDeclaration = firstAccessor.AddBodyStatements(returnStatement);
                newRoot = root.ReplaceNode(firstAccessor, newAccessorDeclaration);
            }
            else
            {
                newRoot = root.ReplaceNode(oldStatement, returnStatement);
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
                if (propertySyntax.Identifier.Text != "SupportedDiagnostics")
                {
                    continue;
                }

                AccessorDeclarationSyntax getAccessor = propertySyntax.AccessorList.Accessors.First();
                var returnStatement = getAccessor.Body.Statements.First() as ReturnStatementSyntax;
                InvocationExpressionSyntax invocationExpression = null;
                if (returnStatement == null)
                {
                    var declarationStatement = getAccessor.Body.Statements.First() as LocalDeclarationStatementSyntax;
                    if (declarationStatement == null)
                    {
                        return document;
                    }

                    invocationExpression = declarationStatement.Declaration.Variables[0].Initializer.Value as InvocationExpressionSyntax;
                }
                else
                {
                    invocationExpression = returnStatement.Expression as InvocationExpressionSyntax;
                }

                var oldArgumentList = invocationExpression.ArgumentList as ArgumentListSyntax;

                string argumentListString = "";
                foreach (string ruleName in ruleNames)
                {
                    if (ruleName == ruleNames.First())
                    {
                        argumentListString += ruleName;
                    }
                    else
                    {
                        argumentListString += ", " + ruleName;
                    }
                }

                var argumentListSyntax = SyntaxFactory.ParseArgumentList("(" + argumentListString + ")");
                SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

                var args = argumentListSyntax.Arguments;
                var nodeArgs = new SyntaxList<SyntaxNode>();
                foreach (var arg in args)
                {
                    nodeArgs = nodeArgs.Add(arg as SyntaxNode);
                }

                if (invocationExpression.FirstAncestorOrSelf<ReturnStatementSyntax>() == null)
                {
                    return await ReplaceNode(invocationExpression.FirstAncestorOrSelf<LocalDeclarationStatementSyntax>(), generator.LocalDeclarationStatement(invocationExpression.FirstAncestorOrSelf<LocalDeclarationStatementSyntax>().Declaration.Variables[0].Identifier.Text, generator.InvocationExpression(generator.MemberAccessExpression(generator.IdentifierName("ImmutableArray"), "Create"), nodeArgs)).WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// This array contains all the diagnostics that can be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
                }
                else
                {
                    return await ReplaceNode(invocationExpression.Parent, generator.ReturnStatement(generator.InvocationExpression(generator.MemberAccessExpression(generator.IdentifierName("ImmutableArray"), "Create"), nodeArgs)).WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// This array contains all the diagnostics that can be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
                }
            }

           return document;
        }
        #endregion

        // replaces the incorrect statement with the keyword statement
        private async Task<Document> IncorrectKeywordAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var block = declaration.Parent as BlockSyntax;
            var ifKeyword = CodeFixHelper.KeywordHelper(generator, block);

            return await ReplaceNode(declaration, ifKeyword.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// This statement navigates down the syntax tree one level to extract the 'if' keyword").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        private async Task<Document> TrailingCheckIncorrectAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifBlockStatements = new SyntaxList<StatementSyntax>();
            if (declaration.Body.Statements[2].Kind() == SyntaxKind.IfStatement)
            {
                var ifDeclaration = declaration.Body.Statements[2] as IfStatementSyntax;
                var ifBlock = ifDeclaration.Statement as BlockSyntax;
                if (ifBlock != null)
                {
                    ifBlockStatements = ifBlock.Statements;
                }
            }

            StatementSyntax ifStatement = CodeFixHelper.TriviaCheckHelper(generator, declaration.Body, ifBlockStatements) as StatementSyntax;

            var oldBlock = declaration.Body;
            var newBlock = declaration.Body.WithStatements(declaration.Body.Statements.Replace(declaration.Body.Statements[2], ifStatement));

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // adds the keyword statement
        private async Task<Document> MissingKeywordAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodBlock = declaration.Body as BlockSyntax;
            var ifKeyword = CodeFixHelper.KeywordHelper(generator, methodBlock) as StatementSyntax;
            var newBlock = methodBlock.AddStatements(ifKeyword.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// This statement navigates down the syntax tree one level to extract the 'if' keyword").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(methodBlock, newBlock, document);
        }

        private async Task<Document> TrailingCheckMissingAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifBlockStatements = new SyntaxList<StatementSyntax>();
            StatementSyntax ifStatement = CodeFixHelper.TriviaCheckHelper(generator, declaration.Body, ifBlockStatements) as StatementSyntax;

            var oldBlock = declaration.Body;
            var newBlock = declaration.Body.WithStatements(declaration.Body.Statements.Add(ifStatement));

            return await ReplaceNode(oldBlock, newBlock, document);
        }
        
        private async Task<Document> TrailingVarMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifStatement = declaration.Parent.Parent as IfStatementSyntax;
            var localDeclaration = new SyntaxList<SyntaxNode>().Add(CodeFixHelper.TriviaVarMissingHelper(generator, ifStatement));

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(localDeclaration);

            return await ReplaceNode(oldBlock, newBlock, document);
        }
        
        private async Task<Document> TrailingVarIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            IfStatementSyntax ifStatement;
            if (declaration.Parent.Parent.Parent.Parent.Kind() == SyntaxKind.MethodDeclaration)
            {
                ifStatement = declaration as IfStatementSyntax;
            }
            else
            {
                ifStatement = declaration.Parent.Parent as IfStatementSyntax;
            }

            var localDeclaration = CodeFixHelper.TriviaVarMissingHelper(generator, ifStatement) as LocalDeclarationStatementSyntax;

            var oldBlock = ifStatement.Statement as BlockSyntax;
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
            if (declaration.Parent.Parent.Parent.Parent.Kind() == SyntaxKind.MethodDeclaration)
            {
                ifStatement = declaration as IfStatementSyntax;
            }
            else
            {
                ifStatement = declaration.Parent.Parent as IfStatementSyntax;
                var ifBlock = declaration.Statement as BlockSyntax;
                if (ifBlock != null)
                {
                    ifBlockStatements = ifBlock.Statements;
                }
            }

            var newIfStatement = CodeFixHelper.TriviaKindCheckHelper(generator, ifStatement, ifBlockStatements) as StatementSyntax;

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
            var newIfStatement = CodeFixHelper.TriviaKindCheckHelper(generator, declaration, ifBlockStatements) as StatementSyntax;

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.AddStatements(newIfStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> WhitespaceCheckIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            IfStatementSyntax ifStatement;
            var ifBlockStatements = new SyntaxList<SyntaxNode>();

            if (declaration.Parent.Parent.Parent.Parent.Parent.Parent.Kind() == SyntaxKind.MethodDeclaration)
            {
                ifStatement = declaration as IfStatementSyntax;
            }
            else
            {
                ifStatement = declaration.Parent.Parent as IfStatementSyntax;
                var ifBlock = declaration.Statement as BlockSyntax;
                if (ifBlock != null)
                {
                    ifBlockStatements = ifBlock.Statements;
                }
            }

            var newIfStatement = CodeFixHelper.WhitespaceCheckHelper(generator, ifStatement, ifBlockStatements) as StatementSyntax;

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
            var newIfStatement = new SyntaxList<SyntaxNode>().Add(CodeFixHelper.WhitespaceCheckHelper(generator, declaration, ifBlockStatements) as StatementSyntax);

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(newIfStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> ReturnIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            IfStatementSyntax ifStatement;
            if (declaration.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Kind() != SyntaxKind.MethodDeclaration)
            {
                ifStatement = declaration.Parent.Parent as IfStatementSyntax;
            }
            else
            {
                ifStatement = declaration as IfStatementSyntax;
            }

            var returnStatement = generator.ReturnStatement() as ReturnStatementSyntax;

            var oldBlock = ifStatement.Statement as BlockSyntax;
            var newStatements = oldBlock.Statements.Replace(oldBlock.Statements[0], returnStatement.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', it will return from this method without reporting a diagnostic").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            var newBlock = oldBlock.WithStatements(newStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> ReturnMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var returnStatements = new SyntaxList<SyntaxNode>().Add(generator.ReturnStatement().WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', it will return from this method without reporting a diagnostic").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(returnStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> TriviaCountMissingAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var nameStatement = declaration.Body.Statements[1] as LocalDeclarationStatementSyntax;
            var name = nameStatement.Declaration.Variables[0].Identifier.ValueText;
            var ifBlockStatements = new SyntaxList<StatementSyntax>();

            var ifStatement = declaration.Body.Statements[2] as IfStatementSyntax;
            var localDeclaration = new SyntaxList<SyntaxNode>().Add(CodeFixHelper.TriviaCountHelper(generator, name, ifBlockStatements));

            var oldBlock = ifStatement.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(localDeclaration);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        private async Task<Document> TriviaCountIncorrectAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var nameStatement = declaration.Body.Statements[1] as LocalDeclarationStatementSyntax;
            var name = nameStatement.Declaration.Variables[0].Identifier.ValueText;
            var ifStatement = declaration.Body.Statements[2] as IfStatementSyntax;

            var ifBlockStatements = new SyntaxList<StatementSyntax>();
            if (declaration.Body.Statements[2].Kind() == SyntaxKind.IfStatement)
            {
                var ifDeclaration = ifStatement.Statement as BlockSyntax;
                var ifBlockStatement = ifDeclaration.Statements[0] as IfStatementSyntax;
                var ifBlock = ifBlockStatement.Statement as BlockSyntax;
                if (ifBlock != null)
                {
                    ifBlockStatements = ifBlock.Statements;
                }
            }

            var localDeclaration = CodeFixHelper.TriviaCountHelper(generator, name, ifBlockStatements) as StatementSyntax;

            var oldBlock = ifStatement.Statement as BlockSyntax;
            var oldStatement = oldBlock.Statements[0];
            var newStatements = oldBlock.Statements.Replace(oldStatement, localDeclaration);
            var newBlock = oldBlock.WithStatements(newStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        class CodeFixHelper
        {
            // creates an if-statement checking the count of trailing trivia
            internal static SyntaxNode TriviaCountHelper(SyntaxGenerator generator, string name, SyntaxList<StatementSyntax> ifBlockStatements)
            {
                var variableName = generator.IdentifierName(name);
                var memberAccess = generator.MemberAccessExpression(variableName, "TrailingTrivia");
                var fullMemberAccess = generator.MemberAccessExpression(memberAccess, "Count");
                var one = generator.LiteralExpression(1);
                var equalsExpression = generator.ValueEqualsExpression(fullMemberAccess, one);
                var newIfStatement = generator.IfStatement(equalsExpression, ifBlockStatements);

                return newIfStatement;
            }

            // creates a statement casting context.Node to if-statement
            internal static SyntaxNode IfHelper(SyntaxGenerator generator, string name)
            {
                var type = SyntaxFactory.ParseTypeName("IfStatementSyntax");
                var expression = generator.IdentifierName(name);
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
                var methodDecl = declaration.Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                var methodBlock = methodDecl.Body as BlockSyntax;
                var secondStatement = methodBlock.Statements[1] as LocalDeclarationStatementSyntax;
                var variableName = generator.IdentifierName(secondStatement.Declaration.Variables[0].Identifier.ValueText);

                var ifTrailing = generator.MemberAccessExpression(variableName, "TrailingTrivia");
                var fullVariable = generator.MemberAccessExpression(ifTrailing, "First");
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

                var trailingTriviaDeclaration = ifOneBlock.Statements[0] as LocalDeclarationStatementSyntax;
                var trailingTrivia = generator.IdentifierName(trailingTriviaDeclaration.Declaration.Variables[0].Identifier.ValueText);

                var arguments = new SyntaxList<SyntaxNode>();
                var trailingTriviaToString = generator.InvocationExpression(generator.MemberAccessExpression(trailingTrivia, "ToString"), arguments);
                var rightSide = generator.LiteralExpression(" ");
                var equalsExpression = generator.ValueEqualsExpression(trailingTriviaToString, rightSide);

                var newIfStatement = generator.IfStatement(equalsExpression, ifBlockStatements);

                return newIfStatement;
            }

            internal static SyntaxNode BuildInitialize(SyntaxGenerator generator, INamedTypeSymbol notImplementedException, SyntaxList<StatementSyntax> statements, string name)
            {
                var type = SyntaxFactory.ParseTypeName("AnalysisContext");
                var parameters = new[] { generator.ParameterDeclaration(name, type) };

                if (notImplementedException != null)
                {
                    statements = statements.Add(generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)) as StatementSyntax);
                }

                var initializeDeclaration = generator.MethodDeclaration("Initialize", parameters: parameters, accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, statements: statements);
                return initializeDeclaration;
            }

            internal static SyntaxNode NewIdCreator(SyntaxGenerator generator, string fieldName, SyntaxNode initializer)
            {
                SyntaxNode newField = generator.FieldDeclaration(fieldName, generator.TypeExpression(SpecialType.System_String), Accessibility.Public, DeclarationModifiers.Const, initializer);

                return newField;
            }

            // creates a variable creating a location for the diagnostic
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

            // creates a variable creating a span for the diagnostic
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

            // creates a variable of the form var variableName = identifierString.SpanStart;
            internal static SyntaxNode CreateEndOrStartSpan(SyntaxGenerator generator, string identifierString, string variableName)
            {
                SyntaxNode identifier = generator.IdentifierName(identifierString);
                SyntaxNode initializer = generator.MemberAccessExpression(identifier, "SpanStart");
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(variableName, initializer);

                return localDeclaration;
            }

            // creates a variable of the form var openParen = expressionString.OpenParentToken
            internal static SyntaxNode CreateOpenParen(SyntaxGenerator generator, string expressionString)
            {
                string name = "openParen";
                var expression = generator.IdentifierName(expressionString);
                var initializer = generator.MemberAccessExpression(expression, "OpenParenToken");
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

                return localDeclaration;
            }

            // creates a variable that creates a diagnostic
            internal static SyntaxNode CreateDiagnostic(SyntaxGenerator generator, string locationName, string ruleName)
            {
                var identifier = generator.IdentifierName("Diagnostic");
                var expression = generator.MemberAccessExpression(identifier, "Create");

                SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();

                var ruleExpression = generator.IdentifierName(ruleName);
                var ruleArg = generator.Argument(ruleExpression);

                var locationExpression = generator.IdentifierName(locationName);
                var locationArg = generator.Argument(locationExpression);

                arguments = arguments.Add(ruleArg);
                arguments = arguments.Add(locationArg);

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

            // creates a statement that reports a diagnostic
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

            // creates a variable holding a DiagnosticDescriptor
            // uses SyntaxFactory for formatting
            internal static FieldDeclarationSyntax CreateEmptyRule(SyntaxGenerator generator, string idName="Change me to the name of the above constant", string titleDefault="Enter a title for this diagnostic", string messageDefault="Enter a message to be displayed with this diagnostic",
                                                                    string categoryDefault="Enter a category for this diagnostic (e.g. Formatting)", ExpressionSyntax severityDefault=null, ExpressionSyntax enabledDefault=null)
            {
                if (severityDefault == null)
                {
                    severityDefault = generator.DefaultExpression(SyntaxFactory.ParseTypeName("DiagnosticSeverity")) as ExpressionSyntax;
                }

                if (enabledDefault == null)
                {
                    enabledDefault = generator.DefaultExpression(generator.TypeExpression(SpecialType.System_Boolean)) as ExpressionSyntax;
                }

                var type = SyntaxFactory.ParseTypeName("DiagnosticDescriptor");

                var arguments = new ArgumentSyntax[6];
                var whitespace = "            ";

                var id = generator.LiteralExpression(idName);
                var idArg = generator.Argument("id", RefKind.None, id).WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[0] = idArg;

                var title = generator.LiteralExpression(titleDefault);
                var titleArg = generator.Argument("title", RefKind.None, title).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[1] = titleArg;

                var message = generator.LiteralExpression(messageDefault);
                var messageArg = generator.Argument("messageFormat", RefKind.None, message).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[2] = messageArg;

                var category = generator.LiteralExpression(categoryDefault);
                var categoryArg = generator.Argument("category", RefKind.None, category).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[3] = categoryArg;

                var defaultSeverityArg = generator.Argument("defaultSeverity", RefKind.None, severityDefault).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[4] = defaultSeverityArg;

                var enabledArg = generator.Argument("isEnabledByDefault", RefKind.None, enabledDefault).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[5] = enabledArg;
                
                var identifier = SyntaxFactory.ParseToken("spacingRule");

                var separators = new List<SyntaxToken>();
                var separator = SyntaxFactory.ParseToken(",").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);
                separators.Add(separator);
                separators.Add(separator);
                separators.Add(separator);
                separators.Add(separator);
                separators.Add(separator);

                var argumentsNewLines = SyntaxFactory.SeparatedList(arguments, separators);
                var argumentList = SyntaxFactory.ArgumentList(argumentsNewLines);
                var value = SyntaxFactory.ObjectCreationExpression(type, argumentList, null);
                var initializer = SyntaxFactory.EqualsValueClause(value);

                var variables = new SeparatedSyntaxList<VariableDeclaratorSyntax>();
                var variable = SyntaxFactory.VariableDeclarator(identifier, null, initializer);
                variables = variables.Add(variable);

                var declaration = SyntaxFactory.VariableDeclaration(type.WithTrailingTrivia(SyntaxFactory.Whitespace(" ")), variables);
                var modifiers = SyntaxFactory.TokenList(SyntaxFactory.ParseToken("internal").WithTrailingTrivia(SyntaxFactory.Whitespace(" ")), SyntaxFactory.ParseToken("static").WithTrailingTrivia(SyntaxFactory.Whitespace(" ")));
                var rule = SyntaxFactory.FieldDeclaration(new SyntaxList<AttributeListSyntax>(), modifiers, declaration);

                return rule;
            }

            // creates the SupportedDiagnostics property with a get accessor with a not implemented exception
            internal static PropertyDeclarationSyntax CreateSupportedDiagnostics(SyntaxGenerator generator, INamedTypeSymbol notImplementedException)
            {
                var type = SyntaxFactory.ParseTypeName("ImmutableArray<DiagnosticDescriptor>");
                var modifiers = DeclarationModifiers.Override;

                SyntaxList<SyntaxNode> getAccessorStatements = new SyntaxList<SyntaxNode>();

                SyntaxNode throwStatement = generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException));
                getAccessorStatements = getAccessorStatements.Add(throwStatement);

                var propertyDeclaration = generator.PropertyDeclaration("SupportedDiagnostics", type, accessibility: Accessibility.Public, modifiers: modifiers, getAccessorStatements: getAccessorStatements) as PropertyDeclarationSyntax;
                propertyDeclaration = propertyDeclaration.RemoveNode(propertyDeclaration.AccessorList.Accessors[1], 0);

                return propertyDeclaration;
            }

            // creates a SyntaxKind.IfStatement argument
            internal static ArgumentSyntax CreateSyntaxKindIfStatement(SyntaxGenerator generator)
            {
                var syntaxKind = generator.IdentifierName("SyntaxKind");
                var expression = generator.MemberAccessExpression(syntaxKind, "IfStatement");
                var argument = generator.Argument(expression) as ArgumentSyntax;

                return argument;
            }

            internal static SyntaxNode CreateRegister(SyntaxGenerator generator, MethodDeclarationSyntax declaration, string methodName)
            {
                SyntaxNode argument1 = generator.IdentifierName(methodName);
                SyntaxNode argument2 = generator.MemberAccessExpression(generator.IdentifierName("SyntaxKind"), "IfStatement");
                SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>().Add(argument1).Add(argument2);

                SyntaxNode expression = generator.IdentifierName(declaration.ParameterList.Parameters[0].Identifier.ValueText);
                SyntaxNode memberAccessExpression = generator.MemberAccessExpression(expression, "RegisterSyntaxNodeAction");
                SyntaxNode invocationExpression = generator.InvocationExpression(memberAccessExpression, arguments);

                return invocationExpression;
            }

            internal static SyntaxNode CreateAnalysisMethod(SyntaxGenerator generator, string methodName, SemanticModel semanticModel)
            {
                var type = SyntaxFactory.ParseTypeName("SyntaxNodeAnalysisContext");
                var parameters = new[] { generator.ParameterDeclaration("context", type) };
                SyntaxList<SyntaxNode> statements = new SyntaxList<SyntaxNode>();
                INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
                statements = statements.Add(generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)));

                SyntaxNode newMethodDeclaration = generator.MethodDeclaration(methodName, parameters: parameters, accessibility: Accessibility.Private, statements: statements);
                return newMethodDeclaration.WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia("// This method, which is the method that is registered within Initialize, performs the analysis of the Syntax Tree when an IfStatementSyntax Node is found. If the analysis finds an error, a diagnostic is reported").ElementAt(0), SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// In this tutorial, this method will walk through the Syntax Tree seen in IfSyntaxTree.jpg and determine if the if-statement being analyzed has the correct spacing").ElementAt(0), SyntaxFactory.CarriageReturnLineFeed);
            }

            internal static string ExistingAnalysisMethod(ClassDeclarationSyntax classDeclaration)
            {
                IEnumerable<MethodDeclarationSyntax> methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();
                string methodName = null;

                foreach (MethodDeclarationSyntax method in methods)
                {
                    var parameterList = method.ParameterList;
                    if (parameterList != null)
                    {
                        var parameters = parameterList.Parameters;
                        if (parameters != null)
                        {
                            if (parameters.Count > 0)
                            {
                                var parameterType = parameters.First().Type;
                                if (parameterType != null && parameterType.ToString() == "SyntaxNodeAnalysisContext")
                                {
                                    return methodName = method.Identifier.Text;
                                }
                            }
                        }
                    }
                }

                return methodName;
            }

            // creates a method keeping everything except for the parameters, and inserting a parameter of type SyntaxNodeAnalysisContext
            internal static SyntaxNode CreateMethodWithContextParameter(SyntaxGenerator generator, MethodDeclarationSyntax methodDeclaration)
            {
                var type = SyntaxFactory.ParseTypeName("SyntaxNodeAnalysisContext");
                var parameters = new[] { generator.ParameterDeclaration("context", type) };
                string methodName = methodDeclaration.Identifier.Text;
                var returnType = methodDeclaration.ReturnType;
                var statements = methodDeclaration.Body.Statements;

                var newDeclaration = generator.MethodDeclaration(methodName, parameters, returnType: returnType, accessibility: Accessibility.Private, statements: statements);
                return newDeclaration;
            }
        }
    }
}