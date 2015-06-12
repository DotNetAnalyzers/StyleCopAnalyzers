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

        #region id rules
        public const string MissingId = "missingId";
        internal static DiagnosticDescriptor MissingIdRule = new DiagnosticDescriptor(
            id: MissingId,
            title: "You are missing a diagnostic id",
            messageFormat: "You are missing a diagnostic id",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region initialize rules
        public const string MissingInit = "missingInit";
        internal static DiagnosticDescriptor MissingInitRule = new DiagnosticDescriptor(
            id: MissingInit,
            title: "You are missing the required Initialize method",
            messageFormat: "You are missing the required Initialize method",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string MissingRegisterStatement = "missingRegister";
        internal static DiagnosticDescriptor MissingRegisterRule = new DiagnosticDescriptor(
            id: MissingRegisterStatement,
            title: "You need to register an action within the Initialize method",
            messageFormat: "You need to register an action within the Initialize method",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string TooManyInitStatements = "incorrectInit001";
        internal static DiagnosticDescriptor TooManyInitStatementsRule = new DiagnosticDescriptor(
            id: TooManyInitStatements,
            title: "Please only have one statement within Initiailize. You will only be registering one action.",
            messageFormat: "Please only have one statement within Initiailize. You will only be registering one action.",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectInitStatement = "incorrectInit002";
        internal static DiagnosticDescriptor IncorrectInitStatementRule = new DiagnosticDescriptor(
            id: IncorrectInitStatement,
            title: "This statement needs to register for a supported action",
            messageFormat: "This statement needs to register for a supported action",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string IncorrectInitSig = "initSignature";
        internal static DiagnosticDescriptor IncorrectInitSigRule = new DiagnosticDescriptor(
            id: IncorrectInitSig,
            title: "The signature for the Initialize method is incorrect",
            messageFormat: "The signature for the Initialize method is incorrect",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(MissingIdRule, MissingInitRule, MissingRegisterRule, TooManyInitStatementsRule, IncorrectInitStatementRule, IncorrectInitSigRule);
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
                if (_branchesDict.ContainsKey(registerSymbol.Name.ToString()))
                {
                    string kindName = null;
                    if (kind != null)
                    {
                        kindName = kind.Name.ToString();
                    }

                    if (kindName == null || allowedKinds.Contains(kindName))
                    {
                        //look for and interpret id fields
                        List<string> idNames = CheckIds(_branchesDict[registerSymbol.Name.ToString()], kindName, context);
                        if (idNames.Count > 0)
                        {
                            //look for and interpret rule fields
                            List<string> ruleNames = CheckRules(idNames, _branchesDict[registerSymbol.Name.ToString()], kindName, context);

                            if (ruleNames.Count > 0)
                            {
                                //look for and interpret SupportedDiagnostics property
                               bool supportedDiagnosticsCorrect = CheckSupportedDiagnostics(ruleNames, context);

                                if (supportedDiagnosticsCorrect)
                                {
                                    //check the SyntaxNode, Symbol, Compilation, CodeBlock, etc analysis method(s)
                                    bool analysisCorrect = CheckAnlaysis(_branchesDict[registerSymbol.Name.ToString()], kindName, ruleNames, context);
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
                                    //diagnostic
                                }
                            }
                            else
                            {
                                //diagnostic
                            }
                        }
                        else
                        {
                            // diagnostic for missing id names
                           var analyzerClassSyntax = _analyzerClassSymbol.DeclaringSyntaxReferences[0].GetSyntax() as ClassDeclarationSyntax;
                           ReportDiagnostic(context, MissingIdRule, analyzerClassSyntax.Identifier.GetLocation(), analyzerClassSyntax.Identifier.ToString());
                        }
                    }
                    else
                    {
                        ReportDiagnostic(context, IncorrectInitStatementRule, invocationExpression.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                    }
                }
                else
                {
                    //diagnostic
                }
            }

            internal bool CheckAnlaysis(string branch, string kind, List<string> ruleNames, CompilationAnalysisContext context)
            {
                throw new NotImplementedException();
            }

            internal bool CheckSupportedDiagnostics(List<string> ruleNames, CompilationAnalysisContext context)
            {
                throw new NotImplementedException();
            }
            
            //returns a list of rule names
            internal List<string> CheckRules(List<string> idNames, string branch, string kind, CompilationAnalysisContext context)
            {
                throw new NotImplementedException();

            }
            
            //returns a list of id names, empty if none found
            internal List<string> CheckIds(string branch, string kind, CompilationAnalysisContext context)
            {
                List<string> idNames = new List<string>();
                foreach (IFieldSymbol field in _analyzerFieldSymbols)
                {
                    if (field.IsConst && field.IsStatic && field.DeclaredAccessibility.ToString() == "Public" && field.Type.ToString() == "string")
                    {
                        if (field.Name == null)
                        {
                            continue;
                        }
                        idNames.Add(field.Name.ToString());
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
                    ImmutableArray<IParameterSymbol> parameters = _initializeSymbol.Parameters;
                    if (parameters.Count() != 1 || parameters[0].Type.ToString() != "Microsoft.CodeAnalysis.Diagnostics.AnalysisContext" || parameters[0].Name.ToString() != "context" || _initializeSymbol.DeclaredAccessibility.ToString() != "Public" || !_initializeSymbol.IsOverride || !_initializeSymbol.ReturnsVoid)
                    {
                        ReportDiagnostic(context, IncorrectInitSigRule, _initializeSymbol.Locations[0], MissingInitRule.MessageFormat);
                        return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                    }
                    
                    //looking at the contents of the initialize method
                    var initializeMethod = _initializeSymbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                    if (initializeMethod == null)
                    {
                        return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                    }

                    var codeBlock = initializeMethod.Body as BlockSyntax;
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
                        var statement = statements[0] as ExpressionStatementSyntax;
                        if (statement == null)
                        {
                            ReportDiagnostic(context, IncorrectInitStatementRule, statements[0].GetLocation(), IncorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }

                        var invocationExpr = statement.Expression as InvocationExpressionSyntax;
                        if (invocationExpr == null)
                        {
                            ReportDiagnostic(context, IncorrectInitStatementRule, statement.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }
                        invocExpr = invocationExpr;

                        var memberExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
                        if (memberExpr == null)
                        {
                            ReportDiagnostic(context, IncorrectInitStatementRule, invocationExpr.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }

                        var memberExprContext = memberExpr.Expression as IdentifierNameSyntax;
                        if (memberExprContext == null)
                        {
                            ReportDiagnostic(context, IncorrectInitStatementRule, memberExpr.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }
                        if (memberExprContext.Identifier.ToString() != "context")
                        {
                            ReportDiagnostic(context, IncorrectInitStatementRule, memberExprContext.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }

                        var memberExprRegister = memberExpr.Name as IdentifierNameSyntax;
                        if (memberExprRegister == null)
                        {
                            ReportDiagnostic(context, IncorrectInitStatementRule, memberExpr.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }
                        if (!_branchesDict.ContainsKey(memberExprRegister.ToString()))
                        {
                            ReportDiagnostic(context, IncorrectInitStatementRule, memberExprRegister.GetLocation(), IncorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs, invocExpr });
                        }

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
                if (sym.ContainingType.BaseType.ToString() != "Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer")
                {
                    return;
                }
                if (_analyzerMethodSymbols.Contains(sym))
                {
                    return;
                }

                if (sym.Name.ToString() == "Initialize")
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
                if (sym.ContainingType.BaseType.ToString() != "Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer")
                {
                    return;
                }
                if (_analyzerPropertySymbols.Contains(sym))
                {
                    return;
                }

                if (sym.Name.ToString() == "SupportedDiagnostics")
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
                if (sym.ContainingType.BaseType.ToString() != "Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer")
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
                if (sym.BaseType.ToString() != "Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer")
                {
                    if (sym.ContainingType == null)
                    {
                        return;
                    }
                    if (sym.ContainingType.BaseType == null)
                    {
                        return;
                    }
                    if (sym.ContainingType.BaseType.ToString() == "Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer")
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
