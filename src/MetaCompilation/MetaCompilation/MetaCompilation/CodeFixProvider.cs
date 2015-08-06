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
                                             MetaCompilationAnalyzer.IdStringLiteral,
                                             MetaCompilationAnalyzer.Title,
                                             MetaCompilationAnalyzer.Message,
                                             MetaCompilationAnalyzer.Category);
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
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the first trailing trivia into a variable", c => TrailingVarMissingAsync(context.Document, declaration, c), "Extract the first trailing trivia into a variable"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaVarIncorrect:
                        IEnumerable<IfStatementSyntax> varDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (varDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = varDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the first trailing trivia into a variable", c => TrailingVarIncorrectAsync(context.Document, declaration, c), "Extract the first trailing trivia into a variable"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect:
                        IEnumerable<IfStatementSyntax> kindDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (kindDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = kindDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check the kind of the first trailing trivia", c => TrailingKindCheckIncorrectAsync(context.Document, declaration, c), "Check the kind of the first trailing trivia"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.TrailingTriviaKindCheckMissing:
                        IEnumerable<IfStatementSyntax> kindMissingDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                        if (kindMissingDeclarations.Count() != 0)
                        {
                            IfStatementSyntax declaration = kindMissingDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Check the kind of the first trailing trivia", c => TrailingKindCheckMissingAsync(context.Document, declaration, c), "Check the kind of the first trailing trivia"), diagnostic);
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
                        IEnumerable<PropertyDeclarationSyntax> incorrectAccessorDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>();
                        if (incorrectAccessorDeclarations.Count() != 0)
                        {
                            PropertyDeclarationSyntax declaration = incorrectAccessorDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Return an ImmutableArray of DiagnosticDescriptors from SupportedDiagnostics", c => AccessorReturnValueAsync(context.Document, declaration, c), "Return an ImmutableArray of DiagnosticDescriptors from SupportedDiagnostics"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.IncorrectAccessorReturn:
                        IEnumerable<ClassDeclarationSyntax> accessorReturnDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                        if (accessorReturnDeclarations.Count() != 0)
                        {
                            ClassDeclarationSyntax declaration = accessorReturnDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Return an ImmutableArray of all DiagnosticDescriptors that can be reported", c => AccessorWithRulesAsync(context.Document, declaration, c), "Return an ImmutableArray of all DiagnosticDescriptors that can be reported"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.SupportedRules:
                        IEnumerable<ClassDeclarationSyntax> rulesDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                        if (rulesDeclarations.Count() != 0)
                        {
                            ClassDeclarationSyntax declaration = rulesDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Return a list of all diagnostics that can be reported by this analyzer", c => AccessorWithRulesAsync(context.Document, declaration, c), "Return a list of all diagnostics that can be reported by this analyzer"), diagnostic);
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
                    case MetaCompilationAnalyzer.Title:
                        IEnumerable<LiteralExpressionSyntax> titleDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>();
                        if (titleDeclarations.Count() != 0)
                        {
                            LiteralExpressionSyntax declaration = titleDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Replace the title", c => ReplaceTitle(context.Document, declaration, c), "Replace the title"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.Message:
                        IEnumerable<LiteralExpressionSyntax> messageDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>();
                        if (messageDeclarations.Count() != 0)
                        {
                            LiteralExpressionSyntax declaration = messageDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Replace the title", c => ReplaceMessage(context.Document, declaration, c), "Replace the title"), diagnostic);
                        }
                        break;
                    case MetaCompilationAnalyzer.Category:
                        IEnumerable<LiteralExpressionSyntax> categoryDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>();
                        if (categoryDeclarations.Count() != 0)
                        {
                            LiteralExpressionSyntax declaration = categoryDeclarations.First();
                            context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Replace the title", c => ReplaceCategory(context.Document, declaration, c), "Replace the title"), diagnostic);
                        }
                        break;
                }
            }
        }

        // replace the category string
        private async Task<Document> ReplaceCategory(Document document, LiteralExpressionSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document); 
            SyntaxNode newString = generator.LiteralExpression("Formatting");
            return await ReplaceNode(declaration, newString, document);
        }

        // replaces the messageFormat string
        private async Task<Document> ReplaceMessage(Document document, LiteralExpressionSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode newString = generator.LiteralExpression("The trivia between 'if' and '(' should be a single space");
            return await ReplaceNode(declaration, newString, document);
        }

        // replaces the title string
        private async Task<Document> ReplaceTitle(Document document, LiteralExpressionSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode newString = generator.LiteralExpression("If-statement spacing");
            return await ReplaceNode(declaration, newString, document);
        }

        // replaces a node in the document
        private async Task<Document> ReplaceNode(SyntaxNode oldNode, SyntaxNode newNode, Document document)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync();
            SyntaxNode newRoot = root.ReplaceNode(oldNode, newNode);
            Document newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        #region analysis method fixes
        // sets the analysis method to take a parameter of type SyntaxNodeAnalysisContext
        private async Task<Document> IncorrectAnalysisParameterAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode newDeclaration = CodeFixHelper.CreateMethodWithContextParameter(generator, declaration);

            return await ReplaceNode(declaration, newDeclaration, document);
        }

        // sets the return type of the method to void
        private async Task<Document> IncorrectAnalysisReturnTypeAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxNode newDeclaration = CodeFixHelper.MethodReturnType(declaration, "void");

            return await ReplaceNode(declaration, newDeclaration, document);
        }

        // sets the method accessibility to private
        private async Task<Document> IncorrectAnalysisAccessibilityAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode newDeclaration = CodeFixHelper.MethodAccessibility(generator, declaration, Accessibility.Private);

            return await ReplaceNode(declaration, newDeclaration, document);
        }

        // adds an analysis method to the enclosing class
        private async Task<Document> MissingAnalysisMethodAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string analysisMethodName = CodeFixHelper.AnalysisMethodName(declaration);

            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            var newAnalysisMethod = CodeFixHelper.CreateAnalysisMethod(generator, analysisMethodName, semanticModel) as MethodDeclarationSyntax;

            ClassDeclarationSyntax classDeclaration = declaration.Ancestors().OfType<ClassDeclarationSyntax>().First();

            return await ReplaceNode(classDeclaration, classDeclaration.AddMembers(newAnalysisMethod), document);
        }

        // replaces the if statement variable
        private async Task<Document> IncorrectIfAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            MethodDeclarationSyntax methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string name = CodeFixHelper.GetContextParameter(methodDeclaration);
            SyntaxNode ifStatement = CodeFixHelper.IfHelper(generator, name);

            return await ReplaceNode(declaration, ifStatement.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // adds the if statement variable
        private async Task<Document> MissingIfAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string name = CodeFixHelper.GetContextParameter(declaration);
            StatementSyntax ifStatement = CodeFixHelper.IfHelper(generator, name) as StatementSyntax;

            var oldBlock = declaration.Body as BlockSyntax;
            var newBlock = oldBlock.AddStatements(ifStatement.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // replaces the incorrect statement with the keyword statement
        private async Task<Document> IncorrectKeywordAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var block = declaration.Parent as BlockSyntax;
            SyntaxNode ifKeyword = CodeFixHelper.KeywordHelper(generator, block);

            return await ReplaceNode(declaration, ifKeyword.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// This statement navigates down the syntax tree one level to extract the 'if' keyword").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // adds the keyword statement
        private async Task<Document> MissingKeywordAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodBlock = declaration.Body as BlockSyntax;
            var ifKeyword = CodeFixHelper.KeywordHelper(generator, methodBlock) as StatementSyntax;
            BlockSyntax newBlock = methodBlock.AddStatements(ifKeyword.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// This statement navigates down the syntax tree one level to extract the 'if' keyword").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(methodBlock, newBlock, document);
        }

        // replaces the trailing trivia check
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

            BlockSyntax oldBlock = declaration.Body;
            BlockSyntax newBlock = declaration.Body.WithStatements(declaration.Body.Statements.Replace(declaration.Body.Statements[2], ifStatement));

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // adds the trailing trivia check
        private async Task<Document> TrailingCheckMissingAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifBlockStatements = new SyntaxList<StatementSyntax>();
            var leadingTrivia = SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Checks if there is any trailing trivia (eg spaces or comments) associated with the if-keyword").ElementAt(0), SyntaxFactory.CarriageReturnLineFeed);
            StatementSyntax ifStatement = (CodeFixHelper.TriviaCheckHelper(generator, declaration.Body, ifBlockStatements) as StatementSyntax).WithLeadingTrivia(leadingTrivia);

            BlockSyntax oldBlock = declaration.Body;
            BlockSyntax newBlock = oldBlock.WithStatements(declaration.Body.Statements.Add(ifStatement));

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // adds the trivia count check
        private async Task<Document> TriviaCountMissingAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string name = CodeFixHelper.GetIfKeywordName(declaration.Body);
            var ifBlockStatements = new SyntaxList<StatementSyntax>();

            var ifStatement = declaration.Body.Statements[2] as IfStatementSyntax;
            var leadingTrivia = SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Checks that there is only one piece of trailing trivia").ElementAt(0), SyntaxFactory.CarriageReturnLineFeed);
            var localDeclaration = new SyntaxList<SyntaxNode>().Add((CodeFixHelper.TriviaCountHelper(generator, name, ifBlockStatements)).WithLeadingTrivia(leadingTrivia));

            var oldBlock = ifStatement.Statement as BlockSyntax;
            BlockSyntax newBlock = oldBlock.WithStatements(localDeclaration);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // replaces the trivia count check
        private async Task<Document> TriviaCountIncorrectAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string name = CodeFixHelper.GetIfKeywordName(declaration.Body);
            var ifStatement = declaration.Body.Statements[2] as IfStatementSyntax;

            var ifBlockStatements = new SyntaxList<StatementSyntax>();
            if (ifStatement != null)
            {
                var ifDeclaration = ifStatement.Statement as BlockSyntax;
                var ifBlockStatement = ifDeclaration.Statements[0] as IfStatementSyntax;

                if (ifBlockStatement != null)
                {
                    var ifBlock = ifBlockStatement.Statement as BlockSyntax;
                    if (ifBlock != null)
                    {
                        ifBlockStatements = ifBlock.Statements;
                    }
                }
            }

            var localDeclaration = CodeFixHelper.TriviaCountHelper(generator, name, ifBlockStatements) as StatementSyntax;

            var oldBlock = ifStatement.Statement as BlockSyntax;
            StatementSyntax oldStatement = oldBlock.Statements[0];
            SyntaxList<StatementSyntax> newStatements = oldBlock.Statements.Replace(oldStatement, localDeclaration);
            BlockSyntax newBlock = oldBlock.WithStatements(newStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // adds the trailing trivia variable
        private async Task<Document> TrailingVarMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifStatement = declaration.Parent.Parent as IfStatementSyntax;
            var localDeclaration = new SyntaxList<SyntaxNode>().Add(CodeFixHelper.TriviaVarMissingHelper(generator, ifStatement));

            var oldBlock = declaration.Statement as BlockSyntax;
            BlockSyntax newBlock = oldBlock.WithStatements(localDeclaration);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // replaces the trailing trivia variable
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
            StatementSyntax oldStatement = oldBlock.Statements[0];
            SyntaxList<StatementSyntax> newStatements = oldBlock.Statements.Replace(oldStatement, localDeclaration);
            BlockSyntax newBlock = oldBlock.WithStatements(newStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // replaces the trailing trivia kind check
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
            StatementSyntax oldStatement = oldBlock.Statements[1];
            SyntaxList<StatementSyntax> newStatements = oldBlock.Statements.Replace(oldStatement, newIfStatement);
            BlockSyntax newBlock = oldBlock.WithStatements(newStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // adds the trailing trivia kind check
        private async Task<Document> TrailingKindCheckMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifBlockStatements = new SyntaxList<SyntaxNode>();
            var leadingTrivia = SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Checks that the single trailing trivia is of kind whitespace (as opposed to a comment for example)").ElementAt(0), SyntaxFactory.CarriageReturnLineFeed);
            var newIfStatement = (CodeFixHelper.TriviaKindCheckHelper(generator, declaration, ifBlockStatements) as StatementSyntax).WithLeadingTrivia(leadingTrivia);

            var oldBlock = declaration.Statement as BlockSyntax;
            BlockSyntax newBlock = oldBlock.AddStatements(newIfStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // replaces the whitespace check
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
            StatementSyntax oldStatement = oldBlock.Statements[0];
            SyntaxList<StatementSyntax> newStatements = oldBlock.Statements.Replace(oldStatement, newIfStatement);
            BlockSyntax newBlock = oldBlock.WithStatements(newStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // adds the whitespace check
        private async Task<Document> WhitespaceCheckMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifBlockStatements = new SyntaxList<SyntaxNode>();
            var leadingTrivia = SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Finally, this statement checks that the trailing trivia is one single space").ElementAt(0), SyntaxFactory.CarriageReturnLineFeed);
            var newIfStatement = new SyntaxList<SyntaxNode>().Add((CodeFixHelper.WhitespaceCheckHelper(generator, declaration, ifBlockStatements) as StatementSyntax).WithLeadingTrivia(leadingTrivia));

            var oldBlock = declaration.Statement as BlockSyntax;
            BlockSyntax newBlock = oldBlock.WithStatements(newIfStatement);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // replaces the return statement
        private async Task<Document> ReturnIncorrectAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

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
            SyntaxList<StatementSyntax> newStatements = oldBlock.Statements.Replace(oldBlock.Statements[0], returnStatement.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', it will return from this method without reporting a diagnostic").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            BlockSyntax newBlock = oldBlock.WithStatements(newStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // adds the return statement
        private async Task<Document> ReturnMissingAsync(Document document, IfStatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var returnStatements = new SyntaxList<SyntaxNode>().Add(generator.ReturnStatement().WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', it will return from this method without reporting a diagnostic").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            var oldBlock = declaration.Statement as BlockSyntax;
            BlockSyntax newBlock = oldBlock.WithStatements(returnStatements);

            return await ReplaceNode(oldBlock, newBlock, document);
        }

        // replaces the open paren statement
        private async Task<Document> ReplaceOpenParenAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string expressionString = CodeFixHelper.GetIfStatementName(methodDeclaration.Body);

            SyntaxNode openParen = CodeFixHelper.CreateOpenParen(generator, expressionString);

            return await ReplaceNode(declaration, openParen.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Extracts the opening parenthesis of the if-statement condition").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // adds the open paren statement
        private async Task<Document> AddOpenParenAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string expressionString = CodeFixHelper.GetIfStatementName(declaration.Body);
            SyntaxNode openParen = CodeFixHelper.CreateOpenParen(generator, expressionString);
            SyntaxNode newMethod = CodeFixHelper.AddStatementToMethod(generator, declaration, openParen.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Extracts the opening parenthesis of the if-statement condition").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(declaration, newMethod, document);
        }

        // replaces the start span statement
        private async Task<Document> ReplaceStartSpanAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string identifierString = CodeFixHelper.GetIfKeywordName(methodDeclaration.Body);
            SyntaxNode startSpan = CodeFixHelper.CreateEndOrStartSpan(generator, identifierString, "startDiagnosticSpan");

            return await ReplaceNode(declaration, startSpan.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Determines the start of the span of the diagnostic that will be reported, ie the start of the squiggle").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // adds the start span statement
        private async Task<Document> AddStartSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string identifierString = CodeFixHelper.GetIfKeywordName(declaration.Body);
            SyntaxNode startSpan = CodeFixHelper.CreateEndOrStartSpan(generator, identifierString, "startDiagnosticSpan");
            SyntaxNode newMethod = CodeFixHelper.AddStatementToMethod(generator, declaration, startSpan.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Determines the start of the span of the diagnostic that will be reported, ie the start of the squiggle").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(declaration, newMethod, document);
        }

        // replaces the end span statement
        private async Task<Document> ReplaceEndSpanAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string identifierString = CodeFixHelper.GetOpenParenName(methodDeclaration);

            SyntaxNode endSpan = CodeFixHelper.CreateEndOrStartSpan(generator, identifierString, "endDiagnosticSpan");

            return await ReplaceNode(declaration, endSpan.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Determines the end of the span of the diagnostic that will be reported").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // adds the end span statement
        private async Task<Document> AddEndSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string identifierString = CodeFixHelper.GetOpenParenName(declaration);
            SyntaxNode endSpan = CodeFixHelper.CreateEndOrStartSpan(generator, identifierString, "endDiagnosticSpan");
            SyntaxNode newMethod = CodeFixHelper.AddStatementToMethod(generator, declaration, endSpan.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Determines the end of the span of the diagnostic that will be reported").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(declaration, newMethod, document);
        }

        // replaces the span statement
        private async Task<Document> ReplaceSpanAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string startIdentifier = CodeFixHelper.GetStartSpanName(methodDeclaration);
            string endIdentifier = CodeFixHelper.GetEndSpanName(methodDeclaration);

            SyntaxNode span = CodeFixHelper.CreateSpan(generator, startIdentifier, endIdentifier);

            return await ReplaceNode(declaration, span.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// The span is the range of integers that define the position of the characters the red squiggle will underline").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // adds the span statement
        private async Task<Document> AddSpanAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string startIdentifier = CodeFixHelper.GetStartSpanName(declaration);
            string endIdentifier = CodeFixHelper.GetEndSpanName(declaration);
            SyntaxNode span = CodeFixHelper.CreateSpan(generator, startIdentifier, endIdentifier);
            SyntaxNode newMethod = CodeFixHelper.AddStatementToMethod(generator, declaration, span.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// The span is the range of integers that define the position of the characters the red squiggle will underline").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(declaration, newMethod, document);
        }

        // replaces the location statement
        private async Task<Document> ReplaceLocationAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            string ifStatementIdentifier = CodeFixHelper.GetIfStatementName(methodDeclaration.Body);
            string spanIdentifier = CodeFixHelper.GetSpanName(methodDeclaration);
            SyntaxNode location = CodeFixHelper.CreateLocation(generator, ifStatementIdentifier, spanIdentifier);

            return await ReplaceNode(declaration, location.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Uses the span created above to create a location for the diagnostic squiggle to appear within the syntax tree passed in as an argument").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // adds the location statement
        private async Task<Document> AddLocationAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string ifStatementIdentifier = CodeFixHelper.GetIfStatementName(declaration.Body);
            string spanIdentifier = CodeFixHelper.GetSpanName(declaration);
            SyntaxNode location = CodeFixHelper.CreateLocation(generator, ifStatementIdentifier, spanIdentifier);
            SyntaxNode newMethod = CodeFixHelper.AddStatementToMethod(generator, declaration, location.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Uses the span created above to create a location for the diagnostic squiggle to appear within the syntax tree passed in as an argument").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(declaration, newMethod, document);
        }

        // replace the diagnostic creation statement
        private async Task<Document> ReplaceDiagnosticAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();
            var classDeclaration = methodDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().First();

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string locationName = CodeFixHelper.GetLocationName(methodDeclaration);
            string ruleName = CodeFixHelper.GetFirstRuleName(classDeclaration);
            if (ruleName == null)
            {
                return document;
            }

            SyntaxNode diagnostic = CodeFixHelper.CreateDiagnostic(generator, locationName, ruleName);

            return await ReplaceNode(declaration, diagnostic.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Holds the diagnostic and all necessary information to be reported").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // adds the diagnostic creation statement
        private async Task<Document> AddDiagnosticAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string ruleName = CodeFixHelper.GetFirstRuleName(declaration);
            if (ruleName == null)
            {
                return document;
            }

            MethodDeclarationSyntax analysis = CodeFixHelper.GetAnalysis(declaration);
            if (analysis == null)
            {
                return document;
            }

            string locationName = CodeFixHelper.GetLocationName(analysis);

            SyntaxNode diagnostic = CodeFixHelper.CreateDiagnostic(generator, locationName, ruleName);
            SyntaxNode newMethod = CodeFixHelper.AddStatementToMethod(generator, analysis, diagnostic.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Holds the diagnostic and all necessary information to be reported").ElementAt(0), SyntaxFactory.EndOfLine(" \r\n"))));

            return await ReplaceNode(analysis, newMethod, document);
        }

        // replaces the diagnostic report statement
        private async Task<Document> ReplaceDiagnosticReportAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            var methodDeclaration = declaration.Ancestors().OfType<MethodDeclarationSyntax>().First();

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string argumentName = CodeFixHelper.GetDiagnosticName(methodDeclaration);
            string contextName = CodeFixHelper.GetContextParameter(methodDeclaration);

            SyntaxNode diagnosticReport = CodeFixHelper.CreateDiagnosticReport(generator, argumentName, contextName);

            return await ReplaceNode(declaration, diagnosticReport.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Sends diagnostic information to the IDE to be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // adds the diagnostic report statement
        private async Task<Document> AddDiagnosticReportAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string argumentName = CodeFixHelper.GetDiagnosticName(declaration);
            string contextName = CodeFixHelper.GetContextParameter(declaration);
            SyntaxNode diagnosticReport = CodeFixHelper.CreateDiagnosticReport(generator, argumentName, contextName);
            SyntaxNode newMethod = CodeFixHelper.AddStatementToMethod(generator, declaration, diagnosticReport.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Sends diagnostic information to the IDE to be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));

            return await ReplaceNode(declaration, newMethod, document);
        }
        #endregion

        #region id code fixes
        // adds an id to the class
        private async Task<Document> MissingIdAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode newField = CodeFixHelper.NewIdCreator(generator, "spacingRuleId", "IfSpacing001").WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs").ElementAt(0), SyntaxFactory.EndOfLine("\r\n")));
            var newClass = generator.InsertMembers(declaration, 0, newField) as ClassDeclarationSyntax;
            ClassDeclarationSyntax triviaClass = newClass.ReplaceNode(newClass.Members[0], newField);

            return await ReplaceNode(declaration, triviaClass, document);
        }
        #endregion

        #region initialize code fixes
        // adds the Initialize method
        private async Task<Document> MissingInitAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SemanticModel semanticModel = await document.GetSemanticModelAsync();

            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();
            string name = "context";
            SyntaxNode initializeDeclaration = CodeFixHelper.BuildInitialize(generator, notImplementedException, statements, name);
            SyntaxNode newClassDeclaration = generator.AddMembers(declaration, initializeDeclaration);

            return await ReplaceNode(declaration, newClassDeclaration, document);
        }

        // adds a register statement and analysis method if necessary
        private async Task<Document> MissingRegisterAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            ClassDeclarationSyntax classDeclaration = declaration.Parent as ClassDeclarationSyntax;
            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            bool newAnalysisRequired = false;

            string methodName = CodeFixHelper.GetExistingAnalysisMethodName(classDeclaration);

            if (methodName == null)
            {
                methodName = "AnalyzeIfStatement";
                newAnalysisRequired = true;
            }

            SyntaxNode invocationExpression = CodeFixHelper.CreateRegister(generator, declaration, methodName);
            SyntaxList<SyntaxNode> statements = new SyntaxList<SyntaxNode>().Add(invocationExpression.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Calls the method (first argument) to perform analysis whenever a SyntaxNode of kind IfStatement is found").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))));
            SyntaxNode newInitializeMethod = generator.MethodDeclaration("Initialize", declaration.ParameterList.Parameters, accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, statements: statements);
            ClassDeclarationSyntax newClassDecl = classDeclaration.ReplaceNode(declaration, newInitializeMethod);

            if (newAnalysisRequired)
            {
                SyntaxNode newAnalysisMethod = CodeFixHelper.CreateAnalysisMethod(generator, methodName, semanticModel);
                newClassDecl = generator.AddMembers(newClassDecl, newAnalysisMethod) as ClassDeclarationSyntax;
            }

            return await ReplaceNode(classDeclaration, newClassDecl, document);
        }

        // gets ride of multiple statement inside Initialize, keeping one correct statement
        private async Task<Document> MultipleStatementsAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();
            SyntaxList<StatementSyntax> initializeStatements = declaration.Body.Statements;

            foreach (ExpressionStatementSyntax statement in initializeStatements)
            {
                bool correctRegister = CodeFixHelper.IsCorrectRegister(statement);

                if (correctRegister)
                {
                    statements = statements.Add(statement);
                    break;
                }
            }

            BlockSyntax newBlock = declaration.Body;
            newBlock = newBlock.WithStatements(statements);
            MethodDeclarationSyntax newDeclaration = declaration.WithBody(newBlock);

            return await ReplaceNode(declaration, newDeclaration, document);
        }

        // removes the invalid statement from the method
        private async Task<Document> InvalidStatementAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxNode newInitializeDeclaration = CodeFixHelper.RemoveStatement(declaration);
            return await ReplaceNode(declaration.Ancestors().OfType<MethodDeclarationSyntax>().First(), newInitializeDeclaration, document);
        }

        // replaces the old Initialize declaration with one with a correct signature
        private async Task<Document> IncorrectSigAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string name = CodeFixHelper.GetContextParameter(declaration);
            SyntaxList<StatementSyntax> statements = declaration.Body.Statements;
            SyntaxNode initializeDeclaration = CodeFixHelper.BuildInitialize(generator, null, statements, name);

            return await ReplaceNode(declaration, initializeDeclaration, document);
        }

        // puts the correct arguments in the register statement
        private async Task<Document> CorrectArgumentsAsync(Document document, InvocationExpressionSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string methodName = CodeFixHelper.GetRegisterMethodName(declaration);

            ClassDeclarationSyntax classDeclaration = declaration.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
            methodName = CodeFixHelper.GetExistingAnalysisMethodName(classDeclaration);

            if (methodName == null)
            {
                methodName = "AnalyzeIfStatement";
            }

            SyntaxNode statement = CodeFixHelper.CreateRegister(generator, declaration.Ancestors().OfType<MethodDeclarationSyntax>().First(), methodName);
            SyntaxNode expression = generator.ExpressionStatement(statement);

            return await ReplaceNode(declaration.Parent, expression.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Calls the method (first argument) to perform analysis whenever a SyntaxNode of kind IfStatement is found").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // corrects the register statement to be RegisterSyntaxNodeAction
        private async Task<Document> CorrectRegisterAsync(Document document, IdentifierNameSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var memberExpression = declaration.Parent as MemberAccessExpressionSyntax;
            var invocationExpression = memberExpression.Parent as InvocationExpressionSyntax;
            string methodName = CodeFixHelper.GetRegisterMethodName(invocationExpression);
            if (methodName == null)
            {
                methodName = "AnalyzeIfStatement";
            }

            SyntaxNode newExpression = CodeFixHelper.CreateRegister(generator, declaration.Ancestors().OfType<MethodDeclarationSyntax>().First(), methodName);
            return await ReplaceNode(declaration.FirstAncestorOrSelf<ExpressionStatementSyntax>(), newExpression.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// Calls the method (first argument) to perform analysis whenever a SyntaxNode of kind IfStatement is found").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))), document);
        }

        // corrects the kind argument of the register statement to be SyntaxKind.IfStatement
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

            ArgumentListSyntax argList = SyntaxFactory.ArgumentList(arguments);
            string contextParameter = (((declaration.Parent as InvocationExpressionSyntax).Expression as MemberAccessExpressionSyntax).Expression as IdentifierNameSyntax).Identifier.Text;
            SyntaxNode newExpr = CodeFixHelper.BuildRegister(generator, contextParameter, "RegisterSyntaxNodeAction", argList);

            return await ReplaceNode(declaration.Ancestors().OfType<InvocationExpressionSyntax>().First(), newExpr.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.Whitespace("            "), SyntaxFactory.ParseLeadingTrivia("// Calls the method (first argument) to perform analysis whenever a SyntaxNode of kind IfStatement is found").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("            "))), document);
        }
        #endregion

        #region rule code fixes
        // adds the internal and static modifiers to the property
        private async Task<Document> InternalStaticAsync(Document document, FieldDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode newFieldDecl = generator.FieldDeclaration(declaration.Declaration.Variables[0].Identifier.Text, generator.IdentifierName("DiagnosticDescriptor"), accessibility: Accessibility.Internal, modifiers: DeclarationModifiers.Static, initializer: declaration.Declaration.Variables[0].Initializer.Value as SyntaxNode).WithLeadingTrivia(declaration.GetLeadingTrivia()).WithTrailingTrivia(declaration.GetTrailingTrivia());
            return await ReplaceNode(declaration, newFieldDecl, document);
        }

        // sets the isEnabledByDefault parameter to true
        private async Task<Document> EnabledByDefaultAsync(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            FieldDeclarationSyntax newRule = rule.ReplaceNode(argument.Expression, generator.LiteralExpression(true));

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// isEnabledByDefault: Determines whether the analyzer is enabled by default or if the user must manually enable it. Generally set to true").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
        }

        // sets the diagnosticSeverity parameter to warning
        private async Task<Document> DiagnosticSeverityWarning(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            var newExpression = generator.MemberAccessExpression(expression, "Warning") as ExpressionSyntax;
            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            FieldDeclarationSyntax newRule = rule.ReplaceNode(argument.Expression, newExpression);

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
        }

        // sets the diagnosticSeverity parameter to hidden
        private async Task<Document> DiagnosticSeverityHidden(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            var newExpression = generator.MemberAccessExpression(expression, "Hidden") as ExpressionSyntax;
            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            FieldDeclarationSyntax newRule = rule.ReplaceNode(argument.Expression, newExpression);

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
        }

        // sets the diagnosticSeverity parameter to info
        private async Task<Document> DiagnosticSeverityInfo(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            var newExpression = generator.MemberAccessExpression(expression, "Info") as ExpressionSyntax;
            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            FieldDeclarationSyntax newRule = rule.ReplaceNode(argument.Expression, newExpression);

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
        }

        // sets the diagnosticSeverity parameter to error
        private async Task<Document> DiagnosticSeverityError(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            var newExpression = generator.MemberAccessExpression(expression, "Error") as ExpressionSyntax;
            var rule = argument.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            FieldDeclarationSyntax newRule = rule.ReplaceNode(argument.Expression, newExpression);

            return await ReplaceNode(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>(), newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(argument.FirstAncestorOrSelf<FieldDeclarationSyntax>().GetLeadingTrivia()), document);
        }

        // adds a declaration for the id
        private async Task<Document> MissingIdDeclarationAsync(Document document, VariableDeclaratorSyntax ruleDeclarationField, CancellationToken c)
        {
            var classDeclaration = ruleDeclarationField.Parent.Parent.Parent as ClassDeclarationSyntax;
            var objectCreationSyntax = ruleDeclarationField.Initializer.Value as ObjectCreationExpressionSyntax;
            ArgumentListSyntax ruleArgumentList = objectCreationSyntax.ArgumentList;

            string currentRuleId = null;
            for (int i = 0; i < ruleArgumentList.Arguments.Count; i++)
            {
                ArgumentSyntax currentArg = ruleArgumentList.Arguments[i];
                string currentArgName = currentArg.NameColon.Name.Identifier.Text;
                if (currentArgName == "id")
                {
                    var currentRuleIdentifier = currentArg.Expression as IdentifierNameSyntax;
                    currentRuleId = currentRuleIdentifier.Identifier.Text;
                    break;
                }
            }

            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode newField = CodeFixHelper.NewIdCreator(generator, currentRuleId, "DescriptiveId").WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs").ElementAt(0), SyntaxFactory.EndOfLine("\r\n")));

            var newClass = generator.InsertMembers(classDeclaration, 0, newField) as ClassDeclarationSyntax;
            ClassDeclarationSyntax triviaClass = newClass.ReplaceNode(newClass.Members[0], newField);

            return await ReplaceNode(classDeclaration, triviaClass, document);
        }

        // corrects the id declaration
        private async Task<Document> IdDeclTypeAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxList<MemberDeclarationSyntax> members = classDeclaration.Members;
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

                var fieldType = fieldDeclaration.Declaration.Type as IdentifierNameSyntax;
                if (fieldType != null && fieldType.Identifier.Text == "DiagnosticDescriptor")
                {
                    rule = fieldDeclaration;

                    var declaratorSyntax = fieldDeclaration.Declaration.Variables[0] as VariableDeclaratorSyntax;
                    var objectCreationSyntax = declaratorSyntax.Initializer.Value as ObjectCreationExpressionSyntax;
                    ArgumentListSyntax ruleArgumentList = objectCreationSyntax.ArgumentList;

                    for (int i = 0; i < ruleArgumentList.Arguments.Count; i++)
                    {
                       ArgumentSyntax currentArg = ruleArgumentList.Arguments[i];
                        string currentArgName = currentArg.NameColon.Name.Identifier.Text;
                        if (currentArgName == "id")
                        {
                            oldIdName = currentArg.Expression;
                            break;
                        }
                    }

                    continue;
                }

                SyntaxTokenList modifiers = fieldDeclaration.Modifiers;
                if (modifiers == null)
                {
                    continue;
                }

                bool isPublic = false;
                bool isConst = false;

                foreach (SyntaxToken modifier in modifiers)
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
                    var ruleIdSyntax = fieldDeclaration.Declaration.Variables[0] as VariableDeclaratorSyntax;
                    string newIdIdentifier = ruleIdSyntax.Identifier.Text;
                    newIdName = generator.IdentifierName(newIdIdentifier) as IdentifierNameSyntax;
                }
            }

            SyntaxNode newArg = generator.Argument("id", RefKind.None, newIdName).WithLeadingTrivia(SyntaxFactory.Whitespace("            "));
            FieldDeclarationSyntax newRule = rule.ReplaceNode(oldIdName.Ancestors().OfType<ArgumentSyntax>().First(), newArg);

            return await ReplaceNode(rule, newRule.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseTrailingTrivia("// id: Identifies each rule. Same as the public constant declared above").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"))).WithLeadingTrivia(rule.GetLeadingTrivia()), document);
        }

        // adds a DiagnosticDescriptor rule to the class
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

            var newNodes = new SyntaxList<SyntaxNode>();
            newNodes = newNodes.Add(fieldDeclaration.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseLeadingTrivia("// If the analyzer finds an issue, it will report the DiagnosticDescriptor rule").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"), SyntaxFactory.Whitespace("        "))));

            SyntaxNode root = await document.GetSyntaxRootAsync();
            if (insertPointNode != null)
            {
                SyntaxNode newRoot = root.InsertNodesBefore(insertPointNode, newNodes);
                Document newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
            else
            {
                SyntaxNode newRoot = root.ReplaceNode(declaration, declaration.AddMembers(fieldDeclaration.WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.Whitespace("        "), SyntaxFactory.ParseLeadingTrivia("// If the analyzer finds an issue, it will report the DiagnosticDescriptor rule").ElementAt(0), SyntaxFactory.EndOfLine("\r\n")))));
                Document newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
        }
        #endregion

        #region supported diagnostics code fixes
        // fixes the signature of the SupportedDiagnostics property
        private async Task<Document> IncorrectSigSuppDiagAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode type = generator.IdentifierName("ImmutableArray<DiagnosticDescriptor>");
            var getAccessorStatements = new SyntaxList<SyntaxNode>();
            foreach (AccessorDeclarationSyntax accessor in declaration.AccessorList.Accessors)
            {
                if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                {
                    getAccessorStatements = accessor.Body.Statements;
                    break;
                }
            }

            SyntaxNode newPropertyDecl = generator.PropertyDeclaration("SupportedDiagnostics", type, accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, getAccessorStatements: getAccessorStatements).WithLeadingTrivia(declaration.GetLeadingTrivia()).WithTrailingTrivia(declaration.GetTrailingTrivia());
            newPropertyDecl = newPropertyDecl.RemoveNode((newPropertyDecl as PropertyDeclarationSyntax).AccessorList.Accessors[1], 0);
            
            return await ReplaceNode(declaration, newPropertyDecl, document);
        }

        // adds the get accessor
        private async Task<Document> MissingAccessorAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            SemanticModel semanticModel = await document.GetSemanticModelAsync();

            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            SyntaxNode[] throwStatement = new[] { generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)) };
            SyntaxNode type = generator.GetType(declaration);
            PropertyDeclarationSyntax newPropertyDeclaration = generator.PropertyDeclaration("SupportedDiagnostics", type, Accessibility.Public, DeclarationModifiers.Override, throwStatement) as PropertyDeclarationSyntax;
            newPropertyDeclaration = newPropertyDeclaration.RemoveNode(newPropertyDeclaration.AccessorList.Accessors[1], 0);

            return await ReplaceNode(declaration, newPropertyDeclaration, document);
        }

        // removes all unnecessary accessors
        private async Task<Document> TooManyAccessorsAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var allAccessors = declaration.AccessorList.Accessors.OfType<AccessorDeclarationSyntax>();
            bool foundGetAccessor = false;
            AccessorDeclarationSyntax accessorToKeep = null;
            AccessorListSyntax accessorList = declaration.AccessorList;

            foreach (AccessorDeclarationSyntax accessor in allAccessors)
            {
                SyntaxToken keyword = accessor.Keyword;
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

            BlockSyntax block = SyntaxFactory.Block(new StatementSyntax[0]);
            if (accessorToKeep == null)
            {
                accessorToKeep = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, block);
            }
            
            SyntaxList<SyntaxNode> accessorsToAdd = new SyntaxList<SyntaxNode>();
            accessorsToAdd = accessorsToAdd.Add(accessorToKeep);
            PropertyDeclarationSyntax newPropertyDeclaration = declaration.WithAccessorList(null);
            newPropertyDeclaration = generator.AddAccessors(newPropertyDeclaration, accessorsToAdd) as PropertyDeclarationSyntax;

            return await ReplaceNode(declaration, newPropertyDeclaration, document);
        }

        // inserts ImmutableArray.Create()
        private async Task<Document> AccessorReturnValueAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expressionString = generator.IdentifierName("ImmutableArray");
            SyntaxNode identifierString = generator.IdentifierName("Create");
            SyntaxNode expression = generator.MemberAccessExpression(expressionString, identifierString);
            SyntaxNode invocationExpression = generator.InvocationExpression(expression);
            ReturnStatementSyntax returnStatement = (generator.ReturnStatement(invocationExpression) as ReturnStatementSyntax).WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// This array contains all the diagnostics that can be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n")));

            SyntaxList<AccessorDeclarationSyntax> accessors = declaration.AccessorList.Accessors;
            if (accessors == null || accessors.Count == 0)
            {
                return document;
            }

            AccessorDeclarationSyntax firstAccessor = declaration.AccessorList.Accessors.First();
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

            var oldStatementDeclaration = oldStatement as LocalDeclarationStatementSyntax;

            SyntaxNode root = await document.GetSyntaxRootAsync();
            SyntaxNode newRoot = root;

            if (oldStatement == null)
            {
                var newAccessorDeclaration = firstAccessor.AddBodyStatements(returnStatement);
                newRoot = root.ReplaceNode(firstAccessor, newAccessorDeclaration);
            }
            else if (oldStatementDeclaration != null)
            {
                var oldStatementDeclarator = oldStatementDeclaration.Declaration.Variables[0] as VariableDeclaratorSyntax;
                SyntaxNode oldVariableName = generator.IdentifierName(oldStatementDeclarator.Identifier.Text);
                var newStatementDeclaration = generator.LocalDeclarationStatement(oldStatementDeclarator.Identifier.Text, invocationExpression).WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// This array contains all the diagnostics that can be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n")));
                newRoot = root.ReplaceNode(oldStatement, newStatementDeclaration);
            }
            else
            {
                newRoot = root.ReplaceNode(oldStatement, returnStatement);
            }

            Document newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        // inserts ImmutableArray.Create(all rules)
        private async Task<Document> AccessorWithRulesAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            List<string> ruleNames = CodeFixHelper.GetAllRuleNames(declaration);

            var propertyMembers = declaration.Members.OfType<PropertyDeclarationSyntax>();
            foreach (PropertyDeclarationSyntax propertySyntax in propertyMembers)
            {
                if (propertySyntax.Identifier.Text != "SupportedDiagnostics")
                {
                    continue;
                }

                SyntaxList<SyntaxNode> nodeArgs = CodeFixHelper.CreateRuleList(document, ruleNames);
                SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
                var newInvocationExpression = (generator.InvocationExpression(generator.MemberAccessExpression(generator.IdentifierName("ImmutableArray"), "Create"), nodeArgs));
                var leadingTrivia = SyntaxFactory.TriviaList(SyntaxFactory.ParseLeadingTrivia("// This array contains all the diagnostics that can be shown to the user").ElementAt(0), SyntaxFactory.EndOfLine("\r\n"));
                var newReturnStatement = generator.ReturnStatement(newInvocationExpression).WithLeadingTrivia(leadingTrivia);
                AccessorDeclarationSyntax getAccessor = propertySyntax.AccessorList.Accessors.First();

                if (getAccessor.Body.Statements.Count == 0)
                {
                    return await ReplaceNode(getAccessor, getAccessor.AddBodyStatements(newReturnStatement as ReturnStatementSyntax), document);
                }

                var localDeclaration = getAccessor.Body.Statements.First() as LocalDeclarationStatementSyntax;
                var returnStatement = getAccessor.Body.Statements.First() as ReturnStatementSyntax;
                var otherStatement = getAccessor.Body.Statements.First();

                if (localDeclaration != null)
                {
                    return await ReplaceNode(localDeclaration, generator.LocalDeclarationStatement(localDeclaration.Declaration.Variables[0].Identifier.Text, newInvocationExpression).WithLeadingTrivia(leadingTrivia), document);
                }
                else if (returnStatement != null)
                {
                    return await ReplaceNode(returnStatement, newReturnStatement, document);
                }
                else if (otherStatement != null)
                {
                    return await ReplaceNode(otherStatement, newReturnStatement, document);
                }
            }

           return document;
        }

        // adds a SupportedDiagnostics property to the class
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

            SemanticModel semanticModel = await document.GetSemanticModelAsync();
            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            PropertyDeclarationSyntax propertyDeclaration = CodeFixHelper.CreateSupportedDiagnostics(generator, notImplementedException);

            var newNodes = new SyntaxList<SyntaxNode>();
            newNodes = newNodes.Add(propertyDeclaration);

            SyntaxNode root = await document.GetSyntaxRootAsync();
            if (insertPoint != null)
            {
                SyntaxNode newRoot = root.InsertNodesBefore(insertPointNode, newNodes);
                Document newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
            else
            {
                SyntaxNode newRoot = root.ReplaceNode(declaration, declaration.AddMembers(propertyDeclaration));
                Document newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
        }
        #endregion

        class CodeFixHelper
        {
            // removes the provided statement from the method that it is in
            protected internal static SyntaxNode RemoveStatement(StatementSyntax statement)
            {
                MethodDeclarationSyntax initializeDeclaration = statement.Ancestors().OfType<MethodDeclarationSyntax>().First();
                MethodDeclarationSyntax newInitializeDeclaration = initializeDeclaration.RemoveNode(statement, 0);
                return newInitializeDeclaration as SyntaxNode;
            }
            
            // checks if the statement is a correct regsiter statement
            protected internal static bool IsCorrectRegister(ExpressionStatementSyntax statement)
            {
                var expression = statement.Expression as InvocationExpressionSyntax;
                if (expression == null)
                {
                    return false;
                }

                var expressionStart = expression.Expression as MemberAccessExpressionSyntax;
                if (expressionStart == null || expressionStart.Name == null || expressionStart.Name.Identifier.Text != "RegisterSyntaxNodeAction")
                {
                    return false;
                }

                if (expression.ArgumentList == null || expression.ArgumentList.Arguments.Count() != 2)
                {
                    return false;
                }

                var argumentMethod = expression.ArgumentList.Arguments[0].Expression as IdentifierNameSyntax;
                if (argumentMethod == null || argumentMethod.Identifier == null)
                {
                    return false;
                }

                var argumentKind = expression.ArgumentList.Arguments[1].Expression as MemberAccessExpressionSyntax;
                if (argumentKind == null || argumentKind.Name == null || argumentKind.Name.Identifier.Text != "IfStatement")
                {
                    return false;
                }

                var preArgumentKind = argumentKind.Expression as IdentifierNameSyntax;
                if (preArgumentKind.Identifier == null || preArgumentKind.Identifier.ValueText != "SyntaxKind")
                {
                    return false;
                }

                return true;
            }
            
            // gets the name of the span variable
            protected internal static string GetSpanName(MethodDeclarationSyntax methodDecl)
            {
                string spanName = (methodDecl.Body.Statements[6] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
                return spanName;
            }
            
            // gets the name of the start span variable
            protected internal static string GetStartSpanName(MethodDeclarationSyntax methodDecl)
            {
                string startIdentifier = (methodDecl.Body.Statements[4] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
                return startIdentifier;
                
            }

            // gets the name of the end span variable
            protected internal static string GetEndSpanName(MethodDeclarationSyntax methodDecl)
            {
                string endIdentifier = (methodDecl.Body.Statements[5] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
                return endIdentifier;
            }

            // gets the name of the open paren variable
            protected internal static string GetOpenParenName(MethodDeclarationSyntax methodDecl)
            {
                string openParenName = (methodDecl.Body.Statements[3] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
                return openParenName;
            }
            
            // gets the name of the location variable
            protected internal static string GetLocationName(MethodDeclarationSyntax methodDecl)
            {
                string locationName = (methodDecl.Body.Statements[7] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
                return locationName;
            }

            // adds a statement to the provided method
            protected internal static SyntaxNode AddStatementToMethod(SyntaxGenerator generator, MethodDeclarationSyntax methodDecl, SyntaxNode statement)
            {
                var oldStatements = (SyntaxList<SyntaxNode>)methodDecl.Body.Statements;
                SyntaxList<SyntaxNode> newStatements = oldStatements.Add(statement);
                SyntaxNode newMethod = generator.WithStatements(methodDecl, newStatements);
                return newMethod;
            }

            // gets the name of the diagnostic variable
            protected internal static string GetDiagnosticName(MethodDeclarationSyntax methodDecl)
            {
                string diagnosticName = (methodDecl.Body.Statements[8] as LocalDeclarationStatementSyntax).Declaration.Variables[0].Identifier.Text;
                return diagnosticName;
            }
            
            // gets the context parameter of the analysis method
            protected internal static string GetContextParameter(MethodDeclarationSyntax methodDecl)
            {
                string contextName = methodDecl.ParameterList.Parameters[0].Identifier.Text;
                return contextName;
            }
            
            // builds a register statement
            protected internal static SyntaxNode BuildRegister(SyntaxGenerator generator, string context, string register, ArgumentListSyntax argumentList)
            {
                SyntaxNode registerIdentifier = generator.IdentifierName(register);
                SyntaxNode contextIdentifier = generator.IdentifierName(context);
                SyntaxNode memberExpr = generator.MemberAccessExpression(contextIdentifier, registerIdentifier);
                SyntaxNode invocationExpr = generator.InvocationExpression(memberExpr, argumentList.Arguments);
                return invocationExpr;
            }

            // gets the name of the method registered, null if none found
            protected internal static string GetRegisterMethodName(InvocationExpressionSyntax invocationExpression)
            {
                string methodName = null;
                ArgumentListSyntax argList = invocationExpression.ArgumentList;
                if (argList != null)
                {
                    SeparatedSyntaxList<ArgumentSyntax> args = argList.Arguments;
                    if (args != null)
                    {
                        if (args.Count > 0)
                        {
                            ArgumentSyntax nameArg = args[0];
                            var name = nameArg.Expression as IdentifierNameSyntax;
                            if (name != null)
                            {
                                methodName = name.Identifier.Text;
                            }
                        }
                    }
                }

                return methodName;
            }
            
            // gets the name of the analysis method
            protected internal static string AnalysisMethodName(MethodDeclarationSyntax methodDeclaration)
            {
                var statements = methodDeclaration.Body.Statements.First() as ExpressionStatementSyntax;
                var invocationExpression = statements.Expression as InvocationExpressionSyntax;
                var methodIdentifier = invocationExpression.ArgumentList.Arguments[0].Expression as IdentifierNameSyntax;
                string methodName = methodIdentifier.Identifier.Text;
                return methodName;
            }
            
            // set method accessibility to accessibility
            protected internal static SyntaxNode MethodAccessibility(SyntaxGenerator generator, MethodDeclarationSyntax methodDeclaration, Accessibility accessibility)
            {
                SyntaxNode newMethod = generator.WithAccessibility(methodDeclaration, accessibility);
                return newMethod;
            }
            
            // set method return type to returnType
            protected internal static SyntaxNode MethodReturnType(MethodDeclarationSyntax methodDeclaration, string returnType)
            {
                TypeSyntax voidType = SyntaxFactory.ParseTypeName(returnType).WithTrailingTrivia(SyntaxFactory.Whitespace(" "));
                methodDeclaration = methodDeclaration.WithReturnType(voidType);
                return methodDeclaration;
            }

            // gets the name of the if-statement variable
            protected internal static string GetIfStatementName(BlockSyntax methodBlock)
            {
                var firstStatement = methodBlock.Statements[0] as LocalDeclarationStatementSyntax;
                string variableName = firstStatement.Declaration.Variables[0].Identifier.ValueText;
                return variableName;
            }

            // gets the name of the if-keyword variable
            protected internal static string GetIfKeywordName(BlockSyntax methodBlock)
            {
                var secondStatement = methodBlock.Statements[1] as LocalDeclarationStatementSyntax;
                string variableName = secondStatement.Declaration.Variables[0].Identifier.ValueText;
                return variableName;
            }

            // gets the name of the trailing trivia variable
            protected internal static string GetTrailingTriviaName(BlockSyntax ifBlock)
            {
                var trailingTriviaDeclaration = ifBlock.Statements[0] as LocalDeclarationStatementSyntax;
                string variableName = trailingTriviaDeclaration.Declaration.Variables[0].Identifier.ValueText;
                return variableName;
            }

            // gets the name of the first parameter of the method
            protected internal static string GetFirstParameterName(MethodDeclarationSyntax methodDeclaration)
            {
                var firstParameter = methodDeclaration.ParameterList.Parameters[0];
                string name = firstParameter.Identifier.Text;
                return name;
            }
            
            // creates an if-statement checking the count of trailing trivia
            protected internal static SyntaxNode TriviaCountHelper(SyntaxGenerator generator, string name, SyntaxList<StatementSyntax> ifBlockStatements)
            {
                SyntaxNode variableName = generator.IdentifierName(name);
                SyntaxNode memberAccess = generator.MemberAccessExpression(variableName, "TrailingTrivia");
                SyntaxNode fullMemberAccess = generator.MemberAccessExpression(memberAccess, "Count");
                SyntaxNode one = generator.LiteralExpression(1);
                SyntaxNode equalsExpression = generator.ValueEqualsExpression(fullMemberAccess, one);
                SyntaxNode newIfStatement = generator.IfStatement(equalsExpression, ifBlockStatements);

                return newIfStatement;
            }

            // creates a statement casting context.Node to if-statement
            protected internal static SyntaxNode IfHelper(SyntaxGenerator generator, string name)
            {
                TypeSyntax type = SyntaxFactory.ParseTypeName("IfStatementSyntax");
                SyntaxNode expression = generator.IdentifierName(name);
                SyntaxNode memberAccessExpression = generator.MemberAccessExpression(expression, "Node");
                SyntaxNode initializer = generator.CastExpression(type, memberAccessExpression);
                SyntaxNode ifStatement = generator.LocalDeclarationStatement("ifStatement", initializer);

                return ifStatement;
            }

            // creates the if-keyword statement
            protected internal static SyntaxNode KeywordHelper(SyntaxGenerator generator, BlockSyntax methodBlock)
            {
                string variableName = GetIfStatementName(methodBlock);
                SyntaxNode identifierName = generator.IdentifierName(variableName);
                SyntaxNode initializer = generator.MemberAccessExpression(identifierName, "IfKeyword");
                SyntaxNode ifKeyword = generator.LocalDeclarationStatement("ifKeyword", initializer);

                return ifKeyword;
            }
            
            // creates the HasTrailingTrivia check
            protected internal static SyntaxNode TriviaCheckHelper(SyntaxGenerator generator, BlockSyntax methodBlock, SyntaxList<StatementSyntax> ifBlockStatements)
            {
                string variableName = GetIfKeywordName(methodBlock);
                SyntaxNode identifierName = generator.IdentifierName(variableName);
                SyntaxNode conditional = generator.MemberAccessExpression(identifierName, "HasTrailingTrivia");
                SyntaxNode ifStatement = generator.IfStatement(conditional, ifBlockStatements);

                return ifStatement;
            }

            // creates the first trailing trivia variable
            protected internal static SyntaxNode TriviaVarMissingHelper(SyntaxGenerator generator, IfStatementSyntax declaration)
            {
                var methodDecl = declaration.Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
                var methodBlock = methodDecl.Body as BlockSyntax;

                string variableName = GetIfKeywordName(methodBlock);
                SyntaxNode identifierName = generator.IdentifierName(variableName);

                SyntaxNode ifTrailing = generator.MemberAccessExpression(identifierName, "TrailingTrivia");
                SyntaxNode fullVariable = generator.MemberAccessExpression(ifTrailing, "First");
                var parameters = new SyntaxList<SyntaxNode>();
                SyntaxNode variableExpression = generator.InvocationExpression(fullVariable, parameters);
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement("trailingTrivia", variableExpression);

                return localDeclaration;
            }

            // creates the trivia kind check
            protected internal static SyntaxNode TriviaKindCheckHelper(SyntaxGenerator generator, IfStatementSyntax ifStatement, SyntaxList<SyntaxNode> ifBlockStatements)
            {
                var ifBlock = ifStatement.Statement as BlockSyntax;
                string variableName = GetTrailingTriviaName(ifBlock);
                SyntaxNode identifierName = generator.IdentifierName(variableName);


                var arguments = new SyntaxList<SyntaxNode>();

                SyntaxNode whitespaceTrivia = generator.MemberAccessExpression(generator.IdentifierName("SyntaxKind"), "WhitespaceTrivia");
                arguments = arguments.Add(whitespaceTrivia);
                SyntaxNode trailingTriviaKind = generator.InvocationExpression(generator.MemberAccessExpression(identifierName, "IsKind"), arguments);

                SyntaxNode newIfStatement = generator.IfStatement(trailingTriviaKind, ifBlockStatements);

                return newIfStatement;
            }

            // creates the whitespace check
            protected internal static SyntaxNode WhitespaceCheckHelper(SyntaxGenerator generator, IfStatementSyntax ifStatement, SyntaxList<SyntaxNode> ifBlockStatements)
            {
                var ifBlock = ifStatement.Parent as BlockSyntax;
                string variableName = GetTrailingTriviaName(ifBlock);
                SyntaxNode identifierName = generator.IdentifierName(variableName);

                var arguments = new SyntaxList<SyntaxNode>();
                SyntaxNode trailingTriviaToString = generator.InvocationExpression(generator.MemberAccessExpression(identifierName, "ToString"), arguments);
                SyntaxNode rightSide = generator.LiteralExpression(" ");
                SyntaxNode equalsExpression = generator.ValueEqualsExpression(trailingTriviaToString, rightSide);

                SyntaxNode newIfStatement = generator.IfStatement(equalsExpression, ifBlockStatements);

                return newIfStatement;
            }

            // builds an Initialize method
            protected internal static SyntaxNode BuildInitialize(SyntaxGenerator generator, INamedTypeSymbol notImplementedException, SyntaxList<StatementSyntax> statements, string name)
            {
                TypeSyntax type = SyntaxFactory.ParseTypeName("AnalysisContext");
                SyntaxNode[] parameters = new[] { generator.ParameterDeclaration(name, type) };

                if (notImplementedException != null && statements.Count == 0)
                {
                    statements = statements.Add(generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)) as StatementSyntax);
                }

                SyntaxNode initializeDeclaration = generator.MethodDeclaration("Initialize", parameters: parameters, accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, statements: statements);
                return initializeDeclaration;
            }

            // creates a new id with the provided name as a literal expression
            protected internal static SyntaxNode NewIdCreator(SyntaxGenerator generator, string fieldName, string idName)
            {
                SyntaxNode initializer = generator.LiteralExpression(idName);
                SyntaxNode newField = generator.FieldDeclaration(fieldName, generator.TypeExpression(SpecialType.System_String), Accessibility.Public, DeclarationModifiers.Const, initializer);

                return newField;
            }

            // creates a variable creating a location for the diagnostic
            protected internal static SyntaxNode CreateLocation(SyntaxGenerator generator, string ifStatementIdentifier, string spanIdentifier)
            {
                string name = "diagnosticLocation";

                SyntaxNode memberIdentifier = generator.IdentifierName("Location");
                SyntaxNode memberName = generator.IdentifierName("Create");
                SyntaxNode expression = generator.MemberAccessExpression(memberIdentifier, memberName);

                SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();

                SyntaxNode treeIdentifier = generator.IdentifierName(ifStatementIdentifier);
                SyntaxNode treeArgExpression = generator.MemberAccessExpression(treeIdentifier, "SyntaxTree");
                SyntaxNode treeArg = generator.Argument(treeArgExpression);

                SyntaxNode spanArgIdentifier = generator.IdentifierName(spanIdentifier);
                SyntaxNode spanArg = generator.Argument(spanArgIdentifier);

                arguments = arguments.Add(treeArg);
                arguments = arguments.Add(spanArg);

                SyntaxNode initializer = generator.InvocationExpression(expression, arguments);
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

                return localDeclaration;
            }

            // creates a variable creating a span for the diagnostic
            protected internal static SyntaxNode CreateSpan(SyntaxGenerator generator, string startIdentifier, string endIdentifier)
            {
                string name = "diagnosticSpan";

                SyntaxNode memberIdentifier = generator.IdentifierName("TextSpan");
                SyntaxNode memberName = generator.IdentifierName("FromBounds");
                SyntaxNode expression = generator.MemberAccessExpression(memberIdentifier, memberName);

                SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();

                SyntaxNode startSpanIdentifier = generator.IdentifierName(startIdentifier);
                SyntaxNode endSpanIdentifier = generator.IdentifierName(endIdentifier);

                arguments = arguments.Add(startSpanIdentifier);
                arguments = arguments.Add(endSpanIdentifier);

                SyntaxNode initializer = generator.InvocationExpression(expression, arguments);
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

                return localDeclaration;
            }

            // creates a variable of the form var variableName = identifierString.SpanStart;
            protected internal static SyntaxNode CreateEndOrStartSpan(SyntaxGenerator generator, string identifierString, string variableName)
            {
                SyntaxNode identifier = generator.IdentifierName(identifierString);
                SyntaxNode initializer = generator.MemberAccessExpression(identifier, "SpanStart");
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(variableName, initializer);

                return localDeclaration;
            }

            // creates a variable of the form var openParen = expressionString.OpenParentToken
            protected internal static SyntaxNode CreateOpenParen(SyntaxGenerator generator, string expressionString)
            {
                string name = "openParen";
                SyntaxNode expression = generator.IdentifierName(expressionString);
                SyntaxNode initializer = generator.MemberAccessExpression(expression, "OpenParenToken");
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

                return localDeclaration;
            }

            // creates a variable that creates a diagnostic
            protected internal static SyntaxNode CreateDiagnostic(SyntaxGenerator generator, string locationName, string ruleName)
            {
                SyntaxNode identifier = generator.IdentifierName("Diagnostic");
                SyntaxNode expression = generator.MemberAccessExpression(identifier, "Create");

                SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>();

                SyntaxNode ruleExpression = generator.IdentifierName(ruleName);
                SyntaxNode ruleArg = generator.Argument(ruleExpression);

                SyntaxNode locationExpression = generator.IdentifierName(locationName);
                SyntaxNode locationArg = generator.Argument(locationExpression);

                arguments = arguments.Add(ruleArg);
                arguments = arguments.Add(locationArg);

                string name = "diagnostic";
                SyntaxNode initializer = generator.InvocationExpression(expression, arguments);
                SyntaxNode localDeclaration = generator.LocalDeclarationStatement(name, initializer);

                return localDeclaration;
            }

            // gets the name of the first rule, or null if none is found
            protected internal static string GetFirstRuleName(ClassDeclarationSyntax declaration)
            {
                SyntaxList<MemberDeclarationSyntax> members = declaration.Members;
                FieldDeclarationSyntax rule = null;

                foreach (MemberDeclarationSyntax member in members)
                {
                    rule = member as FieldDeclarationSyntax;
                    var ruleType = rule.Declaration.Type as IdentifierNameSyntax;
                    if (rule != null && ruleType != null && ruleType.Identifier.Text == "DiagnosticDescriptor")
                    {
                        break;
                    }

                    rule = null;
                }

                if (rule == null)
                {
                    return null;
                }

                return rule.Declaration.Variables[0].Identifier.Text;
            }

            // gets the analysis method
            protected internal static MethodDeclarationSyntax GetAnalysis(ClassDeclarationSyntax declaration)
            {
                SyntaxList<MemberDeclarationSyntax> members = declaration.Members;
                MethodDeclarationSyntax analysisMethod = null;

                foreach (MemberDeclarationSyntax member in members)
                {
                    analysisMethod = IsSyntaxNodeAnalysisMethod(member);
                    if (analysisMethod != null)
                    {
                        break;
                    }
                }

                return analysisMethod;
            }

            // check if the member is the SyntaxNodeAnalysis method, returns the MethodDeclarationSyntax if it is, null if not
            protected internal static MethodDeclarationSyntax IsSyntaxNodeAnalysisMethod(MemberDeclarationSyntax member)
            {
                MethodDeclarationSyntax analysisMethod = member as MethodDeclarationSyntax;
                if (analysisMethod == null)
                {
                    return null;
                }

                if (analysisMethod.Identifier.Text == "Initialize")
                {
                    return null;
                }

                ParameterListSyntax parameterList = analysisMethod.ParameterList;
                if (parameterList == null)
                {
                    return null;
                }

                SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;
                if (parameters == null || parameters.Count < 1)
                {
                    return null;
                }

                ParameterSyntax contextParameter = parameters[0];
                var parameterType = contextParameter.Type as IdentifierNameSyntax;
                if (parameterType != null && parameterType.Identifier.Text != "SyntaxNodeAnalysisContext")
                {
                    return null;
                }

                return analysisMethod;
            }

            // creates a statement that reports a diagnostic
            protected internal static SyntaxNode CreateDiagnosticReport(SyntaxGenerator generator, string argumentName, string contextName)
            {
                SyntaxNode argumentExpression = generator.IdentifierName(argumentName);
                SyntaxNode argument = generator.Argument(argumentExpression);

                SyntaxNode identifier = generator.IdentifierName(contextName);
                SyntaxNode memberExpression = generator.MemberAccessExpression(identifier, "ReportDiagnostic");
                SyntaxNode expression = generator.InvocationExpression(memberExpression, argument);
                SyntaxNode expressionStatement = generator.ExpressionStatement(expression);

                return expressionStatement;
            }

            // creates a variable holding a DiagnosticDescriptor
            // uses SyntaxFactory for formatting
            protected internal static FieldDeclarationSyntax CreateEmptyRule(SyntaxGenerator generator, string idName="", string titleDefault="Enter a title for this diagnostic", string messageDefault="Enter a message to be displayed with this diagnostic",
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
                string whitespace = "            ";
                SyntaxNode id = null;
                if (idName != "")
                {
                    id = generator.LiteralExpression(idName);
                }
                else
                {
                    id = generator.IdentifierName("").WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia("/* The ID here should be the public constant declared above */"));
                }

                var idArg = generator.Argument("id", RefKind.None, id).WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[0] = idArg;

                SyntaxNode title = generator.LiteralExpression(titleDefault);
                var titleArg = generator.Argument("title", RefKind.None, title).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[1] = titleArg;

                SyntaxNode message = generator.LiteralExpression(messageDefault);
                var messageArg = generator.Argument("messageFormat", RefKind.None, message).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[2] = messageArg;

                SyntaxNode category = generator.LiteralExpression(categoryDefault);
                var categoryArg = generator.Argument("category", RefKind.None, category).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[3] = categoryArg;

                var defaultSeverityArg = generator.Argument("defaultSeverity", RefKind.None, severityDefault).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[4] = defaultSeverityArg;

                var enabledArg = generator.Argument("isEnabledByDefault", RefKind.None, enabledDefault).WithLeadingTrivia(SyntaxFactory.Whitespace(whitespace)) as ArgumentSyntax;
                arguments[5] = enabledArg;
                
                SyntaxToken identifier = SyntaxFactory.ParseToken("spacingRule");

                var separators = new List<SyntaxToken>();
                SyntaxToken separator = SyntaxFactory.ParseToken(",").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);
                separators.Add(separator);
                separators.Add(separator);
                separators.Add(separator);
                separators.Add(separator);
                separators.Add(separator);

                SeparatedSyntaxList<ArgumentSyntax> argumentsNewLines = SyntaxFactory.SeparatedList(arguments, separators);
                ArgumentListSyntax argumentList = SyntaxFactory.ArgumentList(argumentsNewLines);
                ObjectCreationExpressionSyntax value = SyntaxFactory.ObjectCreationExpression(type, argumentList, null);
                EqualsValueClauseSyntax initializer = SyntaxFactory.EqualsValueClause(value);

                var variables = new SeparatedSyntaxList<VariableDeclaratorSyntax>();
                VariableDeclaratorSyntax variable = SyntaxFactory.VariableDeclarator(identifier, null, initializer);
                variables = variables.Add(variable);

                VariableDeclarationSyntax declaration = SyntaxFactory.VariableDeclaration(type.WithTrailingTrivia(SyntaxFactory.Whitespace(" ")), variables);
                SyntaxTokenList modifiers = SyntaxFactory.TokenList(SyntaxFactory.ParseToken("internal").WithTrailingTrivia(SyntaxFactory.Whitespace(" ")), SyntaxFactory.ParseToken("static").WithTrailingTrivia(SyntaxFactory.Whitespace(" ")));
                FieldDeclarationSyntax rule = SyntaxFactory.FieldDeclaration(new SyntaxList<AttributeListSyntax>(), modifiers, declaration);

                return rule;
            }

            // creates the SupportedDiagnostics property with a get accessor with a not implemented exception
            protected internal static PropertyDeclarationSyntax CreateSupportedDiagnostics(SyntaxGenerator generator, INamedTypeSymbol notImplementedException)
            {
                TypeSyntax type = SyntaxFactory.ParseTypeName("ImmutableArray<DiagnosticDescriptor>");
                DeclarationModifiers modifiers = DeclarationModifiers.Override;

                SyntaxList<SyntaxNode> getAccessorStatements = new SyntaxList<SyntaxNode>();

                SyntaxNode throwStatement = generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException));
                getAccessorStatements = getAccessorStatements.Add(throwStatement);

                PropertyDeclarationSyntax propertyDeclaration = generator.PropertyDeclaration("SupportedDiagnostics", type, accessibility: Accessibility.Public, modifiers: modifiers, getAccessorStatements: getAccessorStatements) as PropertyDeclarationSyntax;
                propertyDeclaration = propertyDeclaration.RemoveNode(propertyDeclaration.AccessorList.Accessors[1], 0);

                return propertyDeclaration;
            }

            // creates a SyntaxKind.IfStatement argument
            protected internal static ArgumentSyntax CreateSyntaxKindIfStatement(SyntaxGenerator generator)
            {
                SyntaxNode syntaxKind = generator.IdentifierName("SyntaxKind");
                SyntaxNode expression = generator.MemberAccessExpression(syntaxKind, "IfStatement");
                var argument = generator.Argument(expression) as ArgumentSyntax;

                return argument;
            }

            // creates a correct register statement
            protected internal static SyntaxNode CreateRegister(SyntaxGenerator generator, MethodDeclarationSyntax declaration, string methodName)
            {
                var argument1 = generator.Argument(generator.IdentifierName(methodName)) as ArgumentSyntax;
                var argument2 = generator.Argument(generator.MemberAccessExpression(generator.IdentifierName("SyntaxKind"), "IfStatement")) as ArgumentSyntax;
                SeparatedSyntaxList<ArgumentSyntax> arguments = new SeparatedSyntaxList<ArgumentSyntax>();
                arguments = arguments.Add(argument1);
                arguments = arguments.Add(argument2);
                ArgumentListSyntax argumentList = SyntaxFactory.ArgumentList(arguments);

                string parameterName = GetFirstParameterName(declaration);
                SyntaxNode invocationExpr = BuildRegister(generator, parameterName, "RegisterSyntaxNodeAction", argumentList);
                return invocationExpr;
            }

            // creates the SyntaxNode analysis method
            protected internal static SyntaxNode CreateAnalysisMethod(SyntaxGenerator generator, string methodName, SemanticModel semanticModel)
            {
                TypeSyntax type = SyntaxFactory.ParseTypeName("SyntaxNodeAnalysisContext");
                SyntaxNode[] parameters = new[] { generator.ParameterDeclaration("context", type) };
                SyntaxList<SyntaxNode> statements = new SyntaxList<SyntaxNode>();
                INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
                statements = statements.Add(generator.ThrowStatement(generator.ObjectCreationExpression(notImplementedException)));

                SyntaxNode newMethodDeclaration = generator.MethodDeclaration(methodName, parameters: parameters, accessibility: Accessibility.Private, statements: statements);
                return newMethodDeclaration.WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia("// This is the method that is registered within Initialize and is called when an IfStatement SyntaxNode is found").ElementAt(0), SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// First, this method analyzes the Syntax Tree. Then, it reports a diagnostic if an error is found").ElementAt(0), SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.ParseLeadingTrivia("// In this tutorial, this method will walk through the Syntax Tree seen in IfSyntaxTree.jpg and determine if the if-statement being analyzed has the correct spacing").ElementAt(0), SyntaxFactory.CarriageReturnLineFeed);
            }

            // gets the name of an existing analysis method, or null if none is found
            protected internal static string GetExistingAnalysisMethodName(ClassDeclarationSyntax classDeclaration)
            {
                IEnumerable<MethodDeclarationSyntax> methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();
                string methodName = null;

                foreach (MethodDeclarationSyntax method in methods)
                {
                    ParameterListSyntax parameterList = method.ParameterList;
                    if (parameterList != null)
                    {
                        SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;
                        if (parameters != null)
                        {
                            if (parameters.Count > 0)
                            {
                                var parameterType = parameters.First().Type as IdentifierNameSyntax;
                                if (parameterType != null && parameterType.Identifier.Text == "SyntaxNodeAnalysisContext")
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
            protected internal static SyntaxNode CreateMethodWithContextParameter(SyntaxGenerator generator, MethodDeclarationSyntax methodDeclaration)
            {
                TypeSyntax type = SyntaxFactory.ParseTypeName("SyntaxNodeAnalysisContext");
                SyntaxNode[] parameters = new[] { generator.ParameterDeclaration("context", type) };
                string methodName = methodDeclaration.Identifier.Text;
                TypeSyntax returnType = methodDeclaration.ReturnType;
                SyntaxList<StatementSyntax> statements = methodDeclaration.Body.Statements;

                SyntaxNode newDeclaration = generator.MethodDeclaration(methodName, parameters, returnType: returnType, accessibility: Accessibility.Private, statements: statements);
                return newDeclaration;
            }

            protected internal static List<string> GetAllRuleNames(ClassDeclarationSyntax declaration)
            {
                List<string> ruleNames = new List<string>();
                var fieldMembers = declaration.Members.OfType<FieldDeclarationSyntax>();
                foreach (FieldDeclarationSyntax fieldSyntax in fieldMembers)
                {
                    var fieldType = fieldSyntax.Declaration.Type as IdentifierNameSyntax;
                    if (fieldType != null && fieldType.Identifier.Text == "DiagnosticDescriptor")
                    {
                        string ruleName = fieldSyntax.Declaration.Variables[0].Identifier.Text;
                        ruleNames.Add(ruleName);
                    }
                }
                return ruleNames;
            }

            protected internal static SyntaxList<SyntaxNode> CreateRuleList(Document document, List<string> ruleNames)
            {
              
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

                ArgumentListSyntax argumentListSyntax = SyntaxFactory.ParseArgumentList("(" + argumentListString + ")");
                SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

                SeparatedSyntaxList<ArgumentSyntax> args = argumentListSyntax.Arguments;
                var nodeArgs = new SyntaxList<SyntaxNode>();
                foreach (ArgumentSyntax arg in args)
                {
                    nodeArgs = nodeArgs.Add(arg as SyntaxNode);
                }

                return nodeArgs;
            }
        }
    }
}