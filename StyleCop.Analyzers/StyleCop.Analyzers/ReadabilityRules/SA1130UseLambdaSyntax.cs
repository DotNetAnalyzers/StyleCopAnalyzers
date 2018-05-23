// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An anonymous method was declared using the form <c>delegate (parameters) { }</c>, when a lambda expression would
    /// provide equivalent behavior with the syntax <c>(parameters) => { }</c>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1130UseLambdaSyntax : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1130UseLambdaSyntax"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1130";

        /// <summary>
        /// Property identifier used to pass information to the codefix.
        /// </summary>
        internal const string DelegateArgumentNamesProperty = "DelegateArgumentNames";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1130Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1130MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1130Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1130.md";
        private static readonly ParameterListSyntax EmptyParameterList = SyntaxFactory.ParameterList();

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var diagnosticProperties = ImmutableDictionary.CreateBuilder<string, string>();

            bool reportDiagnostic = true;
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            switch (anonymousMethod.Parent.Kind())
            {
            case SyntaxKind.Argument:
                reportDiagnostic = HandleMethodInvocation(context.SemanticModel, anonymousMethod, (ArgumentSyntax)anonymousMethod.Parent, diagnosticProperties);
                break;

            case SyntaxKind.EqualsValueClause:
                reportDiagnostic = HandleAssignment(context.SemanticModel, (EqualsValueClauseSyntax)anonymousMethod.Parent, diagnosticProperties);
                break;

            case SyntaxKind.AddAssignmentExpression:
            case SyntaxKind.SubtractAssignmentExpression:
                reportDiagnostic = HandleAssignmentExpression(context.SemanticModel, anonymousMethod, (AssignmentExpressionSyntax)anonymousMethod.Parent, diagnosticProperties);
                break;
            }

            if (reportDiagnostic)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, anonymousMethod.DelegateKeyword.GetLocation(), diagnosticProperties.ToImmutable()));
            }
        }

        private static bool HandleMethodInvocation(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod, ArgumentSyntax argumentSyntax, ImmutableDictionary<string, string>.Builder propertiesBuilder)
        {
            // invocation -> argument list -> argument -> anonymous method
            var argumentListSyntax = argumentSyntax?.Parent as ArgumentListSyntax;

            var originalInvocationExpression = argumentListSyntax?.Parent as InvocationExpressionSyntax;
            if (originalInvocationExpression != null)
            {
                SymbolInfo originalSymbolInfo = semanticModel.GetSymbolInfo(originalInvocationExpression);
                Location location = originalInvocationExpression.GetLocation();

                var argumentIndex = argumentListSyntax.Arguments.IndexOf(argumentSyntax);
                var parameterList = GetDelegateParameterList(originalSymbolInfo, argumentIndex);

                // In some cases passing a delegate as an argument to a method is required to call the right overload
                // When there is an other overload that takes an expression.
                var lambdaExpression = SyntaxFactory.ParenthesizedLambdaExpression(
                    anonymousMethod.AsyncKeyword,
                    parameterList,
                    SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                    anonymousMethod.Body);

                var invocationExpression = originalInvocationExpression.ReplaceNode(anonymousMethod, lambdaExpression);
                SymbolInfo newSymbolInfo = semanticModel.GetSpeculativeSymbolInfo(location.SourceSpan.Start, invocationExpression, SpeculativeBindingOption.BindAsExpression);

                if (originalSymbolInfo.Symbol != newSymbolInfo.Symbol)
                {
                    return false;
                }

                var parameterNames = parameterList.Parameters.Select(p => p.Identifier.ToString());
                propertiesBuilder.Add(DelegateArgumentNamesProperty, string.Join(",", parameterNames));
            }

            return true;
        }

        private static bool HandleAssignment(SemanticModel semanticModel, EqualsValueClauseSyntax equalsValueClauseSyntax, ImmutableDictionary<string, string>.Builder propertiesBuilder)
        {
            var variableDeclaration = (VariableDeclarationSyntax)equalsValueClauseSyntax.Parent.Parent;
            var symbol = semanticModel.GetSymbolInfo(variableDeclaration.Type);

            var namedTypeSymbol = symbol.Symbol as INamedTypeSymbol;
            if (namedTypeSymbol?.TypeKind == TypeKind.Delegate)
            {
                var delegateParameters = namedTypeSymbol.DelegateInvokeMethod.Parameters;
                propertiesBuilder.Add(DelegateArgumentNamesProperty, string.Join(",", delegateParameters.Select(ps => ps.Name)));
                return true;
            }

            return false;
        }

        private static bool HandleAssignmentExpression(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod, AssignmentExpressionSyntax assignmentExpressionSyntax, ImmutableDictionary<string, string>.Builder propertiesBuilder)
        {
            var symbol = semanticModel.GetSymbolInfo(assignmentExpressionSyntax.Left);

            var eventSymbol = symbol.Symbol as IEventSymbol;
            if (eventSymbol?.Type.TypeKind == TypeKind.Delegate)
            {
                var delegateParameters = ((INamedTypeSymbol)eventSymbol.Type).DelegateInvokeMethod.Parameters;
                propertiesBuilder.Add(DelegateArgumentNamesProperty, string.Join(",", delegateParameters.Select(ps => ps.Name)));
                return true;
            }

            return false;
        }

        private static ParameterListSyntax GetDelegateParameterList(SymbolInfo originalSymbolInfo, int argumentIndex)
        {
            // Determine the parameter list from the method that is invoked, as delegates without parameters are allowed, but they cannot be replaced by a lambda without parameters.
            var methodSymbol = (IMethodSymbol)originalSymbolInfo.Symbol;
            var delegateType = (INamedTypeSymbol)methodSymbol.Parameters[argumentIndex].Type;
            var delegateParameters = delegateType.DelegateInvokeMethod.Parameters;

            var syntaxParameters = GetSyntaxParametersFromSymbolParameters(delegateParameters);

            return SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(syntaxParameters));
        }

        private static ImmutableArray<ParameterSyntax> GetSyntaxParametersFromSymbolParameters(ImmutableArray<IParameterSymbol> symbolParameters)
        {
            var result = ImmutableArray.CreateBuilder<ParameterSyntax>(symbolParameters.Length);

            foreach (var symbolParameter in symbolParameters)
            {
                var syntaxParameter = GetParameterSyntaxFromParameterSymbol(symbolParameter);
                result.Add(syntaxParameter);
            }

            return result.ToImmutable();
        }

        private static ParameterSyntax GetParameterSyntaxFromParameterSymbol(IParameterSymbol symbolParameter)
        {
            return SyntaxFactory.Parameter(SyntaxFactory.Identifier(symbolParameter.Name))
                .WithType(SyntaxFactory.ParseTypeName(symbolParameter.Type.Name));
        }
    }
}
