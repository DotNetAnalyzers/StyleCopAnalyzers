// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

namespace MetaCompilation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MetaCompilationAnalyzer : DiagnosticAnalyzer
    {
        public const string RuleCategory = "Tutorial";
        public const DiagnosticSeverity RuleDefaultSeverity = DiagnosticSeverity.Error;
        public const bool RuleEnabledByDefault = true;

        public static DiagnosticDescriptor CreateRule(string id, string title, string messageFormat)
        {
            DiagnosticDescriptor rule = new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                defaultSeverity: RuleDefaultSeverity,
                isEnabledByDefault: RuleEnabledByDefault,
                category: RuleCategory
                );

            return rule;
        }

        #region rule rules

        public const string IdDeclTypeError = "MetaAnalyzer018";
        internal static DiagnosticDescriptor IdDeclTypeErrorRule = CreateRule(IdDeclTypeError, "This diagnostic id type is incorrect", "The diagnostic id should not be a string literal");

        public const string MissingIdDeclaration = "MetaAnalyzer017";
        internal static DiagnosticDescriptor MissingIdDeclarationRule = CreateRule(MissingIdDeclaration, "The diagnostic id declaration is missing", "This diagnostic id has not been declared");

        public const string DefaultSeverityError = "MetaAnalyzer016";
        internal static DiagnosticDescriptor DefaultSeverityErrorRule = CreateRule(DefaultSeverityError, "defaultSeverity is incorrectly declared", "defaultSeverity must be of the form: DiagnosticSeverity.[severity]");

        public const string EnabledByDefaultError = "MetaAnalyzer015";
        internal static DiagnosticDescriptor EnabledByDefaultErrorRule = CreateRule(EnabledByDefaultError, "isEnabledByDefault should be set to true", "isEnabledByDefault should be set to true");

        public const string InternalAndStaticError = "MetaAnalyzer014";
        internal static DiagnosticDescriptor InternalAndStaticErrorRule = CreateRule(InternalAndStaticError, "The DiagnosticDescriptor should be internal and static", "The DiagnosticDescriptor should be internal and static");

        public const string MissingRule = "MetaAnalyzer019";
        internal static DiagnosticDescriptor MissingRuleRule = CreateRule(MissingRule, "Missing a rule", "You need to have at least one DiagnosticDescriptor rule");
        #endregion

        #region id rules
        public const string MissingId = "MetaAnalyzer001";
        internal static DiagnosticDescriptor MissingIdRule = CreateRule(MissingId, "The analyzer is missing a diagnostic id", "The analyzer '{0}' is missing a diagnostic id");
        #endregion

        #region Initialize rules
        public const string MissingInit = "MetaAnalyzer002";
        internal static DiagnosticDescriptor MissingInitRule = CreateRule(MissingInit, "The analyzer is missing the required Initialize method", "The analyzer '{0}' is missing the required Initialize method");

        public const string MissingRegisterStatement = "MetaAnalyzer003";
        internal static DiagnosticDescriptor MissingRegisterRule = CreateRule(MissingRegisterStatement, "An action must be registered within the method", "An action must be registered within the '{0}' method");

        public const string TooManyInitStatements = "MetaAnalyzer004";
        internal static DiagnosticDescriptor TooManyInitStatementsRule = CreateRule(TooManyInitStatements, "The method registers multiple actions", "The '{0}' method registers multiple actions");

        public const string IncorrectInitStatement = "MetaAnalyzer005";
        internal static DiagnosticDescriptor IncorrectInitStatementRule = CreateRule(IncorrectInitStatement, "This statement needs to register for a supported action", "This statement needs to register for a supported action");

        public const string IncorrectInitSig = "MetaAnalyzer006";
        internal static DiagnosticDescriptor IncorrectInitSigRule = CreateRule(IncorrectInitSig, "The signature for the method is incorrect", "The signature for the '{0}' method is incorrect");

        public const string InvalidStatement = "MetaAnalyzer020";
        internal static DiagnosticDescriptor InvalidStatementRule = CreateRule(InvalidStatement, "The Initialize method only registers actions: the statement is invalid", "The Initialize method only registers actions: the statement '{0}' is invalid");
        #endregion

        #region SupportedDiagnostics rules
        public const string MissingSuppDiag = "MetaAnalyzer007";
        internal static DiagnosticDescriptor MissingSuppDiagRule = CreateRule(MissingSuppDiag, "You are missing the required SupportedDiagnostics method", "You are missing the required SupportedDiagnostics method");

        public const string IncorrectSigSuppDiag = "MetaAnalyzer008";
        internal static DiagnosticDescriptor IncorrectSigSuppDiagRule = CreateRule(IncorrectSigSuppDiag, "The signature of the SupportedDiagnostics property is incorrect", "The signature of the SupportedDiagnostics property is incorrect");

        public const string MissingAccessor = "MetaAnalyzer009";
        internal static DiagnosticDescriptor MissingAccessorRule = CreateRule(MissingAccessor, "You are missing a get accessor in your SupportedDiagnostics property", "You are missing a get accessor in your SupportedDiagnostics property");

        public const string TooManyAccessors = "MetaAnalyzer010";
        internal static DiagnosticDescriptor TooManyAccessorsRule = CreateRule(TooManyAccessors, "You only need a get accessor for this property", "You only need a get accessor for this property");

        public const string IncorrectAccessorReturn = "MetaAnalyzer011";
        internal static DiagnosticDescriptor IncorrectAccessorReturnRule = CreateRule(IncorrectAccessorReturn, "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules", "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules");

        public const string SuppDiagReturnValue = "MetaAnalyzer012";
        internal static DiagnosticDescriptor SuppDiagReturnValueRule = CreateRule(SuppDiagReturnValue, "You need to create an immutable array", "You need to create an immutable array");

        public const string SupportedRules = "MetaAnalyzer013";
        internal static DiagnosticDescriptor SupportedRulesRule = CreateRule(SupportedRules, "The immutable array should contain every DiagnosticDescriptor rule that was created", "The immutable array should contain every DiagnosticDescriptor rule that was created");
        #endregion

        #region analysis rules
        public const string MissingAnalysisMethod = "MetaAnalyzer018";
        internal static DiagnosticDescriptor MissingAnalysisMethodRule = CreateRule(MissingAnalysisMethod, "Missing analysis method", "You are missing the method that was registered to perform the analysis");

        public const string TooManyStatements = "MetaAnalyzer036";
        internal static DiagnosticDescriptor TooManyStatementsRule = CreateRule(TooManyStatements, "Too many statements", "This {0} should only have {1} statement(s)");
        #endregion

        #region analysis for IfStatement rules
        public const string IfStatementMissing = "MetaAnalyzer024";
        internal static DiagnosticDescriptor IfStatementMissingRule = CreateRule(IfStatementMissing, "Missing 1st step", "The first step of the node analysis is to extract the if statement from {0}");

        public const string IfStatementIncorrect = "MetaAnalyzer022";
        internal static DiagnosticDescriptor IfStatementIncorrectRule = CreateRule(IfStatementIncorrect, "If statement extraction incorrect", "This statement should extract the if statement in question by casting {0}.Node to IfStatementSyntax");

        public const string IfKeywordMissing = "MetaAnalyzer021";
        internal static DiagnosticDescriptor IfKeywordMissingRule = CreateRule(IfKeywordMissing, "Missing 2nd step", "The second step is to extract the 'if' keyword from {0}");

        public const string IfKeywordIncorrect = "MetaAnalyzer025";
        internal static DiagnosticDescriptor IfKeywordIncorrectRule = CreateRule(IfKeywordIncorrect, "Incorrect 2nd step", "This statement should extract the 'if' keyword from {0}");

        public const string TrailingTriviaCheckMissing = "MetaAnalyzer026";
        internal static DiagnosticDescriptor TrailingTriviaCheckMissingRule = CreateRule(TrailingTriviaCheckMissing, "Missing 3rd step", "The third step is to begin looking for the space between 'if' and '(' by checking if {0} has trailing trivia");

        public const string TrailingTriviaCheckIncorrect = "MetaAnalyzer027";
        internal static DiagnosticDescriptor TrailingTriviaCheckIncorrectRule = CreateRule(TrailingTriviaCheckIncorrect, "Incorrect 3rd step", "This statement should be an if statement that checks to see if {0} has trailing trivia");

        public const string TrailingTriviaVarMissing = "MetaAnalyzer028";
        internal static DiagnosticDescriptor TrailingTriviaVarMissingRule = CreateRule(TrailingTriviaVarMissing, "Missing 4th step", "The fourth step is to extract the last trailing trivia of {0} into a variable");

        public const string TrailingTriviaVarIncorrect = "MetaAnalyzer029";
        internal static DiagnosticDescriptor TrailingTriviaVarIncorrectRule = CreateRule(TrailingTriviaVarIncorrect, "Incorrect 4th step", "This statement should extract the last trailing trivia of {0} into a variable");

        public const string TrailingTriviaKindCheckMissing = "MetaAnalyzer030";
        internal static DiagnosticDescriptor TrailingTriviaKindCheckMissingRule = CreateRule(TrailingTriviaKindCheckMissing, "Missing 5th step", "The fifth step is to check the kind of {0}");

        public const string TrailingTriviaKindCheckIncorrect = "MetaAnalyzer031";
        internal static DiagnosticDescriptor TrailingTriviaKindCheckIncorrectRule = CreateRule(TrailingTriviaKindCheckIncorrect, "Incorrect 5th step", "This statement should check to see if the kind of {0} is whitespace trivia");

        public const string WhitespaceCheckMissing = "MetaAnalyzer032";
        internal static DiagnosticDescriptor WhitespaceCheckMissingRule = CreateRule(WhitespaceCheckMissing, "Missing 6th step", "The sixth step is to make sure {0} is a single whitespace");

        public const string WhitespaceCheckIncorrect = "MetaAnalyzer033";
        internal static DiagnosticDescriptor WhitespaceCheckIncorrectRule = CreateRule(WhitespaceCheckIncorrect, "Incorrect 6th step", "This statement should check to see if {0} is a single whitespace");

        public const string ReturnStatementMissing = "MetaAnalyzer034";
        internal static DiagnosticDescriptor ReturnStatementMissingRule = CreateRule(ReturnStatementMissing, "Missing 7th step", "The seventh step is to return from {0}");

        public const string ReturnStatementIncorrect = "MetaAnalyzer035";
        internal static DiagnosticDescriptor ReturnStatementIncorrectRule = CreateRule(ReturnStatementIncorrect, "Incorrect 7th step", "This statement should return from {0}, because reaching this point in the code means that the if statement being analyzed has the correct spacing");
        #endregion

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(MissingIdRule, 
                                             MissingInitRule, 
                                             MissingRegisterRule, 
                                             TooManyInitStatementsRule, 
                                             IncorrectInitSigRule,
                                             MissingSuppDiagRule,
                                             IncorrectSigSuppDiagRule,
                                             MissingAccessorRule,
                                             TooManyAccessorsRule,
                                             IncorrectAccessorReturnRule,
                                             SuppDiagReturnValueRule,
                                             SupportedRulesRule,
                                             MissingIdDeclarationRule, 
                                             EnabledByDefaultErrorRule, 
                                             DefaultSeverityErrorRule, 
                                             InternalAndStaticErrorRule,
                                             MissingRuleRule,
                                             MissingAnalysisMethodRule,
                                             IfStatementMissingRule,
                                             IfKeywordMissingRule,
                                             IfStatementIncorrectRule,
                                             IdDeclTypeErrorRule,
                                             IfStatementMissingRule,
                                             IfKeywordMissingRule,
                                             IfStatementIncorrectRule,
                                             IfKeywordIncorrectRule,
                                             TrailingTriviaCheckMissingRule,
                                             TrailingTriviaCheckIncorrectRule,
                                             TrailingTriviaVarMissingRule,
                                             TrailingTriviaVarIncorrectRule,
                                             InvalidStatementRule,
                                             IdDeclTypeErrorRule,
                                             TrailingTriviaKindCheckIncorrectRule,
                                             TrailingTriviaKindCheckMissingRule,
                                             WhitespaceCheckMissingRule,
                                             WhitespaceCheckIncorrectRule,
                                             ReturnStatementMissingRule,
                                             ReturnStatementIncorrectRule,
                                             TooManyStatementsRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(SetupAnalysis);
        }

        private void SetupAnalysis(CompilationStartAnalysisContext context)
        {
            //information collector
            CompilationAnalyzer compilationAnalyzer = new CompilationAnalyzer();

            context.RegisterSymbolAction(compilationAnalyzer.AddClass, SymbolKind.NamedType);
            context.RegisterSymbolAction(compilationAnalyzer.AddMethod, SymbolKind.Method);
            context.RegisterSymbolAction(compilationAnalyzer.AddField, SymbolKind.Field);
            context.RegisterSymbolAction(compilationAnalyzer.AddProperty, SymbolKind.Property);

            context.RegisterCompilationEndAction(compilationAnalyzer.ReportCompilationEndDiagnostics);
        }

        class CompilationAnalyzer
        {
            private List<IMethodSymbol> _analyzerMethodSymbols = new List<IMethodSymbol>();
            private List<IPropertySymbol> _analyzerPropertySymbols = new List<IPropertySymbol>();
            private List<IFieldSymbol> _analyzerFieldSymbols = new List<IFieldSymbol>();
            private List<INamedTypeSymbol> _otherClassSymbols = new List<INamedTypeSymbol>();
            private IMethodSymbol _initializeSymbol = null;
            private IPropertySymbol _propertySymbol = null; 
            private INamedTypeSymbol _analyzerClassSymbol = null;
            private Dictionary<string, string> _branchesDict = new Dictionary<string, string>();

            internal void ReportCompilationEndDiagnostics(CompilationAnalysisContext context)
            {
                //supported main branches for tutorial
                _branchesDict.Add("RegisterSyntaxNodeAction", "SyntaxNode");

                //supported sub-branches for tutorial
                List<string> allowedKinds = new List<string>();
                allowedKinds.Add("IfStatement");

                if (_analyzerClassSymbol == null)
                {
                    return;
                }

                //gather initialize info
                List<object> registerInfo = CheckInitialize(context);
                if (registerInfo == null)
                {
                    return;
                }
                var registerSymbol = (IMethodSymbol)registerInfo[0];
                if (registerSymbol == null)
                {
                    return;
                }
                var registerArgs = (List<ISymbol>)registerInfo[1];
                if (registerArgs == null)
                {
                    return;
                }
                if (registerArgs.Count == 0)
                {
                    return;
                }
                IMethodSymbol analysisMethodSymbol = null;
                if (registerArgs.Count > 0)
                {
                        analysisMethodSymbol = (IMethodSymbol)registerArgs[0];
                }
                IFieldSymbol kind = null;
                if (registerArgs.Count > 1)
                {
                    kind = (IFieldSymbol)registerArgs[1];
                }

                var invocationExpression = (InvocationExpressionSyntax)registerInfo[2];
                if (invocationExpression == null)
                {
                    return;
                }
                //interpret initialize info
                if (_branchesDict.ContainsKey(registerSymbol.Name))
                {
                    string kindName = null;
                    if (kind != null)
                    {
                        kindName = kind.Name;
                    }

                    if (kindName == null || allowedKinds.Contains(kindName))
                    {
                        //look for and interpret id fields
                        List<string> idNames = CheckIds(_branchesDict[registerSymbol.Name], kindName, context);
                        if (idNames.Count > 0)
                        {
                            //look for and interpret rule fields
                            List<string> ruleNames = CheckRules(idNames, _branchesDict[registerSymbol.Name], kindName, context);

                            if (ruleNames.Count > 0)
                            {
                                //look for and interpret SupportedDiagnostics property
                               bool supportedDiagnosticsCorrect = CheckSupportedDiagnostics(ruleNames, context);

                                if (supportedDiagnosticsCorrect)
                                {
                                    //check the SyntaxNode, Symbol, Compilation, CodeBlock, etc analysis method(s)
                                    bool analysisCorrect = CheckAnalysis(_branchesDict[registerSymbol.Name], kindName, ruleNames, context, analysisMethodSymbol);
                                    if (analysisCorrect)
                                    {
                                        //diagnostic to go to code fix
                                    }
                                    else
                                    {
                                        //diagnostic
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                var analyzerClass = _analyzerClassSymbol.DeclaringSyntaxReferences[0].GetSyntax() as ClassDeclarationSyntax;
                                ReportDiagnostic(context, MissingRuleRule, analyzerClass.Identifier.GetLocation(), MissingRuleRule.MessageFormat);
                            }
                        }
                        else
                        {
                            // diagnostic for missing id names
                           ReportDiagnostic(context, MissingIdRule, _analyzerClassSymbol.Locations[0], _analyzerClassSymbol.Name.ToString());
                        }
                    }
                    else
                    {
                        ReportDiagnostic(context, InvalidStatementRule, invocationExpression.GetLocation(), invocationExpression);
                    }
                }
                else
                {
                    return;
                }
            }
            
            //checks the syntax tree analysis part of the analyzer
            internal bool CheckAnalysis(string branch, string kind, List<string> ruleNames, CompilationAnalysisContext context, IMethodSymbol analysisMethodSymbol)
            {
                if (branch == "SyntaxNode")
                {
                    if (kind == "IfStatement")
                    {
                        return CheckIfStatementAnalysis(branch, kind, ruleNames, context, analysisMethodSymbol);
                    }
                }

                return false;
            }

            #region CheckAnalysis for IfStatement
            internal bool CheckIfStatementAnalysis(string branch, string kind, List<string> ruleNames, CompilationAnalysisContext context, IMethodSymbol analysisMethodSymbol)
            {
                var getStatements = AnalysisGetStatements(analysisMethodSymbol);
                if (getStatements.Count == 0)
                {
                    return false;
                }

                var methodDeclaration = getStatements[0] as MethodDeclarationSyntax;
                var statements = (SyntaxList<StatementSyntax>)getStatements[1];
                var contextParameter = methodDeclaration.ParameterList.Parameters[0] as ParameterSyntax;
                if (contextParameter == null)
                {
                    return false;
                }

                int statementCount = statements.Count;

                if (statementCount > 0)
                {
                    SyntaxToken statementIdentifierToken = IfStatementAnalysis1(context, statements, contextParameter);
                    if (statementIdentifierToken.Text == "")
                    {
                        ReportDiagnostic(context, IfStatementIncorrectRule, statements[0].GetLocation(), contextParameter.Identifier.Text);
                        return false;
                    }

                    if (statementCount > 1)
                    {
                        SyntaxToken keywordIdentifierToken = IfStatementAnalysis2(context, statements, statementIdentifierToken);
                        if (keywordIdentifierToken.Text == "")
                        {
                            ReportDiagnostic(context, IfKeywordIncorrectRule, statements[1].GetLocation(), statementIdentifierToken.Text);
                            return false;
                        }

                        //outer if statement in user analyzer
                        if (statementCount > 2)
                        {
                            var triviaBlock = IfStatementAnalysis3(context, statements, keywordIdentifierToken) as BlockSyntax;
                            if (triviaBlock == null)
                            {
                                ReportDiagnostic(context, TrailingTriviaCheckIncorrectRule, statements[2].GetLocation(), keywordIdentifierToken.Text);
                                return false;
                            }

                            SyntaxList<StatementSyntax> triviaBlockStatements = triviaBlock.Statements;
                            if (triviaBlockStatements == null)
                            {
                                ReportDiagnostic(context, TrailingTriviaVarMissingRule, triviaBlock.GetLocation(), keywordIdentifierToken.Text);
                                return false;
                            }

                            if (triviaBlockStatements.Count > 0)
                            {
                                SyntaxToken triviaIdentifierToken = IfStatementAnalysis4(context, triviaBlockStatements, keywordIdentifierToken);
                                if (triviaIdentifierToken.Text == "")
                                {
                                    ReportDiagnostic(context, TrailingTriviaVarIncorrectRule, triviaBlockStatements[0].GetLocation(), keywordIdentifierToken.Text);
                                    return false;
                                }

                                //inner if statement in user analyzer
                                if (triviaBlockStatements.Count > 1)
                                {
                                    BlockSyntax triviaKindCheckBlock = IfStatementAnalysis5(context, triviaBlockStatements, triviaIdentifierToken);
                                    if (triviaKindCheckBlock == null)
                                    {
                                        ReportDiagnostic(context, TrailingTriviaKindCheckIncorrectRule, triviaBlockStatements[1].GetLocation(), triviaIdentifierToken.Text);
                                        return false;
                                    }

                                    SyntaxList<StatementSyntax> triviaKindCheckBlockStatements = triviaKindCheckBlock.Statements;
                                    if (triviaKindCheckBlockStatements == null)
                                    {
                                        ReportDiagnostic(context, TrailingTriviaKindCheckIncorrectRule, triviaBlockStatements[1].GetLocation(), triviaIdentifierToken.Text);
                                        return false;
                                    }

                                    //inner inner if statement in user analyzer
                                    if (triviaKindCheckBlockStatements.Count > 0)
                                    {
                                        BlockSyntax triviaCheckBlock = IfStatementAnalysis6(context, triviaKindCheckBlock.Statements, triviaIdentifierToken);
                                        if (triviaCheckBlock == null)
                                        {
                                            ReportDiagnostic(context, WhitespaceCheckIncorrectRule, triviaKindCheckBlockStatements[0].GetLocation(), triviaIdentifierToken);
                                            return false;
                                        }

                                        SyntaxList<StatementSyntax> triviaCheckBlockStatements = triviaCheckBlock.Statements;
                                        if (triviaCheckBlockStatements == null)
                                        {
                                            ReportDiagnostic(context, WhitespaceCheckIncorrectRule, triviaKindCheckBlockStatements[0].GetLocation(), triviaIdentifierToken);
                                            return false;
                                        }

                                        if (triviaCheckBlockStatements.Count > 0)
                                        {
                                            if (!IfStatementAnalysis7(context, triviaCheckBlockStatements))
                                            {
                                                ReportDiagnostic(context, ReturnStatementIncorrectRule, triviaCheckBlockStatements[0].GetLocation(), methodDeclaration.Identifier.Text);
                                                return false;
                                            }

                                            if (triviaCheckBlockStatements.Count > 1)
                                            {
                                                ReportDiagnostic(context, TooManyStatementsRule, triviaCheckBlock.GetLocation(), "if block", "1");
                                                return false;
                                            }
                                            //successfully through if statement checks
                                        }
                                        else
                                        {
                                            ReportDiagnostic(context, ReturnStatementMissingRule, triviaCheckBlock.GetLocation(), methodDeclaration.Identifier.Text);
                                            return false;
                                        }

                                        if (triviaKindCheckBlockStatements.Count > 1)
                                        {
                                            ReportDiagnostic(context, TooManyStatementsRule, triviaKindCheckBlock.GetLocation(), "if block", "1");
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        ReportDiagnostic(context, WhitespaceCheckMissingRule, triviaKindCheckBlock.GetLocation(), triviaIdentifierToken);
                                        return false;
                                    }

                                    if (triviaBlockStatements.Count > 2)
                                    {
                                        ReportDiagnostic(context, TooManyStatementsRule, triviaBlock.GetLocation(), "if block", "2");
                                    }
                                }
                                else
                                {
                                    ReportDiagnostic(context, TrailingTriviaKindCheckMissingRule, triviaBlockStatements[0].GetLocation(), triviaIdentifierToken.Text);
                                    return false;
                                }
                            }
                            else
                            {
                                ReportDiagnostic(context, TrailingTriviaVarMissingRule, triviaBlock.GetLocation(), keywordIdentifierToken.Text);
                                return false;
                            }

                            //check diagnostic reporting statements
                            if (statementCount > 3)
                            {
                                bool diagnosticReportingCorrect = CheckDiagnosticReporting(context, statementIdentifierToken, keywordIdentifierToken, ruleNames, statements);
                                if (!diagnosticReportingCorrect)
                                {
                                    return false;
                                }

                                if (statementCount > 10)
                                {
                                    ReportDiagnostic(context, TooManyStatementsRule, methodDeclaration.GetLocation(), "method", "10"); 
                                }
                            }
                        }
                        else
                        {
                            ReportDiagnostic(context, TrailingTriviaCheckMissingRule, statements[1].GetLocation(), keywordIdentifierToken.Text);
                            return false;
                        }
                    }
                    else
                    {
                        ReportDiagnostic(context, IfKeywordMissingRule, statements[0].GetLocation(), statementIdentifierToken.Text);
                    }
                }
                else
                {
                    ReportDiagnostic(context, IfStatementMissingRule, methodDeclaration.Identifier.GetLocation(), contextParameter.Identifier.Text);
                    return false;
                }

                return true;
            }

            internal SyntaxToken IfStatementAnalysis1(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements, ParameterSyntax contextParameter)
            {
                var emptyResult = SyntaxFactory.Identifier("");
                var ifStatement = statements[0] as LocalDeclarationStatementSyntax;
                if (ifStatement == null)
                {
                    return emptyResult;
                }

                var statementName = GetIdentifierTokenFromLocalDecl(ifStatement);
                if (statementName.Text == "")
                {
                    return emptyResult;
                }

                var statementEqualsValueClause = GetEqualsValueClauseFromLocalDecl(ifStatement);
                if (statementEqualsValueClause == null)
                {
                    return emptyResult;
                }

                var statementCastExpression = statementEqualsValueClause.Value as CastExpressionSyntax;
                if (statementCastExpression == null)
                {
                    return emptyResult;
                }

                var statementIdentifier = statementCastExpression.Type as TypeSyntax;
                //TODO: figure out how to make this not use ToString()
                if (statementIdentifier == null || statementIdentifier.ToString() != "IfStatementSyntax")
                {
                    return emptyResult;
                }

                var statementExpression = statementCastExpression.Expression as MemberAccessExpressionSyntax;
                if (statementExpression == null)
                {
                    return emptyResult;
                }

                var statementExpressionIdentifier = statementExpression.Expression as IdentifierNameSyntax;
                if (statementExpressionIdentifier == null || statementExpressionIdentifier.Identifier.Text != contextParameter.Identifier.Text)
                {
                    return emptyResult;
                }

                var statementExpressionNode = statementExpression.Name as IdentifierNameSyntax;
                if (statementExpressionNode == null || statementExpressionNode.Identifier.Text != "Node")
                {
                    return emptyResult;
                }

                return statementName;
            }

            internal SyntaxToken IfStatementAnalysis2(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements, SyntaxToken statementIdentifierToken)
            {
                var emptyResult = SyntaxFactory.Identifier("");
                var statement = statements[1] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken keywordIdentifierToken = GetIdentifierTokenFromLocalDecl(statement);
                if (keywordIdentifierToken.Text == "")
                {
                    return emptyResult;
                }

                var equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var memberExpr = equalsValueClause.Value as MemberAccessExpressionSyntax;
                if (memberExpr == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpr.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != statementIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var name = memberExpr.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "IfKeyword")
                {
                    return emptyResult;
                }

                return keywordIdentifierToken;
            }

            internal BlockSyntax IfStatementAnalysis3(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements, SyntaxToken keywordIdentifierToken)
            {
                BlockSyntax emptyResult = null;

                var statement = statements[2] as IfStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                var booleanExpression = statement.Condition as MemberAccessExpressionSyntax;
                if (booleanExpression == null)
                {
                    return emptyResult;
                }

                var identifier = booleanExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != keywordIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var name = booleanExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "HasTrailingTrivia")
                {
                    return emptyResult;
                }

                var block = statement.Statement as BlockSyntax;
                if (block == null)
                {
                    return emptyResult;
                }

                return block;
            }

            internal SyntaxToken IfStatementAnalysis4(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements, SyntaxToken keywordIdentifierToken)
            {
                var emptyResult = SyntaxFactory.Identifier("");
                var statement = statements[0] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken triviaIdentifierToken = GetIdentifierTokenFromLocalDecl(statement);
                if (triviaIdentifierToken.Text == "")
                {
                    return emptyResult;
                }

                var statementEqualsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (statementEqualsValueClause == null)
                {
                    return emptyResult;
                }

                var invocationExpression = statementEqualsValueClause.Value as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return emptyResult;
                }

                var memberExpr = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpr == null)
                {
                    return emptyResult;
                }

                var memberExprInner = memberExpr.Expression as MemberAccessExpressionSyntax;
                if (memberExprInner == null)
                {
                    return emptyResult;
                }

                var innerIdentifier = memberExprInner.Expression as IdentifierNameSyntax;
                if (innerIdentifier == null || innerIdentifier.Identifier.Text != keywordIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var innerName = memberExprInner.Name as IdentifierNameSyntax;
                if (innerName == null || innerName.Identifier.Text != "TrailingTrivia")
                {
                    return emptyResult;
                }

                var memberExprName = memberExpr.Name as IdentifierNameSyntax;
                if (memberExprName == null || memberExprName.Identifier.Text != "Last")
                {
                    return emptyResult;
                }

                return triviaIdentifierToken;
            }

            internal BlockSyntax IfStatementAnalysis5(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements, SyntaxToken triviaIdentifierToken)
            {
                BlockSyntax emptyResult = null;

                var statement = statements[1] as IfStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                var booleanExpression = statement.Condition as BinaryExpressionSyntax;
                if (booleanExpression == null)
                {
                    return emptyResult;
                }

                var left = booleanExpression.Left as InvocationExpressionSyntax;
                if (left == null)
                {
                    return emptyResult;
                }

                var leftMemberExpr = left.Expression as MemberAccessExpressionSyntax;
                if (leftMemberExpr == null)
                {
                    return emptyResult;
                }

                var leftIdentifier = leftMemberExpr.Expression as IdentifierNameSyntax;
                if (leftIdentifier == null || leftIdentifier.Identifier.Text != triviaIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var leftName = leftMemberExpr.Name as IdentifierNameSyntax;
                if (leftName == null || leftName.Identifier.Text != "Kind")
                {
                    return emptyResult;
                }

                var leftArgumentList = left.ArgumentList as ArgumentListSyntax;
                if (leftArgumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> leftArgs = leftArgumentList.Arguments;
                if (leftArgs == null || leftArgs.Count != 0)
                {
                    return emptyResult;
                }

                var right = booleanExpression.Right as MemberAccessExpressionSyntax;
                if (right == null)
                {
                    return emptyResult;
                }

                var rightIdentifier = right.Expression as IdentifierNameSyntax;
                if (rightIdentifier == null || rightIdentifier.Identifier.Text != "SyntaxKind")
                {
                    return emptyResult;
                }

                var rightName = right.Name as IdentifierNameSyntax;
                if (rightName == null || rightName.Identifier.Text != "WhitespaceTrivia") {
                    return emptyResult;
                }

                var block = statement.Statement as BlockSyntax;
                if (block == null)
                {
                    return emptyResult;
                }

                return block;
            }
            
            internal BlockSyntax IfStatementAnalysis6(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements, SyntaxToken triviaIdentifierToken)
            {
                BlockSyntax emptyResult = null;

                var statement = statements[0] as IfStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                var booleanExpression = statement.Condition as BinaryExpressionSyntax;
                if (booleanExpression == null)
                {
                    return emptyResult;
                }

                var left = booleanExpression.Left as InvocationExpressionSyntax;
                if (left == null)
                {
                    return emptyResult;
                }

                var leftMemberExpr = left.Expression as MemberAccessExpressionSyntax;
                if (leftMemberExpr == null)
                {
                    return emptyResult;
                }

                var leftIdentifier = leftMemberExpr.Expression as IdentifierNameSyntax;
                if (leftIdentifier == null || leftIdentifier.Identifier.Text != triviaIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var leftName = leftMemberExpr.Name as IdentifierNameSyntax;
                if (leftName == null || leftName.Identifier.Text != "ToString")
                {
                    return emptyResult;
                }

                var leftArgumentList = left.ArgumentList as ArgumentListSyntax;
                if (leftArgumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> leftArgs = leftArgumentList.Arguments;
                if (leftArgs == null || leftArgs.Count != 0)
                {
                    return emptyResult;
                }

                var right = booleanExpression.Right as LiteralExpressionSyntax;
                if (right == null)
                {
                    return emptyResult;
                }

                SyntaxToken rightToken = right.Token;
                if (rightToken == null || rightToken.Text != "\" \"")
                {
                    return emptyResult;
                }

                var block = statement.Statement as BlockSyntax;
                if (block == null)
                {
                    return emptyResult;
                }

                return block;
            }

            internal bool IfStatementAnalysis7(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements)
            {
                var statement = statements[0] as ReturnStatementSyntax;
                if (statement == null)
                {
                    return false;
                }

                return true;
            }

            internal bool CheckDiagnosticReporting(CompilationAnalysisContext context, SyntaxToken statementIdentifierToken, SyntaxToken keywordIdentifierToken, List<string> ruleNames, SyntaxList<StatementSyntax> statements)
            {
                throw new NotImplementedException();
            }

            internal EqualsValueClauseSyntax GetEqualsValueClauseFromLocalDecl(LocalDeclarationStatementSyntax statement)
            {
                EqualsValueClauseSyntax emptyResult = null;
                if (statement == null)
                {
                    return emptyResult;
                }

                var variableDeclaration = statement.Declaration as VariableDeclarationSyntax;
                if (variableDeclaration == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;
                if (variables == null || variables.Count != 1)
                {
                    return emptyResult;
                }

                var variableDeclarator = variables[0] as VariableDeclaratorSyntax;
                if (variableDeclarator == null)
                {
                    return emptyResult;
                }

                SyntaxToken identifier = variableDeclarator.Identifier;
                if (identifier == null)
                {
                    return emptyResult;
                }

                var equalsValueClause = variableDeclarator.Initializer as EqualsValueClauseSyntax;
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                return equalsValueClause;
            }

            internal SyntaxToken GetIdentifierTokenFromLocalDecl(LocalDeclarationStatementSyntax statement)
            {
                var emptyResult = SyntaxFactory.Identifier("");
                if (statement == null)
                {
                    return emptyResult;
                }

                var variableDeclaration = statement.Declaration as VariableDeclarationSyntax;
                if (variableDeclaration == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;
                if (variables == null || variables.Count != 1)
                {
                    return emptyResult;
                }

                var variableDeclarator = variables[0] as VariableDeclaratorSyntax;
                if (variableDeclarator == null)
                {
                    return emptyResult;
                }

                SyntaxToken identifier = variableDeclarator.Identifier;
                if (identifier == null)
                {
                    return emptyResult;
                }

                return identifier;
            }
            #endregion

            internal List<object> AnalysisGetStatements(IMethodSymbol analysisMethodSymbol)
            {
                List<object> result = new List<object>();
                if (analysisMethodSymbol == null)
                {
                    return result;
                }

                var methodDeclaration = analysisMethodSymbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                if (methodDeclaration == null)
                {
                    return result;
                }

                var body = methodDeclaration.Body as BlockSyntax;
                if (body == null)
                {
                    return result;
                }

                SyntaxList<StatementSyntax> statements = body.Statements;
                if (statements == null)
                {
                    return result;
                }

                result.Add(methodDeclaration);
                result.Add(statements);
                return result;
            }

            //returns a boolean based on whether or not the SupportedDiagnostics property is correct
            internal bool CheckSupportedDiagnostics(List<string> ruleNames, CompilationAnalysisContext context)
            {
                var propertyDeclaration = SuppDiagPropertySymbol(context);
                if (propertyDeclaration == null)
                {
                    return false;
                }

                SyntaxList<StatementSyntax> statements = SuppDiagAccessor(context, propertyDeclaration);

                if (statements.Count == 0)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, propertyDeclaration.GetLocation(), IncorrectAccessorReturnRule.MessageFormat);
                    return false;
                }

                var getAccessorKeywordLocation = propertyDeclaration.AccessorList.Accessors.First().GetLocation();

                IEnumerable<ReturnStatementSyntax> returnStatements = statements.OfType<ReturnStatementSyntax>();
                if (returnStatements.Count() == 0)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation, IncorrectAccessorReturnRule.MessageFormat);
                    return false;
                }

                ReturnStatementSyntax returnStatement = returnStatements.First();
                if (returnStatement == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation, IncorrectAccessorReturnRule.MessageFormat);
                    return false;
                }

                var returnExpression = returnStatement.Expression;
                if (returnExpression == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation, IncorrectAccessorReturnRule.MessageFormat);
                    return false;
                }

                if (returnExpression is InvocationExpressionSyntax)
                {
                    var valueClause = returnExpression as InvocationExpressionSyntax;
                    var returnDeclaration = returnStatement as ReturnStatementSyntax;
                    SuppDiagReturnCheck(context, valueClause, returnDeclaration.GetLocation(), ruleNames);
                }
                else if (returnExpression is IdentifierNameSyntax)
                {
                    SymbolInfo returnSymbolInfo = context.Compilation.GetSemanticModel(returnStatement.SyntaxTree).GetSymbolInfo(returnExpression as IdentifierNameSyntax);
                    List<object> symbolResult = SuppDiagReturnSymbol(context, returnSymbolInfo, getAccessorKeywordLocation);
                    if (symbolResult.Count == 0)
                    {
                        return false;
                    }

                    InvocationExpressionSyntax valueClause = symbolResult[0] as InvocationExpressionSyntax;
                    VariableDeclaratorSyntax returnDeclaration = symbolResult[1] as VariableDeclaratorSyntax;
                    SuppDiagReturnCheck(context, valueClause, returnDeclaration.GetLocation(), ruleNames);
                }
                else
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation, IncorrectAccessorReturnRule.MessageFormat);
                    return false;
                }

                return true;

            }

            #region CheckSupportedDiagnostics helpers
            internal PropertyDeclarationSyntax SuppDiagPropertySymbol(CompilationAnalysisContext context)
            {
                if (_propertySymbol == null)
                {
                    ReportDiagnostic(context, MissingSuppDiagRule, _analyzerClassSymbol.Locations[0], MissingSuppDiagRule.MessageFormat);
                    return null;
                }

                if (_propertySymbol.Name != "SupportedDiagnostics" || _propertySymbol.DeclaredAccessibility != Accessibility.Public ||
                    !_propertySymbol.IsOverride)
                {
                    ReportDiagnostic(context, IncorrectSigSuppDiagRule, _propertySymbol.Locations[0], IncorrectSigSuppDiagRule.MessageFormat);
                    return null;
                }

                return _propertySymbol.DeclaringSyntaxReferences[0].GetSyntax() as PropertyDeclarationSyntax;
            }

            internal SyntaxList<StatementSyntax> SuppDiagAccessor(CompilationAnalysisContext context, PropertyDeclarationSyntax propertyDeclaration)
            {
                SyntaxList<StatementSyntax> emptyResult = new SyntaxList<StatementSyntax>();

                AccessorListSyntax accessorList = propertyDeclaration.AccessorList;
                if (accessorList == null)
                {
                    return emptyResult;
                }

                SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;
                if (accessors == null || accessors.Count == 0)
                {
                    ReportDiagnostic(context, MissingAccessorRule, propertyDeclaration.GetLocation(), MissingAccessorRule.MessageFormat);
                    return emptyResult;
                }
                if (accessors.Count > 1)
                {
                    ReportDiagnostic(context, TooManyAccessorsRule, accessorList.GetLocation(), TooManyAccessorsRule.MessageFormat);
                    return emptyResult;
                }

                var getAccessor = accessors.First() as AccessorDeclarationSyntax;
                if (getAccessor == null || getAccessor.Keyword.Kind() != SyntaxKind.GetKeyword)
                {
                    ReportDiagnostic(context, MissingAccessorRule, propertyDeclaration.GetLocation(), MissingAccessorRule.MessageFormat);
                    return emptyResult;
                }

                var accessorBody = getAccessor.Body as BlockSyntax;
                if (accessorBody == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessor.Keyword.GetLocation(), IncorrectAccessorReturnRule.MessageFormat);
                    return emptyResult;
                }

                return accessorBody.Statements;
            }

            internal void SuppDiagReturnCheck(CompilationAnalysisContext context, InvocationExpressionSyntax valueClause, Location returnDeclarationLocation, List<string> ruleNames)
            {
                if (valueClause == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnDeclarationLocation, IncorrectAccessorReturnRule.MessageFormat);
                    return;
                }

                var valueExpression = valueClause.Expression as MemberAccessExpressionSyntax;
                if (valueExpression == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnDeclarationLocation, IncorrectAccessorReturnRule.MessageFormat);
                    return;
                }

                if (valueExpression.ToString() != "ImmutableArray.Create")
                {
                    ReportDiagnostic(context, SuppDiagReturnValueRule, returnDeclarationLocation, SuppDiagReturnValueRule.MessageFormat);
                    return;
                }

                var valueArguments = valueClause.ArgumentList as ArgumentListSyntax;
                if (valueArguments == null)
                {
                    return;
                }

                SeparatedSyntaxList<ArgumentSyntax> valueArgs = valueArguments.Arguments;
                if (valueArgs == null)
                {
                    return;
                }

                foreach (ArgumentSyntax arg in valueArgs)
                {
                    if (ruleNames.Count == 0)
                    {
                        ReportDiagnostic(context, SupportedRulesRule, valueExpression.GetLocation(), SupportedRulesRule.MessageFormat);
                        return;
                    }
                    if (ruleNames.Contains(arg.ToString()))
                    {
                        ruleNames.Remove(arg.ToString());
                    }
                }
            }

            internal List<object> SuppDiagReturnSymbol(CompilationAnalysisContext context, SymbolInfo returnSymbolInfo, Location getAccessorKeywordLocation)
            {
                List<object> result = new List<object>();

                ILocalSymbol returnSymbol = null;
                if (returnSymbolInfo.CandidateSymbols.Count() == 0)
                {
                    returnSymbol = returnSymbolInfo.Symbol as ILocalSymbol;
                }
                else
                {
                    returnSymbol = returnSymbolInfo.CandidateSymbols[0] as ILocalSymbol;
                }

                if (returnSymbol == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation, IncorrectAccessorReturnRule.MessageFormat);
                    return result;
                }

                if (returnSymbol.Type.Name != "System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.DiagnosticDescriptor>")
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnSymbol.Locations[0], IncorrectAccessorReturnRule.MessageFormat);
                    return result;
                }

                var returnDeclaration = returnSymbol.DeclaringSyntaxReferences[0].GetSyntax() as VariableDeclaratorSyntax;
                if (returnDeclaration == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnSymbol.Locations[0], IncorrectAccessorReturnRule.MessageFormat);
                    return result;
                }

                var equalsValueClause = returnDeclaration.Initializer as EqualsValueClauseSyntax;
                if (equalsValueClause == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnDeclaration.GetLocation(), IncorrectAccessorReturnRule.MessageFormat);
                    return result;
                }

                var valueClause = equalsValueClause.Value as InvocationExpressionSyntax;

                result.Add(valueClause);
                result.Add(returnDeclaration);
                return result;
            }
            #endregion

            //returns a list of rule names
            internal List<string> CheckRules(List<string> idNames, string branch, string kind, CompilationAnalysisContext context)
            {
                List<string> ruleNames = new List<string>();
                List<string> emptyRuleNames = new List<string>();

                foreach (var fieldSymbol in _analyzerFieldSymbols)
                {
                    if (fieldSymbol.Type != null &&  fieldSymbol.Type.MetadataName == "DiagnosticDescriptor")
                    {
                        if (fieldSymbol.DeclaredAccessibility != Accessibility.Internal || !fieldSymbol.IsStatic)
                        {
                            ReportDiagnostic(context, InternalAndStaticErrorRule, fieldSymbol.Locations[0], InternalAndStaticErrorRule.MessageFormat);
                            return emptyRuleNames;
                        }

                        var declaratorSyntax = fieldSymbol.DeclaringSyntaxReferences[0].GetSyntax() as VariableDeclaratorSyntax;
                        if (declaratorSyntax == null)
                        {
                            return emptyRuleNames;
                        }

                        var objectCreationSyntax = declaratorSyntax.Initializer.Value as ObjectCreationExpressionSyntax;
                        if (objectCreationSyntax == null)
                        {
                            return emptyRuleNames;
                        }

                        var ruleArgumentList = objectCreationSyntax.ArgumentList;

                        for (int i = 0; i < ruleArgumentList.Arguments.Count; i++)
                        {
                            var currentArg = ruleArgumentList.Arguments[i];
                            if (currentArg == null)
                            {
                                return emptyRuleNames;
                            }

                            var currentArgExpr = currentArg.Expression;
                            if (currentArgExpr == null)
                            {
                                return emptyRuleNames;
                            }

                            if (currentArg.NameColon != null)
                            {
                                string currentArgName = currentArg.NameColon.Name.Identifier.Text;

                                if (currentArgName == "isEnabledByDefault" && !currentArgExpr.IsKind(SyntaxKind.TrueLiteralExpression))
                                {
                                    ReportDiagnostic(context, EnabledByDefaultErrorRule, currentArgExpr.GetLocation(), EnabledByDefaultErrorRule.MessageFormat);
                                    return emptyRuleNames;
                                }
                                else if (currentArgName == "defaultSeverity")
                                {
                                    var memberAccessExpr = currentArgExpr as MemberAccessExpressionSyntax;
                                    if (memberAccessExpr == null)
                                    {
                                        return emptyRuleNames;
                                    }

                                    if (memberAccessExpr.Expression != null && memberAccessExpr.Name != null)
                                    {
                                        string identifierExpr = memberAccessExpr.Expression.ToString();
                                        string identifierName = memberAccessExpr.Name.Identifier.Text;
                                        if (identifierExpr != "DiagnosticSeverity" && (identifierName != "Warning" || identifierName != "Error" || identifierName != "Hidden" || identifierName != "Info"))
                                        {
                                            ReportDiagnostic(context, DefaultSeverityErrorRule, currentArgExpr.GetLocation(), DefaultSeverityErrorRule.MessageFormat);
                                            return emptyRuleNames;
                                        }
                                    }
                                    else
                                    {
                                        return emptyRuleNames;
                                    }
                                }
                                else if (currentArgName == "id")
                                {
                                    if (currentArgExpr.IsKind(SyntaxKind.StringLiteralExpression))
                                    {
                                        ReportDiagnostic(context, IdDeclTypeErrorRule, currentArgExpr.GetLocation(), IdDeclTypeErrorRule.MessageFormat);
                                        return emptyRuleNames;
                                    }

                                    if (fieldSymbol.Name == null)
                                    {
                                        return emptyRuleNames;
                                    }

                                    var foundId = currentArgExpr.ToString();
                                    var foundRule = fieldSymbol.Name.ToString();
                                    bool ruleIdFound = false;

                                    foreach (string idName in idNames)
                                    {
                                        if (idName == foundId)
                                        {
                                            ruleNames.Add(foundRule);
                                            ruleIdFound = true;
                                        }
                                    }

                                    if (!ruleIdFound)
                                    {
                                        ReportDiagnostic(context, MissingIdDeclarationRule, currentArgExpr.GetLocation(), MissingIdDeclarationRule.MessageFormat);
                                        return emptyRuleNames;
                                    }
                                }
                            }
                        }
                    }
                }
                return ruleNames;
            }

            //returns a list of id names, empty if none found
            internal List<string> CheckIds(string branch, string kind, CompilationAnalysisContext context)
            {
                List<string> idNames = new List<string>();
                foreach (IFieldSymbol field in _analyzerFieldSymbols)
                {
                    if (field.IsConst && field.IsStatic && field.DeclaredAccessibility == Accessibility.Public && field.Type.SpecialType == SpecialType.System_String)
                    {
                        if (field.Name == null)
                        {
                            continue;
                        }
                        idNames.Add(field.Name);
                    }
                }
                return idNames;
            }

            //returns a symbol for the register call, and a list of the arguments
            internal List<object> CheckInitialize(CompilationAnalysisContext context)
            {
                //default values for returning
                IMethodSymbol registerCall = null;
                List<ISymbol> registerArgs = new List<ISymbol>();
                InvocationExpressionSyntax invocExpr = null;
                
                if (_initializeSymbol == null)
                {
                    //the initialize method was not found
                    ReportDiagnostic(context, MissingInitRule, _analyzerClassSymbol.Locations[0], _analyzerClassSymbol.Name.ToString());
                    return new List<object>(new object[] { registerCall, registerArgs });
                }
                else
                {
                    //checking method signature
                    var codeBlock = InitializeOverview(context) as BlockSyntax;
                    if (codeBlock == null)
                    {
                        return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                    }

                    SyntaxList<StatementSyntax> statements = codeBlock.Statements;
                    if (statements.Count == 0)
                    {
                        //no statements inside initiailize
                        ReportDiagnostic(context, MissingRegisterRule, _initializeSymbol.Locations[0], _initializeSymbol.Name.ToString());
                        return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                    }
                    else if (statements.Count > 1)
                    {
                        foreach (var statement in statements)
                        {
                            if (statement.Kind() != SyntaxKind.ExpressionStatement)
                            {
                                ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation(), statement.ToString());
                                return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                            }
                        }

                        if (statements.Count() > 1)
                        {
                            foreach (ExpressionStatementSyntax statement in statements)
                            {
                                var expression = statement.Expression as InvocationExpressionSyntax;
                                if (expression == null)
                                {
                                    ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation(), statement.ToString());
                                    return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                                }

                                var expressionStart = expression.Expression as MemberAccessExpressionSyntax;
                                if (expressionStart == null || expressionStart.Name == null)
                                {
                                    ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation(), statement.ToString());
                                    return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                                }

                                var preExpressionStart = expressionStart.Expression as IdentifierNameSyntax;
                                if (preExpressionStart == null || preExpressionStart.Identifier == null ||
                                    preExpressionStart.Identifier.ValueText != "context")
                                {
                                    ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation(), statement.ToString());
                                    return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                                }

                                var name = expressionStart.Name.ToString();
                                if (!_branchesDict.ContainsKey(name))
                                {
                                    ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation(), statement.ToString());
                                    return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                                }
                            }

                            if (statements.Count() > 1)
                            {
                                //too many statements inside initialize
                                ReportDiagnostic(context, TooManyInitStatementsRule, _initializeSymbol.Locations[0], _initializeSymbol.Name.ToString());
                                return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                            }
                        }
                        return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                    }
                    //only one statement inside initialize
                    else
                    {
                        List<object> bodyResults = InitializeBody(context, statements);
                        if (bodyResults == null)
                        {
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }
                        var invocationExpr = bodyResults[0] as InvocationExpressionSyntax;
                        var memberExpr = bodyResults[1] as MemberAccessExpressionSyntax;
                        invocExpr = invocationExpr;

                        if (context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(memberExpr).CandidateSymbols.Count() == 0)
                        {
                            registerCall = context.Compilation.GetSemanticModel(memberExpr.SyntaxTree).GetSymbolInfo(memberExpr).Symbol as IMethodSymbol;
                        }
                        else
                        {
                            registerCall = context.Compilation.GetSemanticModel(memberExpr.SyntaxTree).GetSymbolInfo(memberExpr).CandidateSymbols[0] as IMethodSymbol;
                        }

                        if (registerCall == null)
                        {
                            return new List<object>(new object[] { registerCall, registerArgs });
                        }

                        SeparatedSyntaxList<ArgumentSyntax> arguments = invocationExpr.ArgumentList.Arguments;
                        if (arguments == null)
                        {
                            ReportDiagnostic(context, InvalidStatementRule, memberExpr.GetLocation(), memberExpr.Name.ToString());
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }
                        if (arguments.Count() > 0)
                        {
                            IMethodSymbol actionSymbol = context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(arguments[0].Expression).Symbol as IMethodSymbol;
                            registerArgs.Add(actionSymbol);

                            if (arguments.Count() > 1)
                            {
                                IFieldSymbol kindSymbol = context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(arguments[1].Expression).Symbol as IFieldSymbol;
                                if (kindSymbol == null)
                                {
                                    return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                                }
                                else
                                {
                                    registerArgs.Add(kindSymbol);
                                }
                            }
                        }
                    }
                }
                

                return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
            }

            #region CheckInitialize helpers
            internal BlockSyntax InitializeOverview(CompilationAnalysisContext context)
            {
                ImmutableArray<IParameterSymbol> parameters = _initializeSymbol.Parameters;
                if (parameters.Count() != 1 || parameters[0].Type != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.AnalysisContext") 
                    || parameters[0].Name != "context" || _initializeSymbol.DeclaredAccessibility != Accessibility.Public 
                    || !_initializeSymbol.IsOverride || !_initializeSymbol.ReturnsVoid)
                {
                    ReportDiagnostic(context, IncorrectInitSigRule, _initializeSymbol.Locations[0], _initializeSymbol.Name.ToString());
                    return null;
                }

                //looking at the contents of the initialize method
                var initializeMethod = _initializeSymbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                if (initializeMethod == null)
                {
                    return null;
                }

                var codeBlock = initializeMethod.Body as BlockSyntax;
                if (codeBlock == null)
                {
                    return null;
                }

                return codeBlock;
            }

            internal List<object> InitializeBody(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements)
            {
                var statement = statements[0] as ExpressionStatementSyntax;
                if (statement == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation(), statements[0]);
                    return null;
                }

                var invocationExpr = statement.Expression as InvocationExpressionSyntax;
                if (invocationExpr == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation(), statements[0]);
                    return null;
                }

                var memberExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
                if (memberExpr == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation(), statements[0]);
                    return null;
                }

                var memberExprContext = memberExpr.Expression as IdentifierNameSyntax;
                if (memberExprContext == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation(), statements[0]);
                    return null;
                }

                if (memberExprContext.Identifier.Text != "context")
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation(), statements[0]);
                    return null;
                }

                var memberExprRegister = memberExpr.Name as IdentifierNameSyntax;
                if (memberExprRegister == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation(), statements[0]);
                    return null;
                }

                if (!_branchesDict.ContainsKey(memberExprRegister.ToString()))
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation(), statements[0]);
                    return null;
                }

                return new List<object>(new object[] { invocationExpr, memberExpr });
            }
            #endregion

            #region symbol collectors
            internal void AddMethod(SymbolAnalysisContext context)
            {
                var sym = (IMethodSymbol)context.Symbol;

                if (sym == null)
                {
                    return;
                }
                if (sym.ContainingType == null)
                {
                    return;
                }
                if (sym.ContainingType.BaseType == null)
                {
                    return;
                }
                if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    return;
                }
                if (_analyzerMethodSymbols.Contains(sym))
                {
                    return;
                }

                if (sym.Name == "Initialize")
                {
                    _initializeSymbol = sym;
                    return;
                }

                _analyzerMethodSymbols.Add(sym);
            }

            internal void AddProperty(SymbolAnalysisContext context)
            {
                var sym = (IPropertySymbol)context.Symbol;

                if (sym == null)
                {
                    return;
                }
                if (sym.ContainingType == null)
                {
                    return;
                }
                if (sym.ContainingType.BaseType == null)
                {
                    return;
                }
                if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    return;
                }
                if (_analyzerPropertySymbols.Contains(sym))
                {
                    return;
                }

                if (sym.Name == "SupportedDiagnostics")
                {
                    _propertySymbol = sym;
                    return;
                }

                _analyzerPropertySymbols.Add(sym);
            }

            internal void AddField(SymbolAnalysisContext context)
            {
                var sym = (IFieldSymbol)context.Symbol;

                if (sym == null)
                {
                    return;
                }
                if (sym.ContainingType == null)
                {
                    return;
                }
                if (sym.ContainingType.BaseType == null)
                {
                    return;
                }
                if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    return;
                }
                if (_analyzerFieldSymbols.Contains(sym))
                {
                    return;
                }

                _analyzerFieldSymbols.Add(sym);
            }

            internal void AddClass(SymbolAnalysisContext context)
            {
                var sym = (INamedTypeSymbol)context.Symbol;

                if (sym == null)
                {
                    return;
                }
                if (sym.BaseType == null)
                {
                    return;
                }
                if (sym.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    if (sym.ContainingType == null)
                    {
                        return;
                    }
                    if (sym.ContainingType.BaseType == null)
                    {
                        return;
                    }
                    if (sym.ContainingType.BaseType == context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                    {
                        if (_otherClassSymbols.Contains(sym))
                        {
                            return;
                        }
                        else
                        {
                            _otherClassSymbols.Add(sym);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                _analyzerClassSymbol = sym;
            }
            #endregion

            internal void ClearState()
            {
                _analyzerClassSymbol = null;
                _analyzerFieldSymbols = new List<IFieldSymbol>();
                _analyzerMethodSymbols = new List<IMethodSymbol>();
                _analyzerPropertySymbols = new List<IPropertySymbol>();
            }

            public static void ReportDiagnostic(CompilationAnalysisContext context, DiagnosticDescriptor rule, Location location, params object[] messageArgs)
            {
                Diagnostic diagnostic = Diagnostic.Create(rule, location, messageArgs);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
