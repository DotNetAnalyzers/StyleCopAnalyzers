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
        #region rule rules

        public const string IdDeclTypeError = "MetaAnalyzer018";
        internal static DiagnosticDescriptor IdDeclTypeErrorRule = new DiagnosticDescriptor(
            id: IdDeclTypeError,
            title: "Thise diagnostic id type is incorrect.",
            messageFormat: "The diagnostic id should not be a string literal.",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string MissingIdDeclaration = "MetaAnalyzer017";
        internal static DiagnosticDescriptor MissingIdDeclarationRule = new DiagnosticDescriptor(
            id: MissingIdDeclaration,
            title: "The diagnostic id declaration is missing.",
            messageFormat: "This diagnostic id has not been declared.",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string DefaultSeverityError = "MetaAnalyzer016";
        internal static DiagnosticDescriptor DefaultSeverityErrorRule = new DiagnosticDescriptor(
            id: DefaultSeverityError,
            title: "defaultSeverity is incorrectly declared.",
            messageFormat: "defaultSeverity must be of the form: DiagnosticSeverity.[severity].",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string EnabledByDefaultError = "MetaAnalyzer015";
        internal static DiagnosticDescriptor EnabledByDefaultErrorRule = new DiagnosticDescriptor(
            id: EnabledByDefaultError,
            title: "isEnabledByDefault should be set to true.",
            messageFormat: "isEnabledByDefault should be set to true.",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string InternalAndStaticError = "MetaAnalyzer014";
        internal static DiagnosticDescriptor InternalAndStaticErrorRule = new DiagnosticDescriptor(
            id: InternalAndStaticError,
            title: "The DiagnosticDescriptor should be internal and static.",
            messageFormat: "The DiagnosticDescriptor should be internal and static.",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string MissingRule = "MetaAnalyzer019";
        internal static DiagnosticDescriptor MissingRuleRule = new DiagnosticDescriptor(
            id: MissingRule,
            title: "Missing a rule",
            messageFormat: "You need to have at least one DiagnosticDescriptor rule",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region id rules
        public const string MissingId = "MetaAnalyzer001";
        internal static DiagnosticDescriptor MissingIdRule = new DiagnosticDescriptor(
            id: MissingId,
            title: "You are missing a diagnostic id",
            messageFormat: "You are missing a diagnostic id",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region Initialize rules
        public const string MissingInit = "MetaAnalyzer002";
        internal static DiagnosticDescriptor MissingInitRule = new DiagnosticDescriptor(
            id: MissingInit,
            title: "You are missing the required Initialize method",
            messageFormat: "You are missing the required Initialize method",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string MissingRegisterStatement = "MetaAnalyzer003";
        internal static DiagnosticDescriptor MissingRegisterRule = new DiagnosticDescriptor(
            id: MissingRegisterStatement,
            title: "You need to register an action within the Initialize method",
            messageFormat: "You need to register an action within the Initialize method",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string TooManyInitStatements = "MetaAnalyzer004";
        internal static DiagnosticDescriptor TooManyInitStatementsRule = new DiagnosticDescriptor(
            id: TooManyInitStatements,
            title: "Please only have one statement within Initiailize. You will only be registering one action.",
            messageFormat: "Please only have one statement within Initiailize. You will only be registering one action.",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectInitStatement = "MetaAnalyzer005";
        internal static DiagnosticDescriptor IncorrectInitStatementRule = new DiagnosticDescriptor(
            id: IncorrectInitStatement,
            title: "This statement needs to register for a supported action",
            messageFormat: "This statement needs to register for a supported action",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectInitSig = "MetaAnalyzer006";
        internal static DiagnosticDescriptor IncorrectInitSigRule = new DiagnosticDescriptor(
            id: IncorrectInitSig,
            title: "The signature for the Initialize method is incorrect",
            messageFormat: "The signature for the Initialize method is incorrect",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region SupportedDiagnostics rules
        public const string MissingSuppDiag = "MetaAnalyzer007";
        internal static DiagnosticDescriptor MissingSuppDiagRule = new DiagnosticDescriptor(
            id: MissingSuppDiag,
            title: "You are missing the required SupportedDiagnostics method",
            messageFormat: "You are missing the required SupportedDiagnostics method",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectSigSuppDiag = "MetaAnalyzer008";
        internal static DiagnosticDescriptor IncorrectSigSuppDiagRule = new DiagnosticDescriptor(
            id: IncorrectSigSuppDiag,
            title: "The signature of the SupportedDiagnostics property is incorrect",
            messageFormat: "The signature of the SupportedDiagnostics property is incorrect",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string MissingAccessor = "MetaAnalyzer009";
        internal static DiagnosticDescriptor MissingAccessorRule = new DiagnosticDescriptor(
            id: MissingAccessor,
            title: "You are missing a get accessor in your SupportedDiagnostics property",
            messageFormat: "You are missing a get accessor in your SupportedDiagnostics property",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string TooManyAccessors = "MetaAnalyzer010";
        internal static DiagnosticDescriptor TooManyAccessorsRule = new DiagnosticDescriptor(
            id: TooManyAccessors,
            title: "You only need a get accessor for this property",
            messageFormat: "You only need a get accessor for this property",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectAccessorReturn = "MetaAnalyzer011";
        internal static DiagnosticDescriptor IncorrectAccessorReturnRule = new DiagnosticDescriptor(
            id: IncorrectAccessorReturn,
            title: "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules",
            messageFormat: "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string SuppDiagReturnValue = "MetaAnalyzer012";
        internal static DiagnosticDescriptor SuppDiagReturnValueRule = new DiagnosticDescriptor(
            id: SuppDiagReturnValue,
            title: "You need to create an immutable array",
            messageFormat: "You need to create an immutable array",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string SupportedRules = "MetaAnalyzer013";
        internal static DiagnosticDescriptor SupportedRulesRule = new DiagnosticDescriptor(
            id: SupportedRules,
            title: "The immutable array should contain every DiagnosticDescriptor rule that was created",
            messageFormat: "The immutable array should contain every DiagnosticDescriptor rule that was created",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region analysis rules
        public const string MissingAnalysisMethod = "MetaAnalyzer018";
        internal static DiagnosticDescriptor MissingAnalysisMethodRule = new DiagnosticDescriptor(
            id: MissingAnalysisMethod,
            title: "Missing analysis method",
            messageFormat: "You are missing the method that was registered to perform the analysis",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region analysis for IfStatement rules
        public const string IfStatementMissing = "MetaAnalyzer024";
        internal static DiagnosticDescriptor IfStatementMissingRule = new DiagnosticDescriptor(
            id: IfStatementMissing,
            title: "Missing 1st step",
            messageFormat: "The first step is to extract the if statement from {0}",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IfStatementIncorrect = "MetaAnalyzer022";
        internal static DiagnosticDescriptor IfStatementIncorrectRule = new DiagnosticDescriptor(
            id: IfStatementIncorrect,
            title: "If statement extraction incorrect",
            messageFormat: "This statement should extract the if statement in question by casting {0}.Node to IfStatementSyntax",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IfKeywordMissing = "MetaAnalyzer021";
        internal static DiagnosticDescriptor IfKeywordMissingRule = new DiagnosticDescriptor(
            id: IfKeywordMissing,
            title: "Missing 2nd step",
            messageFormat: "The second step is to extract the 'if' keyword from {0}",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IfKeywordIncorrect = "MetaAnalyzer025";
        internal static DiagnosticDescriptor IfKeywordIncorrectRule = new DiagnosticDescriptor(
            id: IfKeywordIncorrect,
            title: "Incorrect 2nd step",
            messageFormat: "This statement should extract the 'if' keyword from {0}",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string TrailingTriviaCheckMissing = "MetaAnalyzer026";
        internal static DiagnosticDescriptor TrailingTriviaCheckMissingRule = new DiagnosticDescriptor(
            id: TrailingTriviaCheckMissing,
            title: "Missing 3rd step",
            messageFormat: "The third step is to begin looking for the space between 'if' and '(' by checking if {0} has trailing trivia",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string TrailingTriviaCheckIncorrect = "MetaAnalyzer027";
        internal static DiagnosticDescriptor TrailingTriviaCheckIncorrectRule = new DiagnosticDescriptor(
            id: TrailingTriviaCheckIncorrect,
            title: "Incorrect 3rd step",
            messageFormat: "This statement should be an if statement that checks to see if {0} has trailing trivia",
            category: "Tutorial",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(MissingIdRule, 
                                             MissingInitRule, 
                                             MissingRegisterRule, 
                                             TooManyInitStatementsRule, 
                                             IncorrectInitStatementRule, 
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
                                             TrailingTriviaCheckIncorrectRule);
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
                           var analyzerClassSyntax = _analyzerClassSymbol.DeclaringSyntaxReferences[0].GetSyntax() as ClassDeclarationSyntax;
                           ReportDiagnostic(context, MissingIdRule, analyzerClassSyntax.Identifier.GetLocation(), MissingIdRule.MessageFormat);
                        }
                    }
                    else
                    {
                        ReportDiagnostic(context, IncorrectInitStatementRule, invocationExpression.GetLocation(), IncorrectInitStatementRule.MessageFormat);
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

                        if (statementCount > 2)
                        {
                            var triviaBlock = IfStatementAnalysis3(context, statements, keywordIdentifierToken) as BlockSyntax;
                            if (triviaBlock == null)
                            {
                                ReportDiagnostic(context, TrailingTriviaCheckIncorrectRule, statements[2].GetLocation(), keywordIdentifierToken.Text);
                                return false;
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
                if (statementName == null)
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
                if (keywordIdentifierToken == null)
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
            //assumes that there is only one thing registered
            internal List<object> CheckInitialize(CompilationAnalysisContext context)
            {
                //default values for returning
                IMethodSymbol registerCall = null;
                List<ISymbol> registerArgs = new List<ISymbol>();
                InvocationExpressionSyntax invocExpr = null;
                
                if (_initializeSymbol == null)
                {
                    //the initialize method was not found
                    ReportDiagnostic(context, MissingInitRule, _analyzerClassSymbol.Locations[0], MissingInitRule.MessageFormat);
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
                        ReportDiagnostic(context, MissingRegisterRule, _initializeSymbol.Locations[0], MissingRegisterRule.MessageFormat);
                        return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                    }
                    else if (statements.Count > 1)
                    {
                        //too many statements inside initialize
                        ReportDiagnostic(context, TooManyInitStatementsRule, statements[0].GetLocation(), TooManyInitStatementsRule.MessageFormat);
                        return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                    }
                    else
                    {
                        //only one statement inside initialize
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
                            ReportDiagnostic(context, IncorrectInitStatementRule, memberExpr.GetLocation(), IncorrectInitStatementRule.MessageFormat);
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
                if (parameters.Count() != 1 || parameters[0].Type != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.AnalysisContext") || parameters[0].Name != "context" || _initializeSymbol.DeclaredAccessibility != Accessibility.Public || !_initializeSymbol.IsOverride || !_initializeSymbol.ReturnsVoid)
                {
                    ReportDiagnostic(context, IncorrectInitSigRule, _initializeSymbol.Locations[0], MissingInitRule.MessageFormat);
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
                    ReportDiagnostic(context, IncorrectInitStatementRule, statements[0].GetLocation(), IncorrectInitStatementRule.MessageFormat);
                    return null;
                }

                var invocationExpr = statement.Expression as InvocationExpressionSyntax;
                if (invocationExpr == null)
                {
                    ReportDiagnostic(context, IncorrectInitStatementRule, statement.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                    return null;
                }

                var memberExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
                if (memberExpr == null)
                {
                    ReportDiagnostic(context, IncorrectInitStatementRule, invocationExpr.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                    return null;
                }

                var memberExprContext = memberExpr.Expression as IdentifierNameSyntax;
                if (memberExprContext == null)
                {
                    ReportDiagnostic(context, IncorrectInitStatementRule, memberExpr.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                    return null;
                }
                if (memberExprContext.Identifier.Text != "context")
                {
                    ReportDiagnostic(context, IncorrectInitStatementRule, memberExprContext.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                    return null;
                }

                var memberExprRegister = memberExpr.Name as IdentifierNameSyntax;
                if (memberExprRegister == null)
                {
                    ReportDiagnostic(context, IncorrectInitStatementRule, memberExpr.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                    return null;
                }
                if (!_branchesDict.ContainsKey(memberExprRegister.ToString()))
                {
                    ReportDiagnostic(context, IncorrectInitStatementRule, memberExprRegister.GetLocation(), IncorrectInitStatementRule.MessageFormat);
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
