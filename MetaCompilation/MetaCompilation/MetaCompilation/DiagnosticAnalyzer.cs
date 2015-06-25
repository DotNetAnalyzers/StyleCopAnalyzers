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
        //default values for the DiagnosticDescriptors
        public const string RuleCategory = "Tutorial";
        public const DiagnosticSeverity RuleDefaultSeverity = DiagnosticSeverity.Error;
        public const bool RuleEnabledByDefault = true;

        //creates a DiagnosticDescriptor with the above defaults
        public static DiagnosticDescriptor CreateRule(string id, string title, string messageFormat, string description = "")
        {
            DiagnosticDescriptor rule = new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                defaultSeverity: RuleDefaultSeverity,
                isEnabledByDefault: RuleEnabledByDefault,
                category: RuleCategory,
                description: description
                );

            return rule;
        }

        #region rule rules
        public const string IdDeclTypeError = "MetaAnalyzer018";
        internal static DiagnosticDescriptor IdDeclTypeErrorRule = CreateRule(IdDeclTypeError, "DiagnosticDescriptor 'id' incorrect", "The diagnostic id should be the const declared above this", "The id parameter of a DiagnosticDescriptor should be a string const declared previously. This is so that the diagnostic id is accessible from the CodeFixProvider.cs file.");

        public const string MissingIdDeclaration = "MetaAnalyzer017";
        internal static DiagnosticDescriptor MissingIdDeclarationRule = CreateRule(MissingIdDeclaration, "Diagnostic id declaration missing", "This diagnostic id has not been declared");

        public const string DefaultSeverityError = "MetaAnalyzer016";
        internal static DiagnosticDescriptor DefaultSeverityErrorRule = CreateRule(DefaultSeverityError, "defaultSeverity incorrect", "defaultSeverity must be of the form: DiagnosticSeverity.[severity]", "There are four option for the severity of the diagnostic: error, warning, hidden, and info. An error is completely not allowed and causes build errors. A warning is something that might be a problem, but is not a build error. An info diagnostic is simply information and is not actually a problem. A hidden diagnostic is raised as an issue, but is not accessible through normal means. At least in simple analyzers you will mostly use error and warning.");

        public const string EnabledByDefaultError = "MetaAnalyzer015";
        internal static DiagnosticDescriptor EnabledByDefaultErrorRule = CreateRule(EnabledByDefaultError, "isEnabledByDefault incorrect", "isEnabledByDefault should be set to true", "This determines whether or not the diagnostic is enabled by default, or the user of the analyzer has to manually enable the diagnostic. Generally it will be set to true.");

        public const string InternalAndStaticError = "MetaAnalyzer014";
        internal static DiagnosticDescriptor InternalAndStaticErrorRule = CreateRule(InternalAndStaticError, "DiagnosticDescriptor modifiers incorrect", "The {0} field should be internal and static");

        public const string MissingRule = "MetaAnalyzer019";
        internal static DiagnosticDescriptor MissingRuleRule = CreateRule(MissingRule, "Missing DiagnosticDescriptor", "You need to have at least one DiagnosticDescriptor rule", "The DiagnosticDescriptor rule is what is reported by the analyzer when it finds a problem, and so there must be at least one");
        #endregion

        #region id rules
        public const string MissingId = "MetaAnalyzer001";
        internal static DiagnosticDescriptor MissingIdRule = CreateRule(MissingId, "Missing diagnostic id", "The analyzer '{0}' is missing a diagnostic id", "The diagnostic id identifies a particular diagnostic so that the diagnotic can be fixed in CodeFixProvider.cs");
        #endregion

        #region Initialize rules
        public const string MissingInit = "MetaAnalyzer002";
        internal static DiagnosticDescriptor MissingInitRule = CreateRule(MissingInit, "Missing Initialize method", "The analyzer '{0}' is missing the required Initialize method", "The Initialize method is required because it is where actions are registered for. Actions are registered to call an analysis method when something specific changes in the syntax tree or semantic model. For example, context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.IfStatement) will call AnalyzeMethod every time an if statement changes in the syntax tree.");

        public const string MissingRegisterStatement = "MetaAnalyzer003";
        internal static DiagnosticDescriptor MissingRegisterRule = CreateRule(MissingRegisterStatement, "An action must be registered within the method", "An action must be registered within the '{0}' method", "The Initialize method must register for at least one action so that some analysis can be performed. Otherwise there is no way any diagnostics could be reported");

        public const string TooManyInitStatements = "MetaAnalyzer004";
        internal static DiagnosticDescriptor TooManyInitStatementsRule = CreateRule(TooManyInitStatements, "The method registers multiple actions", "The '{0}' method registers multiple actions", "For this tutorial only, only one action is registered for. This is not necessarily a general rule");

        public const string IncorrectInitStatement = "MetaAnalyzer005";
        internal static DiagnosticDescriptor IncorrectInitStatementRule = CreateRule(IncorrectInitStatement, "This statement needs to register for a supported action", "This statement needs to register for a supported action", "Your register statement may not be correct for this tutorial. See the code fix for more help");

        public const string IncorrectInitSig = "MetaAnalyzer006";
        internal static DiagnosticDescriptor IncorrectInitSigRule = CreateRule(IncorrectInitSig, "Incorrect method signature", "The signature for the '{0}' method is incorrect", "The Initialize method should override the Initialize method from the DiagnosticAnalyzer abstract class from which your analyzer inherits");

        public const string InvalidStatement = "MetaAnalyzer020";
        internal static DiagnosticDescriptor InvalidStatementRule = CreateRule(InvalidStatement, "Incorrect statement", "The Initialize method only registers actions: the statement '{0}' is invalid");
        #endregion

        #region SupportedDiagnostics rules
        public const string MissingSuppDiag = "MetaAnalyzer007";
        internal static DiagnosticDescriptor MissingSuppDiagRule = CreateRule(MissingSuppDiag, "Missing SupportedDiagnostics property", "You are missing the required SupportedDiagnostics property", "The SupportedDiagnostics property tells the analyzer which diagnostic ids the analyzer supports. In other words, which DiagnosticDescriptors might be reported by the analyzer. Generally any DiagnosticDescriptor that you have created should be returned by SupportedDiagnostics");

        public const string IncorrectSigSuppDiag = "MetaAnalyzer008";
        internal static DiagnosticDescriptor IncorrectSigSuppDiagRule = CreateRule(IncorrectSigSuppDiag, "Incorrect SupportedDiagnostics property", "The signature of the SupportedDiagnostics property is incorrect");

        public const string MissingAccessor = "MetaAnalyzer009";
        internal static DiagnosticDescriptor MissingAccessorRule = CreateRule(MissingAccessor, "Missing get accessor", "The {0} property is missing a get accessor", "The SupportedDiagnostics property needs to have a get accessor, because that is how the ImmutableArray of DiagnosticDescriptors is made accessible");

        public const string TooManyAccessors = "MetaAnalyzer010";
        internal static DiagnosticDescriptor TooManyAccessorsRule = CreateRule(TooManyAccessors, "You only need a get accessor for this property", "The {0} property only needs a get accessor, no set accessor is needed");

        public const string IncorrectAccessorReturn = "MetaAnalyzer011";
        internal static DiagnosticDescriptor IncorrectAccessorReturnRule = CreateRule(IncorrectAccessorReturn, "Get accessor return value incorrect", "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules");

        public const string SuppDiagReturnValue = "MetaAnalyzer012";
        internal static DiagnosticDescriptor SuppDiagReturnValueRule = CreateRule(SuppDiagReturnValue, "SupportedDiagnostics return value incorrect", "The {0} property's get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules");

        public const string SupportedRules = "MetaAnalyzer013";
        internal static DiagnosticDescriptor SupportedRulesRule = CreateRule(SupportedRules, "ImmutableArray incorrect", "The immutable array should contain every DiagnosticDescriptor rule that was created");
        #endregion

        #region analysis rules
        public const string MissingAnalysisMethod = "MetaAnalyzer018";
        internal static DiagnosticDescriptor MissingAnalysisMethodRule = CreateRule(MissingAnalysisMethod, "Missing analysis method", "You are missing the method that was registered to perform the analysis", "In Initialize, the register statement denotes an analysis method to be called when an action is triggered. This method needs to be created");

        public const string TooManyStatements = "MetaAnalyzer036";
        internal static DiagnosticDescriptor TooManyStatementsRule = CreateRule(TooManyStatements, "Too many statements", "This {0} should only have {1} statement(s)", "For the purpose of this tutorial this method has too many statements, use the code fixes to guide you through the creation of this method");

        public const string DiagnosticMissing = "MetaAnalyzer046";
        internal static DiagnosticDescriptor DiagnosticMissingRule = CreateRule(DiagnosticMissing, "Diagnostic variable missing", "The next step is to create a variable to hold the diagnostic");

        public const string DiagnosticIncorrect = "MetaAnalyzer047";
        internal static DiagnosticDescriptor DiagnosticIncorrectRule = CreateRule(DiagnosticIncorrect, "Diagnostic variable incorrect", "This statement should use Diagnostic.Create, {0}, and {1} to create the diagnostic that will be reported", "The diagnostic is created with a DiagnosticDescriptor, a Location, message arguments. The message arguments are the inputs to a format string");

        public const string DiagnosticReportMissing = "MetaAnalyzer048";
        internal static DiagnosticDescriptor DiagnosticReportMissingRule = CreateRule(DiagnosticReportMissing, "Diagnostic report missing", "The next step is to report the diagnostic that has been created");

        public const string DiagnosticReportIncorrect = "MetaAnalyzer049";
        internal static DiagnosticDescriptor DiagnosticReportIncorrectRule = CreateRule(DiagnosticReportIncorrect, "Diagnostic report incorrect", "This statement should use ReportDiagnostic on {0} to report {1}", "A diagnostic is reported to a context of some sort so that the diagnostic can appear in all the right places");
        #endregion

        #region analysis for IfStatement rules
        public const string IfStatementMissing = "MetaAnalyzer023";
        internal static DiagnosticDescriptor IfStatementMissingRule = CreateRule(IfStatementMissing, "Missing 1st step", "The first step of the node analysis is to extract the if statement from {0}");

        public const string IfStatementIncorrect = "MetaAnalyzer022";
        internal static DiagnosticDescriptor IfStatementIncorrectRule = CreateRule(IfStatementIncorrect, "If statement extraction incorrect", "This statement should extract the if statement in question by casting {0}.Node to IfStatementSyntax", "The context parameter has a Node member. This Node is what the register statement from Initialize triggered on, and so should be cast to the expected syntax or symbol type");

        public const string IfKeywordMissing = "MetaAnalyzer021";
        internal static DiagnosticDescriptor IfKeywordMissingRule = CreateRule(IfKeywordMissing, "Missing 2nd step", "The second step is to extract the 'if' keyword from {0}");

        public const string IfKeywordIncorrect = "MetaAnalyzer024";
        internal static DiagnosticDescriptor IfKeywordIncorrectRule = CreateRule(IfKeywordIncorrect, "Incorrect 2nd step", "This statement should extract the 'if' keyword from {0}", "In the syntax tree, a node of type IfStatementSyntax has an IfKeyword attached to it");

        public const string TrailingTriviaCheckMissing = "MetaAnalyzer025";
        internal static DiagnosticDescriptor TrailingTriviaCheckMissingRule = CreateRule(TrailingTriviaCheckMissing, "Missing 3rd step", "The third step is to begin looking for the space between 'if' and '(' by checking if {0} has trailing trivia");

        public const string TrailingTriviaCheckIncorrect = "MetaAnalyzer026";
        internal static DiagnosticDescriptor TrailingTriviaCheckIncorrectRule = CreateRule(TrailingTriviaCheckIncorrect, "Incorrect 3rd step", "This statement should be an if statement that checks to see if {0} has trailing trivia", "Syntax trivia are all the things that aren't actually code (i.e. comments, whitespace, end of line tokens, etc). Each node has trivia attached to it, with trailing trivia being the trivia after the node)");

        public const string TrailingTriviaVarMissing = "MetaAnalyzer027";
        internal static DiagnosticDescriptor TrailingTriviaVarMissingRule = CreateRule(TrailingTriviaVarMissing, "Missing 4th step", "The fourth step is to extract the last trailing trivia of {0} into a variable");

        public const string TrailingTriviaVarIncorrect = "MetaAnalyzer028";
        internal static DiagnosticDescriptor TrailingTriviaVarIncorrectRule = CreateRule(TrailingTriviaVarIncorrect, "Incorrect 4th step", "This statement should extract the last trailing trivia of {0} into a variable", "The last trailing trivia of the 'if' keyword should be a single whitespace. Anything else signifies incorrect formatting");

        public const string TrailingTriviaKindCheckMissing = "MetaAnalyzer029";
        internal static DiagnosticDescriptor TrailingTriviaKindCheckMissingRule = CreateRule(TrailingTriviaKindCheckMissing, "Missing 5th step", "The fifth step is to check the kind of {0}");

        public const string TrailingTriviaKindCheckIncorrect = "MetaAnalyzer030";
        internal static DiagnosticDescriptor TrailingTriviaKindCheckIncorrectRule = CreateRule(TrailingTriviaKindCheckIncorrect, "Incorrect 5th step", "This statement should check to see if the kind of {0} is whitespace trivia");

        public const string WhitespaceCheckMissing = "MetaAnalyzer031";
        internal static DiagnosticDescriptor WhitespaceCheckMissingRule = CreateRule(WhitespaceCheckMissing, "Missing 6th step", "The sixth step is to make sure {0} is a single whitespace");

        public const string WhitespaceCheckIncorrect = "MetaAnalyzer032";
        internal static DiagnosticDescriptor WhitespaceCheckIncorrectRule = CreateRule(WhitespaceCheckIncorrect, "Incorrect 6th step", "This statement should check to see if {0} is a single whitespace");

        public const string ReturnStatementMissing = "MetaAnalyzer033";
        internal static DiagnosticDescriptor ReturnStatementMissingRule = CreateRule(ReturnStatementMissing, "Missing 7th step", "The seventh step is to return from {0}");

        public const string ReturnStatementIncorrect = "MetaAnalyzer034";
        internal static DiagnosticDescriptor ReturnStatementIncorrectRule = CreateRule(ReturnStatementIncorrect, "Incorrect 7th step", "This statement should return from {0}, because reaching this point in the code means that the if statement being analyzed has the correct spacing");

        public const string OpenParenMissing = "MetaAnalyzer035";
        internal static DiagnosticDescriptor OpenParenMissingRule = CreateRule(OpenParenMissing, "Missing open parenthesis variable", "The next step is to extract the open parenthesis of the if statement condition");

        public const string OpenParenIncorrect = "MetaAnalyzer037";
        internal static DiagnosticDescriptor OpenParenIncorrectRule = CreateRule(OpenParenIncorrect, "Open parenthesis variable incorrect", "This statement should extract the open parenthesis of {0} to use as the end of the diagnostic span");

        public const string StartSpanMissing = "MetaAnalyzer038";
        internal static DiagnosticDescriptor StartSpanMissingRule = CreateRule(StartSpanMissing, "Start span variable missing", "The next step is to determine the start of the span for the diagnostic that will be reported");

        public const string StartSpanIncorrect = "MetaAnalyzer039";
        internal static DiagnosticDescriptor StartSpanIncorrectRule = CreateRule(StartSpanIncorrect, "Start span variable incorrect", "This statement should extract the start of the span of {0} into a variable, to be used as the start of the diagnostic span", "Each node in the syntax tree has a span. This span represents the number of character spaces that the node takes up");

        public const string EndSpanMissing = "MetaAnalyzer040";
        internal static DiagnosticDescriptor EndSpanMissingRule = CreateRule(EndSpanMissing, "End span variable missing", "The next step is to determine the end of the span for the diagnostic that is going to be reported");

        public const string EndSpanIncorrect = "MetaAnalyzer041";
        internal static DiagnosticDescriptor EndSpanIncorrectRule = CreateRule(EndSpanIncorrect, "End span variable incorrect", "This statement should extract the start of the span of {0} into a variable, to be used as the end of the diagnostic span", "Each node in the syntax tree has a span. This span represents the number of character spaces that the node takes up");

        public const string SpanMissing = "MetaAnalyzer042";
        internal static DiagnosticDescriptor SpanMissingRule = CreateRule(SpanMissing, "Diagnostic span variable missing", "The next step is to create a variable that is the span of the diagnostic that will be reported");

        public const string SpanIncorrect = "MetaAnalyzer043";
        internal static DiagnosticDescriptor SpanIncorrectRule = CreateRule(SpanIncorrect, "Diagnostic span variable incorrect", "This statement should use TextSpan.FromBound, {0}, and {1} to create the span of the diagnostic that will be reported", "Each node in the syntax tree has a span. This span represents the number of character spaces that the node takes up. TextSpan.FromBounds(start, end) can be used to create a span to use for a diagnostic");

        public const string LocationMissing = "MetaAnalyzer044";
        internal static DiagnosticDescriptor LocationMissingRule = CreateRule(LocationMissing, "Diagnostic location variable missing", "The next step is to create a location for the diagnostic");

        public const string LocationIncorrect = "MetaAnalyzer045";
        internal static DiagnosticDescriptor LocationIncorrectRule = CreateRule(LocationIncorrect, "Diagnostic location variable incorrect", "This statement should use Location.Create, {0}, and {1} to create the location of the diagnostic", "A location can be created by combining a span with a syntax tree. The span is applie to the given syntax tree so that the location within the syntax tree is determined");
        #endregion

        #region code fix rules
        public const string IncorrectFixSig = "MetaAnalyzer46";
        internal static DiagnosticDescriptor IncorrectFixSigRule = CreateRule(IncorrectFixSig, "Signature for Fix function incorrect", "The signature for the {0} method is incorrect", "The Fix method should expect a document, statement, and cancellation token as parameters and return a document asynchronously");
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
                                             DiagnosticReportIncorrectRule,
                                             DiagnosticMissingRule,
                                             DiagnosticIncorrectRule,
                                             DiagnosticReportMissingRule,
                                             OpenParenIncorrectRule,
                                             OpenParenMissingRule,
                                             StartSpanIncorrectRule,
                                             StartSpanMissingRule,
                                             EndSpanIncorrectRule,
                                             EndSpanMissingRule,
                                             SpanIncorrectRule,
                                             SpanMissingRule,
                                             LocationIncorrectRule,
                                             LocationMissingRule,
                                             TooManyStatementsRule,
                                             IncorrectFixSigRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(SetupAnalysis);
        }

        //creates an instance of a class to perform the analysis statefully, and registers for various actions
        private void SetupAnalysis(CompilationStartAnalysisContext context)
        {
            //state collector
            CompilationAnalyzer compilationAnalyzer = new CompilationAnalyzer();

            //collects all class, method, field, and property symbols as state
            context.RegisterSymbolAction(compilationAnalyzer.AddClass, SymbolKind.NamedType);
            context.RegisterSymbolAction(compilationAnalyzer.AddMethod, SymbolKind.Method);
            context.RegisterSymbolAction(compilationAnalyzer.AddField, SymbolKind.Field);
            context.RegisterSymbolAction(compilationAnalyzer.AddProperty, SymbolKind.Property);

            //analyzes the state that has been collected
            context.RegisterCompilationEndAction(compilationAnalyzer.ReportCompilationEndDiagnostics);
        }

        //performs stateful analysis
        class CompilationAnalyzer
        {
            private List<IMethodSymbol> _analyzerMethodSymbols = new List<IMethodSymbol>();
            private List<IPropertySymbol> _analyzerPropertySymbols = new List<IPropertySymbol>();
            private List<IFieldSymbol> _analyzerFieldSymbols = new List<IFieldSymbol>();
            private List<INamedTypeSymbol> _otherAnalyzerClassSymbols = new List<INamedTypeSymbol>();
            private IMethodSymbol _initializeSymbol = null;
            private IPropertySymbol _propertySymbol = null;
            private INamedTypeSymbol _analyzerClassSymbol = null;
            private Dictionary<string, string> _branchesDict = new Dictionary<string, string>();
            private IPropertySymbol _codeFixFixableDiagnostics = null;
            private List<IMethodSymbol> _codeFixMethodSymbols = new List<IMethodSymbol>();
            private IMethodSymbol _registerCodeFixesAsync = null;
            private INamedTypeSymbol _codeFixClassSymbol = null;

            //"main" method, performs the analysis once state has been collected
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
                                        bool codeFixCorrect = CodeFixProviderAnalysis(context);
                                    }
                                    else
                                    {
                                        return;
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

            //checks the syntax tree analysis part of the user analyzer, returns a bool representing whether the check was successful or not
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
            //checks the AnalyzeIfStatement of the user's analyzer, returns a bool representing whether the check was successful or not
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
                                        return false;
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
                                bool diagnosticReportingCorrect = CheckDiagnosticCreation(context, statementIdentifierToken, keywordIdentifierToken, ruleNames, statements, contextParameter);
                                if (!diagnosticReportingCorrect)
                                {
                                    return false;
                                }

                                if (statementCount > 10)
                                {
                                    ReportDiagnostic(context, TooManyStatementsRule, methodDeclaration.GetLocation(), "method", "10");
                                    return false;
                                }
                            }
                            else
                            {
                                ReportDiagnostic(context, OpenParenMissingRule, (statements[2] as IfStatementSyntax).Condition.GetLocation(), statementIdentifierToken.Text);
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
                        return false;
                    }
                }
                else
                {
                    ReportDiagnostic(context, IfStatementMissingRule, methodDeclaration.Identifier.GetLocation(), contextParameter.Identifier.Text);
                    return false;
                }

                return true;
            }

            //checks step one of the user's AnalyzerIfStatement method, returns a SyntaxToken of "" if analysis failed
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

            //checks step two of the user's AnalyzerIfStatement method, returns a SyntaxToken of "" if analysis failed
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

            //checks step three of the user's AnalyzerIfStatement method, returns null if analysis failed
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

            //checks step four of the user's AnalyzerIfStatement method, returns a SyntaxToken of "" if analysis failed
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

            //checks step five of the user's AnalyzerIfStatement method, returns null if analysis failed
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

            //checks step six of the user's AnalyzerIfStatement method, returns null if analysis failed
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

            //checks step seven of the user's AnalyzerIfStatement method, returns a bool representing whether or not analysis failed
            internal bool IfStatementAnalysis7(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements)
            {
                var statement = statements[0] as ReturnStatementSyntax;
                if (statement == null)
                {
                    return false;
                }

                return true;
            }

            //checks the buildup steps of creating a diagnostic, returns a bool representing whether or not analysis failed
            internal bool CheckDiagnosticCreation(CompilationAnalysisContext context, SyntaxToken statementIdentifierToken, SyntaxToken keywordIdentifierToken, List<string> ruleNames, SyntaxList<StatementSyntax> statements, ParameterSyntax contextParameter)
            {
                int statementCount = statements.Count;

                SyntaxToken openParenToken = OpenParenAnalysis(context, statementIdentifierToken, statements);
                if (openParenToken.Text == "")
                {
                    ReportDiagnostic(context, OpenParenIncorrectRule, statements[3].GetLocation(), statementIdentifierToken.Text);
                    return false;
                }

                if (statementCount > 4)
                {
                    SyntaxToken startToken = StartAnalysis(context, keywordIdentifierToken, statements);
                    if (startToken.Text == "")
                    {
                        ReportDiagnostic(context, StartSpanIncorrectRule, statements[4].GetLocation(), keywordIdentifierToken);
                        return false;
                    }

                    if (statementCount > 5)
                    {
                        SyntaxToken endToken = EndAnalysis(context, openParenToken, statements);
                        if (endToken.Text == "")
                        {
                            ReportDiagnostic(context, EndSpanIncorrectRule, statements[5].GetLocation(), openParenToken.Text);
                            return false;
                        }

                        if (statementCount > 6)
                        {
                            SyntaxToken spanToken = SpanAnalysis(context, startToken, endToken, statements);
                            if (spanToken.Text == "")
                            {
                                ReportDiagnostic(context, SpanIncorrectRule, statements[6].GetLocation(), startToken.Text, endToken.Text);
                                return false;
                            }

                            if (statementCount > 7)
                            {
                                SyntaxToken locationToken = LocationAnalysis(context, statementIdentifierToken, spanToken, statements);
                                if (locationToken.Text == "")
                                {
                                    ReportDiagnostic(context, LocationIncorrectRule, statements[7].GetLocation(), statementIdentifierToken.Text, spanToken.Text);
                                    return false;
                                }

                                if (statementCount > 8)
                                {
                                    SyntaxToken diagnosticToken = DiagnosticCreationCheck(context, ruleNames, locationToken, statements, contextParameter);
                                    if (diagnosticToken == null || diagnosticToken.Text == "")
                                    {
                                        ReportDiagnostic(context, DiagnosticIncorrectRule, statements[8].GetLocation(), ruleNames[0], locationToken.Text);
                                        return false;
                                    }

                                    if (statementCount > 9)
                                    {
                                        bool reportCorrect = DiagnosticReportCheck(context, diagnosticToken, contextParameter, statements);
                                        if (!reportCorrect)
                                        {
                                            ReportDiagnostic(context, DiagnosticReportIncorrectRule, statements[9].GetLocation(), contextParameter.Identifier.Text, diagnosticToken.Text);
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        ReportDiagnostic(context, DiagnosticReportMissingRule, statements[8].GetLocation());
                                        return false;
                                    }
                                }
                                else
                                {
                                    ReportDiagnostic(context, DiagnosticMissingRule, statements[7].GetLocation());
                                    return false;
                                }
                            }
                            else
                            {
                                ReportDiagnostic(context, LocationMissingRule, statements[6].GetLocation(), statementIdentifierToken.Text, spanToken.Text);
                                return false;
                            }
                        }
                        else
                        {
                            ReportDiagnostic(context, SpanMissingRule, statements[5].GetLocation(), startToken.Text, endToken.Text);
                            return false;
                        }
                    }
                    else
                    {
                        ReportDiagnostic(context, EndSpanMissingRule, statements[4].GetLocation(), openParenToken.Text);
                        return false;
                    }

                }
                else
                {
                    ReportDiagnostic(context, StartSpanMissingRule, statements[3].GetLocation(), keywordIdentifierToken);
                    return false;
                }

                return true;
            }

            //checks the open parenthesis variable, returns a SyntaxToken of "" if analysis failed
            internal SyntaxToken OpenParenAnalysis(CompilationAnalysisContext context, SyntaxToken statementIdentifierToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[3] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken openParenIdentifier = GetIdentifierTokenFromLocalDecl(statement);
                if (openParenIdentifier == null || openParenIdentifier.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var memberExpression = equalsValueClause.Value as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != statementIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "OpenParenToken")
                {
                    return emptyResult;
                }

                return openParenIdentifier;
            }

            //checks the start of the diagnostic span, returns a SyntaxToken of "" if analysis failed
            internal SyntaxToken StartAnalysis(CompilationAnalysisContext context, SyntaxToken keywordIdentifierToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[4] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken startToken = GetIdentifierTokenFromLocalDecl(statement);
                if (startToken == null || startToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var memberExpression = equalsValueClause.Value as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var innerMemberExpression = memberExpression.Expression as MemberAccessExpressionSyntax;
                if (innerMemberExpression == null)
                {
                    return emptyResult;
                }

                var innerIdentifier = innerMemberExpression.Expression as IdentifierNameSyntax;
                if (innerIdentifier == null || innerIdentifier.Identifier.Text != keywordIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var innerName = innerMemberExpression.Name as IdentifierNameSyntax;
                if (innerName == null || innerName.Identifier.Text != "Span")
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "Start")
                {
                    return emptyResult;
                }

                return startToken;
            }

            //checks the end of the diagnostic span, returns a SyntaxToken of "" if analysis failed
            internal SyntaxToken EndAnalysis(CompilationAnalysisContext context, SyntaxToken openParenToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[5] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken endToken = GetIdentifierTokenFromLocalDecl(statement);
                if (endToken == null || endToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var memberExpression = equalsValueClause.Value as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var innerMemberExpression = memberExpression.Expression as MemberAccessExpressionSyntax;
                if (innerMemberExpression == null)
                {
                    return emptyResult;
                }

                var innerIdentifier = innerMemberExpression.Expression as IdentifierNameSyntax;
                if (innerIdentifier == null || innerIdentifier.Identifier.Text != openParenToken.Text)
                {
                    return emptyResult;
                }

                var innerName = innerMemberExpression.Name as IdentifierNameSyntax;
                if (innerName == null || innerName.Identifier.Text != "Span")
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "Start")
                {
                    return emptyResult;
                }

                return endToken;
            }

            //checks the creation of the diagnostic span, returns a SyntaxToken of "" if analysis failed
            internal SyntaxToken SpanAnalysis(CompilationAnalysisContext context, SyntaxToken startToken, SyntaxToken endToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[6] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken spanToken = GetIdentifierTokenFromLocalDecl(statement);
                if (spanToken == null || spanToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var invocationExpression = equalsValueClause.Value as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return emptyResult;
                }

                var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != "TextSpan")
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "FromBounds")
                {
                    return emptyResult;
                }

                var argumentList = invocationExpression.ArgumentList as ArgumentListSyntax;
                if (argumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;
                if (args == null || args.Count != 2)
                {
                    return emptyResult;
                }

                var startArg = args[0] as ArgumentSyntax;
                if (startArg == null)
                {
                    return emptyResult;
                }

                var startArgIdentifier = startArg.Expression as IdentifierNameSyntax;
                if (startArgIdentifier == null || startArgIdentifier.Identifier.Text != startToken.Text)
                {
                    return emptyResult;
                }

                var endArg = args[1] as ArgumentSyntax;
                if (endArg == null)
                {
                    return emptyResult;
                }

                var endArgIdentifier = endArg.Expression as IdentifierNameSyntax;
                if (endArgIdentifier == null || endArgIdentifier.Identifier.Text != endToken.Text)
                {
                    return emptyResult;
                }

                return spanToken;
            }

            //checks the creation of the diagnostics location, returns a SyntaxToken of "" if analysis failed
            internal SyntaxToken LocationAnalysis(CompilationAnalysisContext context, SyntaxToken statementIdentifierToken, SyntaxToken spanToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[7] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken locationToken = GetIdentifierTokenFromLocalDecl(statement);
                if (locationToken == null || locationToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var invocationExpression = equalsValueClause.Value as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return emptyResult;
                }

                var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != "Location")
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "Create")
                {
                    return emptyResult;
                }

                var argumentList = invocationExpression.ArgumentList as ArgumentListSyntax;
                if (argumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;
                if (args == null || args.Count != 2)
                {
                    return emptyResult;
                }

                var treeArg = args[0] as ArgumentSyntax;
                if (treeArg == null)
                {
                    return emptyResult;
                }

                var treeArgExpression = treeArg.Expression as MemberAccessExpressionSyntax;
                if (treeArgExpression == null)
                {
                    return emptyResult;
                }

                var treeIdentifier = treeArgExpression.Expression as IdentifierNameSyntax;
                if (treeIdentifier == null || treeIdentifier.Identifier.Text != statementIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var treeName = treeArgExpression.Name as IdentifierNameSyntax;
                if (treeName == null || treeName.Identifier.Text != "SyntaxTree")
                {
                    return emptyResult;
                }

                var spanArg = args[1] as ArgumentSyntax;
                if (spanArg == null)
                {
                    return emptyResult;
                }

                var spanArgIdentifier = spanArg.Expression as IdentifierNameSyntax;
                if (spanArgIdentifier == null || spanArgIdentifier.Identifier.Text != spanToken.Text)
                {
                    return emptyResult;
                }

                return locationToken;
            }

            //checks the creation of the diagnostic itself, returns a SyntaxToken of "" if analysis failed
            internal SyntaxToken DiagnosticCreationCheck(CompilationAnalysisContext context, List<string> ruleNames, SyntaxToken locationToken, SyntaxList<StatementSyntax> statements, ParameterSyntax contextParameter)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[8] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken diagnosticToken = GetIdentifierTokenFromLocalDecl(statement);
                if (locationToken == null || locationToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var invocationExpression = equalsValueClause.Value as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return emptyResult;
                }

                var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != "Diagnostic")
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "Create")
                {
                    return emptyResult;
                }

                var argumentList = invocationExpression.ArgumentList as ArgumentListSyntax;
                if (argumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;
                if (args == null || args.Count != 3)
                {
                    return emptyResult;
                }

                var ruleArg = args[0] as ArgumentSyntax;
                if (ruleArg == null)
                {
                    return emptyResult;
                }

                var ruleArgIdentifier = ruleArg.Expression as IdentifierNameSyntax;
                if (ruleArgIdentifier == null || !ruleNames.Contains(ruleArgIdentifier.Identifier.Text))
                {
                    return emptyResult;
                }

                var locationArg = args[1] as ArgumentSyntax;
                if (locationArg == null)
                {
                    return emptyResult;
                }

                var locationArgIdentifier = locationArg.Expression as IdentifierNameSyntax;
                if (locationArgIdentifier == null || locationArgIdentifier.Identifier.Text != locationToken.Text)
                {
                    return emptyResult;
                }

                var messageArg = args[2] as ArgumentSyntax;
                if (messageArg == null)
                {
                    return emptyResult;
                }

                var messageArgExpression = messageArg.Expression as MemberAccessExpressionSyntax;
                if (messageArgExpression == null)
                {
                    return emptyResult;
                }

                var messageIdentifier = messageArgExpression.Expression as IdentifierNameSyntax;
                if (messageIdentifier == null || messageIdentifier.Identifier.Text != ruleArgIdentifier.Identifier.Text)
                {
                    return emptyResult;
                }

                var messageName = messageArgExpression.Name as IdentifierNameSyntax;
                if (messageName == null || messageName.Identifier.Text != "MessageFormat")
                {
                    return emptyResult;
                }

                return diagnosticToken;
            }

            //checks the reporting of the diagnostic, returns a bool representing whether or not analysis failed
            internal bool DiagnosticReportCheck(CompilationAnalysisContext context, SyntaxToken diagnosticToken, ParameterSyntax contextParameter, SyntaxList<StatementSyntax> statements)
            {
                var statement = statements[9] as ExpressionStatementSyntax;
                if (statement == null)
                {
                    return false;
                }

                var invocationExpression = statement.Expression as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return false;
                }

                var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return false;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != contextParameter.Identifier.Text)
                {
                    return false;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "ReportDiagnostic")
                {
                    return false;
                }

                var argumentList = invocationExpression.ArgumentList as ArgumentListSyntax;
                if (argumentList == null)
                {
                    return false;
                }

                SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;
                if (args == null || args.Count != 1)
                {
                    return false;
                }

                var diagnosticArg = args[0] as ArgumentSyntax;
                if (diagnosticArg == null)
                {
                    return false;
                }

                var diagnosticArgIdentifier = diagnosticArg.Expression as IdentifierNameSyntax;
                if (diagnosticArgIdentifier == null || diagnosticArgIdentifier.Identifier.Text != diagnosticToken.Text)
                {
                    return false;
                }

                return true;
            }
            #endregion

            //extracts the equals value clause from a local declaration statement, returns null if failed
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

            //extracts the name of the variable from a local declaration statement, returns a SyntaxToken of "" if analysis failed
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

            //returns a list containing the method declaration, and the statements within the method, returns an empty list if failed
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
                    SuppDiagReturnCheck(context, valueClause, returnDeclaration.GetLocation(), ruleNames, propertyDeclaration);
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
                    SuppDiagReturnCheck(context, valueClause, returnDeclaration.GetLocation(), ruleNames, propertyDeclaration);
                }
                else
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation, IncorrectAccessorReturnRule.MessageFormat);
                    return false;
                }

                return true;

            }

            #region CheckSupportedDiagnostics helpers
            //returns the property declaration, null if the property symbol is incorrect
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

            //returns the statements of the get accessor, empty list if get accessor not found/incorrect
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
                    ReportDiagnostic(context, MissingAccessorRule, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text);
                    return emptyResult;
                }
                if (accessors.Count > 1)
                {
                    ReportDiagnostic(context, TooManyAccessorsRule, accessorList.GetLocation(), propertyDeclaration.Identifier.Text);
                    return emptyResult;
                }

                var getAccessor = accessors.First() as AccessorDeclarationSyntax;
                if (getAccessor == null || getAccessor.Keyword.Kind() != SyntaxKind.GetKeyword)
                {
                    ReportDiagnostic(context, MissingAccessorRule, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text);
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

            //checks the return value of the get accessor within SupportedDiagnostics
            internal void SuppDiagReturnCheck(CompilationAnalysisContext context, InvocationExpressionSyntax valueClause, Location returnDeclarationLocation, List<string> ruleNames, PropertyDeclarationSyntax propertyDeclaration)
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
                    ReportDiagnostic(context, SuppDiagReturnValueRule, returnDeclarationLocation, propertyDeclaration.Identifier.Text);
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

                List<string> newRuleNames = new List<string>();
                foreach (string rule in ruleNames)
                {
                    newRuleNames.Add(rule);
                }

                foreach (ArgumentSyntax arg in valueArgs)
                {
                    if (newRuleNames.Count == 0)
                    {
                        ReportDiagnostic(context, SupportedRulesRule, valueExpression.GetLocation(), SupportedRulesRule.MessageFormat);
                        return;
                    }
                    if (newRuleNames.Contains(arg.ToString()))
                    {
                        newRuleNames.Remove(arg.ToString());
                    }
                }
            }

            //returns the valueClause of the return statement from SupportedDiagnostics and the return declaration, empty list if failed
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
                    if (fieldSymbol.Type != null && fieldSymbol.Type.MetadataName == "DiagnosticDescriptor")
                    {
                        if (fieldSymbol.DeclaredAccessibility != Accessibility.Internal || !fieldSymbol.IsStatic)
                        {
                            ReportDiagnostic(context, InternalAndStaticErrorRule, fieldSymbol.Locations[0], fieldSymbol.Name);
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
                                        List<string> severities = new List<string> { "Warning", "Error", "Hidden", "Info" };
                                        if (identifierExpr == "DiagnosticSeverity" && !severities.Contains(identifierName))
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
                            
                        //too many statements inside initialize
                        ReportDiagnostic(context, TooManyInitStatementsRule, _initializeSymbol.Locations[0], _initializeSymbol.Name.ToString());
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
                        if (arguments == null || arguments.Count == 0)
                        {
                            ReportDiagnostic(context, MissingRegisterRule, memberExpr.GetLocation(), memberExpr.Name.ToString());
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }
                        if (arguments.Count > 0)
                        {
                            IMethodSymbol actionSymbol = context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(arguments[0].Expression).Symbol as IMethodSymbol;
                            registerArgs.Add(actionSymbol);

                            if (arguments.Count > 1)
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
            //checks the signature of initialize and returns the block of the method, null if failed
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

            //checks the body of initializer, returns the invocation expression and member expression of the register statements, null if failed
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
            //stores a method in state
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
                    if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider"))
                    {
                        return;
                    }

                    if (sym.Name == "RegisterCodeFixesAsync")
                    {
                        _registerCodeFixesAsync = sym;
                        return;
                    }
                    else
                    {
                        _codeFixMethodSymbols.Add(sym);
                        return;
                    }
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

            //stores a property in state
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
                    if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider"))
                    {
                        return;
                    }

                    if (sym.Name == "FixableDiagnosticIds")
                    {
                        _codeFixFixableDiagnostics = sym;
                        return;
                    }

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

            //stores a field in state
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

            //stores a class in state
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
                        if (_otherAnalyzerClassSymbols.Contains(sym))
                        {
                            return;
                        }
                        else
                        {
                            _otherAnalyzerClassSymbols.Add(sym);
                            return;
                        }
                    }
                }
                if (sym.BaseType == context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    _analyzerClassSymbol = sym;
                }
                else if (sym.BaseType == context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider"))
                {
                    _codeFixClassSymbol = sym;
                }
                else
                {
                    return;
                }
                
            }
            #endregion

            //checks the CodeFixProvider.cs file
            internal bool CodeFixProviderAnalysis(CompilationAnalysisContext context)
            {
                bool fixableDiagnosticIdsCorrect = CheckFixableDiagnosticIds(context);
                if (!fixableDiagnosticIdsCorrect)
                {
                    return false;
                }

                List<object> registerInfo = CheckRegisterCodeFixesAsync(context);
                if (registerInfo == null)
                {
                    return false;
                }

                string branch = registerInfo[0] as string;
                if (branch == null)
                {
                    return false;
                }

                IMethodSymbol fixMethod = registerInfo[1] as IMethodSymbol;
                if (fixMethod == null)
                {
                    return false;
                }

                bool fixMethodCorrect = CheckFixMethod(context, branch, fixMethod);
                if (!fixMethodCorrect)
                {
                    return false;
                }

                return true;
            }

            //returns a bool representing whether or not the fix method is correct
            internal bool CheckFixMethod(CompilationAnalysisContext context, string branch, IMethodSymbol fixMethod)
            {
                IMethodSymbol fixSymbol = null;
                foreach (var methodSymbol in _codeFixMethodSymbols)
                {
                    if (methodSymbol.IsOverride || methodSymbol.DeclaredAccessibility != Accessibility.Private
                        || methodSymbol.Name != fixMethod.Name || methodSymbol.Parameters.Count() != fixMethod.Parameters.Count())
                    {
                        continue;
                    }
                    else
                    {
                        fixSymbol = methodSymbol;
                        break;
                    }
                }
                if (fixSymbol == null)
                {
                    // Report diagnostic missing fix method
                    return false;
                }

                if (fixSymbol.Parameters == null)
                {
                    ReportDiagnostic(context, IncorrectFixSigRule, fixSymbol.Locations[0], fixSymbol.Name.ToString());
                    return false;
                }

                if (fixSymbol.Parameters[0].Kind != fixMethod.Parameters[0].Kind)
                {
                    ReportDiagnostic(context, IncorrectFixSigRule, fixSymbol.Locations[0], fixSymbol.Name.ToString());
                    return false;
                }

                if (fixSymbol.Parameters[1].Kind != fixMethod.Parameters[1].Kind)
                {
                    ReportDiagnostic(context, IncorrectFixSigRule, fixSymbol.Locations[0], fixSymbol.Name.ToString());
                    return false;
                }

                if (fixSymbol.Parameters[2].Kind != fixMethod.Parameters[2].Kind)
                {
                    ReportDiagnostic(context, IncorrectFixSigRule, fixSymbol.Locations[0], fixSymbol.Name.ToString());
                    return false;
                }
                if (fixSymbol.ReturnType != fixMethod.ReturnType)
                {
                    ReportDiagnostic(context, IncorrectFixSigRule, fixSymbol.Locations[0], fixSymbol.Name.ToString());
                    return false;
                }

                SyntaxToken check1 = CheckIf1(fixSymbol);
                if (check1)
                {

                }
                else
                {
                    // Report diagnostic for bad generator statement
                    return false;
                }
                return true;
            }

            #region check fix helper
            internal SyntaxToken CheckIf1(IMethodSymbol fixSymbol)
            {
                SyntaxToken emptyResult = SyntaxFactory.Identifier("");
                var fixSyntax = fixSymbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                var generatorStatement = fixSyntax.Body.Statements[0] as LocalDeclarationStatementSyntax;
                if (generatorStatement == null)
                {
                    return emptyResult;
                }

                var generatorName = GetIdentifierTokenFromLocalDecl(generatorStatement);
                if (generatorName.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax statementEqualsValueClause = GetEqualsValueClauseFromLocalDecl(generatorStatement);
                if (statementEqualsValueClause == null)
                {
                    return emptyResult;
                }

                var equalsExpression = statementEqualsValueClause.Value as InvocationExpressionSyntax;
                if (equalsExpression == null)
                {
                    return emptyResult;
                }

                var expression = equalsExpression.Expression as MemberAccessExpressionSyntax;
                if (expression == null)
                {
                    return emptyResult;
                }

                var identifier = expression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.ValueText != "SyntaxGenerator")
                {
                    return emptyResult;
                }

                var methodCall = expression.Name as IdentifierNameSyntax;
                if (methodCall == null || methodCall.Identifier.ValueText != "GetGenerator")
                {
                    return emptyResult;
                }

                if (equalsExpression.ArgumentList.Arguments.Count() != 1)
                {
                    return emptyResult;
                }

                var argument = equalsExpression.ArgumentList.Arguments[0] as ArgumentSyntax;
            }
            #endregion

            //return a list with the branch that the code fix is in, and the symbol of the registered method. null if failed 
            internal List<object> CheckRegisterCodeFixesAsync(CompilationAnalysisContext context)
            {
                throw new NotImplementedException();
            }

            //returns a bool representing whether or not the FixableDiagnosticsIds property is correct
            internal bool CheckFixableDiagnosticIds(CompilationAnalysisContext context)
            {
                throw new NotImplementedException();
            }

            //clears all state
            internal void ClearState()
            {
                _analyzerClassSymbol = null;
                _analyzerFieldSymbols = new List<IFieldSymbol>();
                _analyzerMethodSymbols = new List<IMethodSymbol>();
                _analyzerPropertySymbols = new List<IPropertySymbol>();
                _otherAnalyzerClassSymbols = new List<INamedTypeSymbol>();
                _initializeSymbol = null;
                _propertySymbol = null;
                _branchesDict = new Dictionary<string, string>();
        }

            //reports a diagnostics
            public static void ReportDiagnostic(CompilationAnalysisContext context, DiagnosticDescriptor rule, Location location, params object[] messageArgs)
            {
                Diagnostic diagnostic = Diagnostic.Create(rule, location, messageArgs);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
