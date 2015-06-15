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
using Microsoft.CodeAnalysis.Text;
namespace MetaCompilation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MetaCompilationAnalyzer : DiagnosticAnalyzer
    {
        #region rule rules

        public const string MissingIdDeclaration = "MetaAnalyzer017";
        internal static DiagnosticDescriptor MissingIdDeclarationRule = new DiagnosticDescriptor(
            id: MissingIdDeclaration,
            title: "This diagnostic id has not been declared.",
            messageFormat: "This diagnostic id has not been declared.",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string DefaultSeverityError = "MetaAnalyzer016";
        internal static DiagnosticDescriptor DefaultSeverityErrorRule = new DiagnosticDescriptor(
            id: DefaultSeverityError,
            title: "defaultSeverity must be of the form: DiagnosticSeverity.[severity].",
            messageFormat: "defaultSeverity must be of the form: DiagnosticSeverity.[severity].",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string EnabledByDefaultError = "MetaAnalyzer015";
        internal static DiagnosticDescriptor EnabledByDefaultErrorRule = new DiagnosticDescriptor(
            id: EnabledByDefaultError,
            title: "isEnabledByDefault should be set to true.",
            messageFormat: "isEnabledByDefault should be set to true.",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string InternalAndStaticError = "MetaAnalyzer014";
        internal static DiagnosticDescriptor InternalAndStaticErrorRule = new DiagnosticDescriptor(
            id: InternalAndStaticError,
            title: "The DiagnosticDescriptor should be internal and static.",
            messageFormat: "The DiagnosticDescriptor should be internal and static.",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string MissingRule = "MetaAnalyzer019";
        internal static DiagnosticDescriptor MissingRuleRule = new DiagnosticDescriptor(
            id: MissingRule,
            title: "Missing a rule",
            messageFormat: "You need to have at least one DiagnosticDescriptor rule",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region id rules
        public const string MissingId = "MetaAnalyzer001";
        internal static DiagnosticDescriptor MissingIdRule = new DiagnosticDescriptor(
            id: MissingId,
            title: "You are missing a diagnostic id",
            messageFormat: "You are missing a diagnostic id",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region Initialize rules
        public const string MissingInit = "MetaAnalyzer002";
        internal static DiagnosticDescriptor MissingInitRule = new DiagnosticDescriptor(
            id: MissingInit,
            title: "You are missing the required Initialize method",
            messageFormat: "You are missing the required Initialize method",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string MissingRegisterStatement = "MetaAnalyzer003";
        internal static DiagnosticDescriptor MissingRegisterRule = new DiagnosticDescriptor(
            id: MissingRegisterStatement,
            title: "You need to register an action within the Initialize method",
            messageFormat: "You need to register an action within the Initialize method",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string TooManyInitStatements = "MetaAnalyzer004";
        internal static DiagnosticDescriptor TooManyInitStatementsRule = new DiagnosticDescriptor(
            id: TooManyInitStatements,
            title: "Please only have one statement within Initiailize. You will only be registering one action.",
            messageFormat: "Please only have one statement within Initiailize. You will only be registering one action.",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectInitStatement = "MetaAnalyzer005";
        internal static DiagnosticDescriptor IncorrectInitStatementRule = new DiagnosticDescriptor(
            id: IncorrectInitStatement,
            title: "This statement needs to register for a supported action",
            messageFormat: "This statement needs to register for a supported action",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectInitSig = "MetaAnalyzer006";
        internal static DiagnosticDescriptor IncorrectInitSigRule = new DiagnosticDescriptor(
            id: IncorrectInitSig,
            title: "The signature for the Initialize method is incorrect",
            messageFormat: "The signature for the Initialize method is incorrect",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region SupportedDiagnostics rules
        public const string MissingSuppDiag = "MetaAnalyzer007";
        internal static DiagnosticDescriptor MissingSuppDiagRule = new DiagnosticDescriptor(
            id: MissingSuppDiag,
            title: "You are missing the required SupportedDiagnostics method",
            messageFormat: "You are missing the required SupportedDiagnostics method",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectSigSuppDiag = "MetaAnalyzer008";
        internal static DiagnosticDescriptor IncorrectSigSuppDiagRule = new DiagnosticDescriptor(
            id: IncorrectSigSuppDiag,
            title: "The signature of the SupportedDiagnostics property is incorrect",
            messageFormat: "The signature of the SupportedDiagnostics property is incorrect",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string MissingAccessor = "MetaAnalyzer009";
        internal static DiagnosticDescriptor MissingAccessorRule = new DiagnosticDescriptor(
            id: MissingAccessor,
            title: "You are missing a get accessor in your SupportedDiagnostics property",
            messageFormat: "You are missing a get accessor in your SupportedDiagnostics property",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string TooManyAccessors = "MetaAnalyzer010";
        internal static DiagnosticDescriptor TooManyAccessorsRule = new DiagnosticDescriptor(
            id: TooManyAccessors,
            title: "You only need a get accessor for this property",
            messageFormat: "You only need a get accessor for this property",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectAccessorReturn = "MetaAnalyzer011";
        internal static DiagnosticDescriptor IncorrectAccessorReturnRule = new DiagnosticDescriptor(
            id: IncorrectAccessorReturn,
            title: "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules",
            messageFormat: "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string SuppDiagReturnValue = "MetaAnalyzer012";
        internal static DiagnosticDescriptor SuppDiagReturnValueRule = new DiagnosticDescriptor(
            id: SuppDiagReturnValue,
            title: "You need to create an immutable array",
            messageFormat: "You need to create an immutable array",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string SupportedRules = "MetaAnalyzer013";
        internal static DiagnosticDescriptor SupportedRulesRule = new DiagnosticDescriptor(
            id: SupportedRules,
            title: "The immutable array should contain every DiagnosticDescriptor rule that was created",
            messageFormat: "The immutable array should contain every DiagnosticDescriptor rule that was created",
            category: "Syntax",
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
                                             InternalAndStaticErrorRule);
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
                if (registerArgs.Count > 0)
                {
                    if (registerArgs[0] != null)
                    {
                        var analysisMethodSymbol = (IMethodSymbol)registerArgs[0];
                    }
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
                                    bool analysisCorrect = CheckAnalysis(_branchesDict[registerSymbol.Name], kindName, ruleNames, context);
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

            internal bool CheckAnalysis(string branch, string kind, List<string> ruleNames, CompilationAnalysisContext context)
            {
                throw new NotImplementedException();
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
                    !_propertySymbol.IsOverride || _propertySymbol.OverriddenProperty.Name != "Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer.SupportedDiagnostics")
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

                foreach (var fieldSymbol in _analyzerFieldSymbols)
                {
                    if (fieldSymbol.Type.MetadataName == "DiagnosticDescriptor")
                    {
                        if (fieldSymbol.DeclaredAccessibility != Accessibility.Internal || !fieldSymbol.IsStatic)
                        {
                            ReportDiagnostic(context, InternalAndStaticErrorRule, fieldSymbol.Locations[0], InternalAndStaticErrorRule.MessageFormat);
                            return ruleNames;
                        }

                        var declaratorSyntax = fieldSymbol.DeclaringSyntaxReferences[0].GetSyntax() as VariableDeclaratorSyntax;
                        var objectCreationSyntax = declaratorSyntax.Initializer.Value as ObjectCreationExpressionSyntax;
                        var ruleArgumentList = objectCreationSyntax.ArgumentList;

                        for (int i = 0; i < ruleArgumentList.Arguments.Count; i++)
                        {
                            var currentArg = ruleArgumentList.Arguments[i];
                            string currentArgName = currentArg.NameColon.Name.Identifier.Text;

                            if (currentArgName == "isEnabledByDefault" && currentArg.Expression.ToString() != "true")
                            {
                                ReportDiagnostic(context, EnabledByDefaultErrorRule, currentArg.Expression.GetLocation(), EnabledByDefaultErrorRule.MessageFormat);
                                return ruleNames;
                            }
                            else if (currentArgName == "defaultSeverity")
                            {
                                var memberAccessExpr = currentArg.Expression as MemberAccessExpressionSyntax;
                                string identifierExpr = memberAccessExpr.Expression.ToString();
                                string identifierName = memberAccessExpr.Name.Identifier.Text;

                                if (identifierExpr != "DiagnosticSeverity" && (identifierName != "Warning" || identifierName != "Error" || identifierName != "Hidden" || identifierName != "Info"))
                                {
                                    ReportDiagnostic(context, DefaultSeverityErrorRule, currentArg.Expression.GetLocation(), DefaultSeverityErrorRule.MessageFormat);
                                    return ruleNames;
                                }
                            }
                            else if (currentArgName == "id")
                            {
                                var foundId = currentArg.Expression.ToString();
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
                                    ReportDiagnostic(context, MissingIdDeclarationRule, currentArg.Expression.GetLocation(), MissingIdDeclarationRule.MessageFormat);
                                    List<string> emptyRuleNames = new List<string>();
                                    return emptyRuleNames;
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
