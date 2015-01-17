namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A base class for <see cref="System.Diagnostics.Debug"/> diagnostics.
    /// It is used to share code in diagnostics <see cref="SA1405DebugAssertMustProvideMessageText"/> and <see cref="SA1406DebugFailMustProvideMessageText"/>
    /// </summary>
    public abstract class SystemDiagnosticsDebugDiagnosticBase : DiagnosticAnalyzer
    {
        /// <summary>
        /// Analyzes a <see cref="InvocationExpressionSyntax"/> node to add a diagnostic to static method calls in <see cref="System.Diagnostics.Debug"/>.
        /// The diagnostic is added if the parameter count is lower than <paramref name="parameterIndex"/> or the string given at the index can be evaluated to null, string.Empty or just whitespaces.
        /// </summary>
        /// <param name="context">The current analysis context</param>
        /// <param name="methodName">The method name that should be detected</param>
        /// <param name="parameterIndex">The index, the string parameter that should be checked, is at</param>
        /// <param name="descriptor">The descriptor of the diagnostic that should be added</param>
        protected internal void HandleMethodCall(SyntaxNodeAnalysisContext context, string methodName, int parameterIndex, DiagnosticDescriptor descriptor)
        {
            var invocationExpressionSyntax = context.Node as InvocationExpressionSyntax;
            if (invocationExpressionSyntax != null)
            {
                IMethodSymbol symbolInfo = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;

                if (symbolInfo != null)
                {
                    var debugType = context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(Debug).FullName);

                    if (symbolInfo.ContainingType == debugType
                        && symbolInfo.Name == methodName)
                    {
                        if (symbolInfo.Parameters.Length <= parameterIndex)
                        {
                            // Wrong overload was used, e.g. Debug.Assert(bool condition)
                            context.ReportDiagnostic(Diagnostic.Create(descriptor, invocationExpressionSyntax.GetLocation()));
                        }
                        else
                        {
                            var messageParameter = invocationExpressionSyntax.ArgumentList?.Arguments[parameterIndex];
                            if (messageParameter?.Expression != null)
                            {
                                Optional<object> constantValue = context.SemanticModel.GetConstantValue(messageParameter.Expression);
                                // Report a diagnostic if the message is constant and null or whitespace
                                if (constantValue.HasValue && string.IsNullOrWhiteSpace(constantValue.Value as string))
                                {
                                    context.ReportDiagnostic(Diagnostic.Create(descriptor, invocationExpressionSyntax.GetLocation()));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
