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
                                             MetaCompilationAnalyzer.IncorrectArguments);
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

                if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingId))
                {
                    IEnumerable<ClassDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ClassDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Each diagnostic should have a unique string id identifying it from other diagnostics", c => MissingIdAsync(context.Document, declaration, c)), diagnostic);

                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingInit))
                {
                    IEnumerable<ClassDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ClassDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The Initialize method prompts actions to analyze the code when changes occur", c => MissingInitAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingRegisterStatement))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Registered actions are necessary to analyze code when changes occur", c => MissingRegisterAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TooManyInitStatements))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Only one registered action is necessary in this tutorial", c => MultipleStatementsAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.InvalidStatement))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The Initialize method can only register actions, anything else is invalid", c => InvalidStatementAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.InternalAndStaticError))
                {
                    IEnumerable<FieldDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        FieldDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Rules must be declared as both internal and static.", c => InternalStaticAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.EnabledByDefaultError))
                {
                    IEnumerable<ArgumentSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ArgumentSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ArgumentSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Rules should be enabled by default.", c => EnabledByDefaultAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.DefaultSeverityError))
                {
                    IEnumerable<ArgumentSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ArgumentSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ArgumentSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "defaultSeverity should be set to \"Error\" if something is not allowed by the language authorities.", c => DiagnosticSeverityError(context.Document, declaration, c)), diagnostic);
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "defaultSeverity should be set to \"Warning\" if something is suspicious but allowed.", c => DiagnosticSeverityWarning(context.Document, declaration, c)), diagnostic);
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "defaultSeverity should be set to \"Hidden\" if something is an issue, but is not surfaced by normal means.", c => DiagnosticSeverityHidden(context.Document, declaration, c)), diagnostic);
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "defaultSeverity should be set to \"Info\" for information that does not indicate a problem.", c => DiagnosticSeverityInfo(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.MissingIdDeclaration))
                {
                    IEnumerable<VariableDeclaratorSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>();
                    if (declarations.Count() != 0)
                    {
                        VariableDeclaratorSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Generate a public field for this rule id.", c => MissingIdDeclarationAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.EndsWith(MetaCompilationAnalyzer.IdDeclTypeError))
                {
                    IEnumerable<ClassDeclarationSyntax> classDeclarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                    if (classDeclarations.Count() != 0)
                    {
                        ClassDeclarationSyntax classDeclaration = classDeclarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Add a rule id local variable of type string.", c => IdDeclTypeAsync(context.Document, classDeclaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfStatementIncorrect))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The first statement of the analyzer must access the node to be analyzed", c => IncorrectIfAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfStatementMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The first statement of the analyzer must access the node to be analyzed", c => MissingIfAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IncorrectInitSig))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The Initialize method must have the correct signature to be implemented", c => IncorrectSigAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfKeywordIncorrect))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The second statement of the analyzer must access the keyword from the node being analyzed", c => IncorrectKeywordAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IfKeywordMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The second statement of the analyzer must access the keyword from the node being analyzed", c => MissingKeywordAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The third statement of the analyzer must be an if statement checking the trailing trivia of the node being analyzed", c => TrailingCheckIncorrectAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaCheckMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The third statement of the analyzer must be an if statement checking the trailing trivia of the node being analyzed", c => TrailingCheckMissingAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaVarMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The fourth statement of the analyzer should store the last trailing trivia of the if keyword", c => TrailingVarMissingAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaVarIncorrect))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The fourth statement of the analyzer should store the last trailing trivia of the if keyword", c => TrailingVarIncorrectAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect))
                {
                    IEnumerable<IfStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        IfStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + " The fifth statement of the analyzer should be a check of the kind of trivia following the if keyword", c => TrailingKindCheckIncorrectAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TrailingTriviaKindCheckMissing))
                {
                    IEnumerable<IfStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        IfStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The fifth statement of the analyzer should be a check of the kind of trivia following the if keywor", c => TrailingKindCheckMissingAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.WhitespaceCheckIncorrect))
                {
                    IEnumerable<IfStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        IfStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The sixth statement of the analyzer should be a check to ensure the whitespace after if statement keyword is correct", c => WhitespaceCheckIncorrectAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.ReturnStatementIncorrect))
                {
                    IEnumerable<IfStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        IfStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The seventh step of the analyzer should quit the analysis (if the if statement is formatted properly)", c => ReturnIncorrectAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.ReturnStatementMissing))
                {
                    IEnumerable<IfStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        IfStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "There must be a return statement indicating that the spacing for the if statement is correct", c => ReturnMissingAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.WhitespaceCheckMissing))
                {
                    IEnumerable<IfStatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        IfStatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The sixth statement of the analyzer should be a check to ensure the whitespace after the if statement keyword is correct", c => WhitespaceCheckMissingAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.LocationMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a diagnostic location. This is where the red squiggle will appear in the code that you are analyzing", c => AddLocationAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.LocationIncorrect))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a diagnostic location. This is where the red squiggle will appear in the code that you are analyzing", c => ReplaceLocationAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.SpanMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a diagnostic span. This is where the red squiggle will appear in the code that you are analyzing", c => AddSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.SpanIncorrect))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create a diagnostic span. This is where the red squiggle will appear in the code that you are analyzing", c => ReplaceSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.EndSpanMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create an int that is the end of the diagnostic span", c => AddEndSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.EndSpanIncorrect))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create an int that is the end of the diagnostic span", c => ReplaceEndSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.StartSpanMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create an int that is the start of the diagnostic span", c => AddStartSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.StartSpanIncorrect))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create an int that is the start of the diagnostic span", c => ReplaceStartSpanAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.OpenParenMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the open parenthesis from the if statement", c => AddOpenParenAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.OpenParenIncorrect))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Extract the open parenthesis from the if statement", c => ReplaceOpenParenAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticMissing))
                {
                    IEnumerable<ClassDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ClassDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create the diagnostic that is going to be reported", c => AddDiagnosticAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticIncorrect))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Create the diagnostic that is going to be reported", c => ReplaceDiagnosticAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticReportMissing))
                {
                    IEnumerable<MethodDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        MethodDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Report the diagnostic to the context of the if statement in question", c => AddDiagnosticReportAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.DiagnosticReportIncorrect))
                {
                    IEnumerable<StatementSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<StatementSyntax>();
                    if (declarations.Count() != 0)
                    {
                        StatementSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Report the diagnostic to the context of the if statement in question", c => ReplaceDiagnosticReportAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IncorrectSigSuppDiag))
                {
                    IEnumerable<PropertyDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        PropertyDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The property's signature must match its inherited member's signature", c => IncorrectSigSuppDiagAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingAccessor))
                {
                    IEnumerable<PropertyDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        PropertyDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The get-accessor should return all diagnostics from this analyzer", c => MissingAccessorAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.TooManyAccessors))
                {
                    IEnumerable<PropertyDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        PropertyDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Remove unnecessary accessors from the SupportedDiagnostics property", c => TooManyAccessorsAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IncorrectAccessorReturn) || diagnostic.Id.Equals(MetaCompilationAnalyzer.SuppDiagReturnValue))
                {
                    IEnumerable<PropertyDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        PropertyDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Return an ImmutableArray from the SupportedDiagnostics property get accessor", c => AccessorReturnValueAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.SupportedRules))
                {
                    IEnumerable<ClassDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ClassDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Return all rules from SupportedDiagnostics", c => SupportedRulesAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingSuppDiag))
                {
                    IEnumerable<ClassDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ClassDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The SupportedDiagnostics property describes the diagnostics from an analyzer", c => AddSuppDiagAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.MissingRule))
                {
                    IEnumerable<ClassDeclarationSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ClassDeclarationSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "Insert the beginning of a DiagnosticDescriptor", c => AddRuleAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IncorrectKind))
                {
                    IEnumerable<ArgumentListSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ArgumentListSyntax>();
                    if (declarations.Count() != 0)
                    {
                        ArgumentListSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "This tutorial analyzes nodes of syntax kind 'IfStatement'", c => CorrectKindAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IncorrectRegister))
                {
                    IEnumerable<IdentifierNameSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IdentifierNameSyntax>();
                    if (declarations.Count() != 0)
                    {
                        IdentifierNameSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "This tutorial only registers actions for SyntaxNode analysis", c => CorrectRegisterAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
                else if (diagnostic.Id.Equals(MetaCompilationAnalyzer.IncorrectArguments))
                {
                    IEnumerable<InvocationExpressionSyntax> declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>();
                    if (declarations.Count() != 0)
                    {
                        InvocationExpressionSyntax declaration = declarations.First();
                        context.RegisterCodeFix(CodeAction.Create(MessagePrefix + "The arguments needed are an analysis method and a syntax kind", c => CorrectArgumentsAsync(context.Document, declaration, c)), diagnostic);
                    }
                }
            }
        }

        private async Task<Document> CorrectArgumentsAsync(Document document, InvocationExpressionSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var argList = declaration.ArgumentList;
            if (argList != null)
            {
                var args = argList.Arguments;
                if (args != null)
                {
                    if (args.Count > 0)
                    {
                        var nameArg = args[0];
                        var expression = nameArg.Expression as IdentifierNameSyntax;
                        if (expression != null)
                        {
                            SyntaxNode statementKeepArg = CodeFixNodeCreator.CreateRegister(generator, declaration.Parent.Parent.Parent as MethodDeclarationSyntax, expression.Identifier.Text);

                            return await ReplaceNode(declaration, statementKeepArg, document);
                        }
                    }
                }
            }

            SyntaxNode statement = CodeFixNodeCreator.CreateRegister(generator, declaration.Parent.Parent.Parent as MethodDeclarationSyntax);

            return await ReplaceNode(declaration, statement, document);
        }

        private async Task<Document> CorrectRegisterAsync(Document document, IdentifierNameSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
            SyntaxNode newRegister = generator.IdentifierName("RegisterSyntaxNodeAction");

            return await ReplaceNode(declaration, newRegister, document);
        }

        private async Task<Document> CorrectKindAsync(Document document, ArgumentListSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            ArgumentSyntax argument = CodeFixNodeCreator.CreateSyntaxKindIfStatement(generator);
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

            return await ReplaceNode(declaration, argList, document);
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

            FieldDeclarationSyntax fieldDeclaration = CodeFixNodeCreator.CreateEmptyRule(generator);

            var newNode = new SyntaxList<SyntaxNode>();
            newNode = newNode.Add(fieldDeclaration);

            var root = await document.GetSyntaxRootAsync();
            if (insertPointNode != null)
            {
                var newRoot = root.InsertNodesBefore(insertPointNode, newNode);
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
            else
            {
                var newRoot = root.ReplaceNode(declaration, declaration.AddMembers(fieldDeclaration));
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
            PropertyDeclarationSyntax propertyDeclaration = CodeFixNodeCreator.CreateSupportedDiagnostics(generator, notImplementedException);

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

        private async Task<Document> ReplaceDiagnosticAsync(Document document, StatementSyntax declaration, CancellationToken c)
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
        private async Task<Document> ReplaceOpenParenAsync(Document document, StatementSyntax declaration, CancellationToken c)
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
        private async Task<Document> ReplaceStartSpanAsync(Document document, StatementSyntax declaration, CancellationToken c)
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
        private async Task<Document> ReplaceEndSpanAsync(Document document, StatementSyntax declaration, CancellationToken c)
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
        private async Task<Document> ReplaceSpanAsync(Document document, StatementSyntax declaration, CancellationToken c)
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
        private async Task<Document> ReplaceLocationAsync(Document document, StatementSyntax declaration, CancellationToken c)
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
            var expressionKind = SyntaxFactory.ParseExpression("\"IfSpacing\"") as ExpressionSyntax;

            var editor = await DocumentEditor.CreateAsync(document, c).ConfigureAwait(false);

            var newField = CodeFixNodeCreator.NewIdCreator(editor.Generator, "spacingRuleId", expressionKind);

            editor.InsertMembers(declaration, 0, new[] { newField });

            return editor.GetChangedDocument();
        }
        #endregion

        #region initialize code fix
        private async Task<Document> MissingInitAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SemanticModel semanticModel = await document.GetSemanticModelAsync();

            INamedTypeSymbol notImplementedException = semanticModel.Compilation.GetTypeByMetadataName("System.NotImplementedException");
            SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();
            string name = "context";
            var initializeDeclaration = CodeFixNodeCreator.BuildInitialize(generator, notImplementedException, statements, name);
            var newClassDeclaration = generator.AddMembers(declaration, initializeDeclaration);

            return await ReplaceNode(declaration, newClassDeclaration, document);
        }

        private async Task<Document> MissingRegisterAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode invocationExpression = CodeFixNodeCreator.CreateRegister(generator, declaration);
            SyntaxList<SyntaxNode> statements = new SyntaxList<SyntaxNode>().Add(invocationExpression);
            
            SyntaxNode newMethod = generator.MethodDeclaration("Initialize", declaration.ParameterList.Parameters, accessibility: Accessibility.Public, modifiers: DeclarationModifiers.Override, statements: statements);

            return await ReplaceNode(declaration, newMethod, document);
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
            initializeDeclaration = CodeFixNodeCreator.BuildInitialize(generator, null, statements, name);

            return await ReplaceNode(declaration, initializeDeclaration, document);
        }

        private async Task<Document> IncorrectIfAsync(Document document, StatementSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            MethodDeclarationSyntax methodDeclaration = declaration.Parent.Parent as MethodDeclarationSyntax;
            string name = methodDeclaration.ParameterList.Parameters[0].Identifier.ValueText as string;
            var ifStatement = CodeFixNodeCreator.IfHelper(generator, name);

            return await ReplaceNode(declaration, ifStatement, document);
        }

        private async Task<Document> MissingIfAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            string name = declaration.ParameterList.Parameters[0].Identifier.ValueText as string;
            StatementSyntax ifStatement = CodeFixNodeCreator.IfHelper(generator, name) as StatementSyntax;

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
            var newFieldDeclaration = declaration.WithModifiers(modifierList).WithLeadingTrivia(declaration.GetLeadingTrivia()).WithTrailingTrivia(declaration.GetTrailingTrivia());

            return await ReplaceNode(declaration, newFieldDeclaration, document);
        }

        private async Task<Document> EnabledByDefaultAsync(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            var literalExpression = argument.Expression;
            var newLiteralExpression = (SyntaxFactory.ParseExpression("true").WithTrailingTrivia(literalExpression.GetTrailingTrivia())) as LiteralExpressionSyntax;

            return await ReplaceNode(literalExpression, newLiteralExpression, document);
        }

        private async Task<Document> DiagnosticSeverityWarning(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            SyntaxNode newExpression = generator.MemberAccessExpression(expression, "Warning");

            var argExpr = argument.Expression;
            return await ReplaceNode(argExpr, newExpression, document);
        }

        private async Task<Document> DiagnosticSeverityHidden(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            SyntaxNode newExpression = generator.MemberAccessExpression(expression, "Hidden");

            var argExpr = argument.Expression;
            return await ReplaceNode(argExpr, newExpression, document);
        }

        private async Task<Document> DiagnosticSeverityInfo(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            SyntaxNode newExpression = generator.MemberAccessExpression(expression, "Info");

            var argExpr = argument.Expression;
            return await ReplaceNode(argExpr, newExpression, document);
        }

        private async Task<Document> DiagnosticSeverityError(Document document, ArgumentSyntax argument, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            SyntaxNode expression = generator.IdentifierName("DiagnosticSeverity");
            SyntaxNode newExpression = generator.MemberAccessExpression(expression, "Error");

            var argExpr = argument.Expression;
            return await ReplaceNode(argExpr, newExpression, document);
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

            var expressionKind = SyntaxFactory.ParseExpression("\"DescriptiveId\"") as ExpressionSyntax;

            var editor = await DocumentEditor.CreateAsync(document, c).ConfigureAwait(false);

            var newField = CodeFixNodeCreator.NewIdCreator(editor.Generator, currentRuleId, expressionKind);

            editor.InsertMembers(classDeclaration, 0, new[] { newField });

            return editor.GetChangedDocument();
        }

        private async Task<Document> IdDeclTypeAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var members = classDeclaration.Members;
            ExpressionSyntax oldIdName = null;
            IdentifierNameSyntax newIdName = null;

            foreach (MemberDeclarationSyntax memberSyntax in members)
            {
                var fieldDeclaration = memberSyntax as FieldDeclarationSyntax;
                if (fieldDeclaration == null)
                {
                    continue;
                }

                if (fieldDeclaration.Declaration.Type != null && fieldDeclaration.Declaration.Type.ToString() == "DiagnosticDescriptor")
                {
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

            return await ReplaceNode(oldIdName, newIdName, document);
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
            var returnStatement = generator.ReturnStatement(invocationExpression) as ReturnStatementSyntax;

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
                if (ifBlock != null)
                {
                    ifBlockStatements = ifBlock.Statements;
                }
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
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifBlockStatements = new SyntaxList<StatementSyntax>();
            StatementSyntax ifStatement = CodeFixNodeCreator.TriviaCheckHelper(generator, declaration.Body, ifBlockStatements) as StatementSyntax;

            var oldBlock = declaration.Body;
            var newBlock = declaration.Body.WithStatements(declaration.Body.Statements.Add(ifStatement));

            return await ReplaceNode(oldBlock, newBlock, document);
        }
        
        private async Task<Document> TrailingVarMissingAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifStatement = declaration.Body.Statements[2] as IfStatementSyntax;
            var localDeclaration = new SyntaxList<SyntaxNode>().Add(CodeFixNodeCreator.TriviaVarMissingHelper(generator, ifStatement));

            var oldBlock = ifStatement.Statement as BlockSyntax;
            var newBlock = oldBlock.WithStatements(localDeclaration);

            return await ReplaceNode(oldBlock, newBlock, document);
        }
        
        private async Task<Document> TrailingVarIncorrectAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken c)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);

            var ifStatement = declaration.Body.Statements[2] as IfStatementSyntax;
            var localDeclaration = CodeFixNodeCreator.TriviaVarMissingHelper(generator, ifStatement) as LocalDeclarationStatementSyntax;

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
            if (declaration.Parent.Parent.Kind() == SyntaxKind.MethodDeclaration)
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
            var newIfStatement = CodeFixNodeCreator.TriviaKindCheckHelper(generator, declaration, ifBlockStatements) as StatementSyntax;

            var oldBlock = declaration.Statement as BlockSyntax;
            var newBlock = oldBlock.AddStatements(newIfStatement);

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
                if (ifBlock != null)
                {
                    ifBlockStatements = ifBlock.Statements;
                }
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
            var generator = SyntaxGenerator.GetGenerator(document);

            IfStatementSyntax ifStatement;
            if (declaration.Parent.Parent.Parent.Parent.Parent.Parent.Kind() != SyntaxKind.MethodDeclaration)
            {
                ifStatement = declaration.Parent.Parent as IfStatementSyntax;
            }
            else
            {
                ifStatement = declaration;
            }

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

        class CodeFixNodeCreator
        {
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
                SyntaxNode newField = generator.FieldDeclaration(
                    fieldName,
                    generator.TypeExpression(SpecialType.System_String),
                    Accessibility.Public,
                    DeclarationModifiers.Const,
                    initializer);

                return newField;
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
                SyntaxNode identifier = generator.IdentifierName(identifierString);

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

            internal static FieldDeclarationSyntax CreateEmptyRule(SyntaxGenerator generator)
            {
                var type = SyntaxFactory.ParseTypeName("DiagnosticDescriptor");

                var arguments = new SyntaxNode[6];

                var id = generator.IdentifierName("");
                var idArg = generator.Argument("id", RefKind.None, id);
                arguments[0] = idArg;

                var title = generator.LiteralExpression("Enter a title for this diagnostic");
                var titleArg = generator.Argument("title", RefKind.None, title);
                arguments[1] = titleArg;

                var message = generator.LiteralExpression("Enter a message to be displayed with this diagnostic");
                var messageArg = generator.Argument("messageFormat", RefKind.None, message);
                arguments[2] = messageArg;

                var category = generator.LiteralExpression("Enter a category for this diagnostic");
                var categoryArg = generator.Argument("category", RefKind.None, category);
                arguments[3] = categoryArg;

                var defaultSeverity = generator.IdentifierName("");
                var defaultSeverityArg = generator.Argument("defaultSeverity", RefKind.None, defaultSeverity);
                arguments[4] = defaultSeverityArg;

                var enabled = generator.IdentifierName("");
                var enabledArg = generator.Argument("isEnabledByDefault", RefKind.None, enabled);
                arguments[5] = enabledArg;

                var initializer = generator.ObjectCreationExpression(type, arguments);

                var rule = generator.FieldDeclaration("spacingRule", type, accessibility: Accessibility.Internal, modifiers: DeclarationModifiers.Static, initializer: initializer) as FieldDeclarationSyntax;
                return rule;
            }

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

            internal static ArgumentSyntax CreateSyntaxKindIfStatement(SyntaxGenerator generator)
            {
                var syntaxKind = generator.IdentifierName("SyntaxKind");
                var expression = generator.MemberAccessExpression(syntaxKind, "IfStatement");
                var argument = generator.Argument(expression) as ArgumentSyntax;
                return argument;
            }

            internal static SyntaxNode CreateRegister(SyntaxGenerator generator, MethodDeclarationSyntax declaration, string methodName = "AnalyzeIfStatement")
            {
                SyntaxNode argument1 = generator.IdentifierName(methodName);
                SyntaxNode argument2 = generator.MemberAccessExpression(generator.IdentifierName("SyntaxKind"), "IfStatement");
                SyntaxList<SyntaxNode> arguments = new SyntaxList<SyntaxNode>().Add(argument1).Add(argument2);

                SyntaxNode expression = generator.IdentifierName(declaration.ParameterList.Parameters[0].Identifier.ValueText);
                SyntaxNode memberAccessExpression = generator.MemberAccessExpression(expression, "RegisterSyntaxNodeAction");
                SyntaxNode invocationExpression = generator.InvocationExpression(memberAccessExpression, arguments);

                return invocationExpression;
            }
        }
    }
}