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
//jessica
namespace MetaCompilation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MetaCompilationAnalyzer : DiagnosticAnalyzer
    {

        #region id rules
        public const string missingId = "missingId";
        internal static DiagnosticDescriptor missingIdRule = new DiagnosticDescriptor(
            id: missingId,
            title: "You are missing a diagnostic id",
            messageFormat: "You are missing a diagnostic id",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        #endregion

        #region initialize rules
        public const string missingInit = "missingInit";
        internal static DiagnosticDescriptor missingInitRule = new DiagnosticDescriptor(
            id: missingInit,
            title: "You are missing the required Initialize method",
            messageFormat: "You are missing the required Initialize method",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string missingRegisterStatement = "missingRegister";
        internal static DiagnosticDescriptor missingRegisterRule = new DiagnosticDescriptor(
            id: missingRegisterStatement,
            title: "You need to register an action within the Initialize method",
            messageFormat: "You need to register an action within the Initialize method",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string tooManyInitStatements = "incorrectInit001";
        internal static DiagnosticDescriptor tooManyInitStatementsRule = new DiagnosticDescriptor(
            id: tooManyInitStatements,
            title: "Please only have one statement within Initiailize. You will only be registering one action.",
            messageFormat: "Please only have one statement within Initiailize. You will only be registering one action.",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string incorrectInitStatement = "incorrectInit002";
        internal static DiagnosticDescriptor incorrectInitStatementRule = new DiagnosticDescriptor(
            id: incorrectInitStatement,
            title: "This statement needs to register for a supported action",
            messageFormat: "This statement needs to register for a supported action",
            category: "Syntax",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public const string incorrectInitSig = "initSignature";
        internal static DiagnosticDescriptor incorrectInitSigRule = new DiagnosticDescriptor(
            id: incorrectInitSig,
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
                return ImmutableArray.Create(missingIdRule, missingInitRule, missingRegisterRule, tooManyInitStatementsRule, incorrectInitStatementRule, incorrectInitSigRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(SetupAnalysis);
        }

        private void SetupAnalysis(CompilationStartAnalysisContext context)
        {
            CompilationAnalyzer compilationAnalyzer = new CompilationAnalyzer();

            context.RegisterSymbolAction(compilationAnalyzer.AddClass, SymbolKind.NamedType);
            context.RegisterSymbolAction(compilationAnalyzer.AddMethod, SymbolKind.Method);
            context.RegisterSymbolAction(compilationAnalyzer.AddField, SymbolKind.Field);
            context.RegisterSymbolAction(compilationAnalyzer.AddProperty, SymbolKind.Property);

            context.RegisterCompilationEndAction(compilationAnalyzer.ReportCompilationEndDiagnostics);
        }

        class CompilationAnalyzer
        {
            List<IMethodSymbol> analyzerMethodSymbols = new List<IMethodSymbol>();
            List<IPropertySymbol> analyzerPropertySymbols = new List<IPropertySymbol>();
            List<IFieldSymbol> analyzerFieldSymbols = new List<IFieldSymbol>();
            List<INamedTypeSymbol> otherClassSymbols = new List<INamedTypeSymbol>();
            IMethodSymbol initializeSymbol = null;
            IPropertySymbol propertySymbol = null; 
            INamedTypeSymbol analyzerClassSymbol = null;
            Dictionary<string, string> branchesDict = new Dictionary<string, string>();

            internal void ReportCompilationEndDiagnostics(CompilationAnalysisContext context)
            {
                //supported main branches for tutorial
                branchesDict.Add("RegisterSyntaxNodeAction", "SyntaxNode");

                //supported sub-branches for tutorial
                List<string> allowedKinds = new List<string>();
                allowedKinds.Add("IfStatement");

                if (analyzerClassSymbol == null)
                {
                    return;
                }

                //gather initialize info
                List<object> registerInfo = CheckInitialize(context);
                if (registerInfo == null) return;
               
                var registerSymbol = (IMethodSymbol)registerInfo[0];
                if (registerSymbol == null) return;
                var registerArgs = (List<ISymbol>)registerInfo[1];
                if (registerArgs == null) return;
                if (registerArgs.Count == 0) return;
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

                //interpret initialize info
                if (branchesDict.ContainsKey(registerSymbol.Name.ToString()))
                {
                    string kindName = null;
                    if (kind != null)
                    {
                        kindName = kind.Name.ToString();
                    }

                    if (kindName == null || allowedKinds.Contains(kindName))
                    {
                        //look for and interpret id fields
                        var idNames = CheckIds(branchesDict[registerSymbol.Name.ToString()], kindName, context);
                        if (idNames.Count > 0)
                        {
                            //look for and interpret rule fields
                            var ruleNames = CheckRules(idNames, branchesDict[registerSymbol.Name.ToString()], kindName, context);

                            if (ruleNames.Count > 0)
                            {
                                //look for and interpret SupportedDiagnostics property
                                var supportedDiagnosticsCorrect = CheckSupportedDiagnostics(ruleNames, context);

                                if (supportedDiagnosticsCorrect)
                                {
                                    //check the SyntaxNode, Symbol, Compilation, CodeBlock, etc analysis method(s)
                                    var analysisCorrect = CheckAnlaysis(branchesDict[registerSymbol.Name.ToString()], kindName, ruleNames, context);
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
                            var analyzerClassSyntax = analyzerClassSymbol.DeclaringSyntaxReferences[0].GetSyntax() as ClassDeclarationSyntax;
                            ReportDiagnostic(context, missingIdRule, analyzerClassSyntax.Identifier.GetLocation(), analyzerClassSyntax.Identifier.ToString());
                        }
                    }
                    else
                    {
                        ReportDiagnostic(context, incorrectInitStatementRule, registerSymbol.Locations[0], incorrectInitStatementRule.MessageFormat);
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
                foreach (IFieldSymbol field in analyzerFieldSymbols)
                {
                    if (field.IsConst && field.IsStatic && field.DeclaredAccessibility.ToString() == "public" && field.Type.ToString() == "string")
                    {
                        if (field.Name == null) continue;
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

                
                if (initializeSymbol == null)
                {
                    //the initialize method was not found
                    ReportDiagnostic(context, missingInitRule, analyzerClassSymbol.Locations[0], missingInitRule.MessageFormat);
                    return new List<object>(new object[] { registerCall, registerArgs });
                }
                else
                {
                    //checking method signature
                    var parameters = initializeSymbol.Parameters;
                    if (parameters.Count() != 1 || parameters[0].Type.ToString() != "Microsoft.CodeAnalysis.Diagnostics.AnalysisContext" || parameters[0].Name.ToString() != "context" || initializeSymbol.DeclaredAccessibility.ToString() != "Public" || !initializeSymbol.IsOverride || !initializeSymbol.ReturnsVoid)
                    {
                        ReportDiagnostic(context, incorrectInitSigRule, initializeSymbol.Locations[0], missingInitRule.MessageFormat);
                        return new List<object>(new object[] { registerCall, registerArgs });
                    }
                    
                    //looking at the contents of the initialize method
                    var initializeMethod = initializeSymbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                    if (initializeMethod == null) return new List<object>(new object[] { registerCall, registerArgs });

                    var codeBlock = initializeMethod.Body as BlockSyntax;
                    if (codeBlock == null) return new List<object>(new object[] { registerCall, registerArgs });

                    var statements = codeBlock.Statements;
                    if (statements.Count == 0)
                    {
                        //no statements inside initiailize
                        ReportDiagnostic(context, missingRegisterRule, initializeSymbol.Locations[0], missingRegisterRule.MessageFormat);
                        return new List<object>(new object[] { registerCall, registerArgs });
                    }
                    else if (statements.Count > 1)
                    {
                        //too many statements inside initialize
                        ReportDiagnostic(context, tooManyInitStatementsRule, statements[0].GetLocation(), tooManyInitStatementsRule.MessageFormat);
                        return new List<object>(new object[] { registerCall, registerArgs });
                    }
                    else
                    {
                        //only one statement inside initialize
                        var statement = statements[0] as ExpressionStatementSyntax;
                        if (statement == null) {
                            ReportDiagnostic(context, incorrectInitStatementRule, initializeMethod.GetLocation(), incorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs });
                        }

                        var invocationExpr = statement.Expression as InvocationExpressionSyntax;
                        if (invocationExpr == null)
                        {
                            ReportDiagnostic(context, incorrectInitStatementRule, statement.GetLocation(), incorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs });
                        }

                        var memberExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
                        if (memberExpr == null)
                        {
                            ReportDiagnostic(context, incorrectInitStatementRule, invocationExpr.GetLocation(), incorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs });
                        }

                        var memberExprContext = memberExpr.Expression as IdentifierNameSyntax;
                        if (memberExprContext == null)
                        {
                            ReportDiagnostic(context, incorrectInitStatementRule, memberExpr.GetLocation(), incorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs });
                        }
                        if (memberExprContext.Identifier.ToString() != "context")
                        {
                            ReportDiagnostic(context, incorrectInitStatementRule, memberExprContext.GetLocation(), incorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs });
                        }

                        var memberExprRegister = memberExpr.Name as IdentifierNameSyntax;
                        if (memberExprRegister == null)
                        {
                            ReportDiagnostic(context, incorrectInitStatementRule, memberExpr.GetLocation(), incorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs });
                        }
                        if (!branchesDict.ContainsKey(memberExprRegister.ToString()))
                        {
                            ReportDiagnostic(context, incorrectInitStatementRule, memberExprRegister.GetLocation(), incorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs });
                        }

                        if (context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(memberExpr).CandidateSymbols.Count() == 0)
                        {
                            registerCall = context.Compilation.GetSemanticModel(memberExpr.SyntaxTree).GetSymbolInfo(memberExpr).Symbol as IMethodSymbol;
                        }
                        else
                        {
                            registerCall = context.Compilation.GetSemanticModel(memberExpr.SyntaxTree).GetSymbolInfo(memberExpr).CandidateSymbols[0] as IMethodSymbol;
                        }
                        if (registerCall == null) return new List<object>(new object[] { registerCall, registerArgs });

                        var arguments = invocationExpr.ArgumentList.Arguments;
                        if (arguments == null)
                        {
                            ReportDiagnostic(context, incorrectInitStatementRule, memberExpr.GetLocation(), incorrectInitStatementRule.MessageFormat);
                            return new List<object>(new object[] { registerCall, registerArgs });
                        }
                        if (arguments.Count() > 0)
                        {
                            var actionSymbol = context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(arguments[0].Expression).Symbol as IMethodSymbol;
                            registerArgs.Add(actionSymbol);

                            if (arguments.Count() > 1)
                            {
                                var kindSymbol = context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(arguments[1].Expression).Symbol as IFieldSymbol;
                                if (kindSymbol == null)
                                {
                                    return new List<object>(new object[] { registerCall, registerArgs });
                                }
                                else
                                {
                                    registerArgs.Add(kindSymbol);
                                }
                            }
                        }

                    }
                }


                return new List<object>(new object[] { registerCall, registerArgs });
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
                if (analyzerMethodSymbols.Contains(sym))
                {
                    return;
                }

                if (sym.Name.ToString() == "Initialize")
                {
                    initializeSymbol = sym;
                    return;
                }

                analyzerMethodSymbols.Add(sym);
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
                if (analyzerPropertySymbols.Contains(sym))
                {
                    return;
                }

                if (sym.Name.ToString() == "SupportedDiagnostics")
                {
                    propertySymbol = sym;
                    return;
                }

                analyzerPropertySymbols.Add(sym);
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
                if (analyzerFieldSymbols.Contains(sym))
                {
                    return;
                }

                analyzerFieldSymbols.Add(sym);
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
                    if (sym.ContainingType == null) return;
                    if (sym.ContainingType.BaseType == null) return;
                    if (sym.ContainingType.BaseType.ToString() == "Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer")
                    {
                        if (otherClassSymbols.Contains(sym))
                        {
                            return;
                        }
                        else
                        {
                            otherClassSymbols.Add(sym);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                analyzerClassSymbol = sym;
            }
            #endregion

            internal void ClearState()
            {
                analyzerClassSymbol = null;
                analyzerFieldSymbols = new List<IFieldSymbol>();
                analyzerMethodSymbols = new List<IMethodSymbol>();
                analyzerPropertySymbols = new List<IPropertySymbol>();
            }

            public static void ReportDiagnostic(CompilationAnalysisContext context, DiagnosticDescriptor rule, Location location, params object[] messageArgs)
            {
                var diagnostic = Diagnostic.Create(rule, location, messageArgs);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
