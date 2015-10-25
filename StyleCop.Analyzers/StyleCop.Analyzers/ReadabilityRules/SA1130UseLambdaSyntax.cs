// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
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
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1130Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1130MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1130Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1130.md";
        private static readonly ParameterListSyntax EmptyParameterList = SyntaxFactory.ParameterList();

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.Parent.IsKind(SyntaxKind.Argument))
            {
                // invocation -> argument list -> argument -> anonymous method
                var originalInvocationExpression = anonymousMethod?.Parent?.Parent?.Parent as InvocationExpressionSyntax;

                if (originalInvocationExpression != null)
                {
                    // In some cases passing a delegate as an argument to a method is required to call the right overload
                    // When there is an other overload that takes an expression.
                    var lambdaExpression = SyntaxFactory.ParenthesizedLambdaExpression(
                        anonymousMethod.AsyncKeyword,
                        anonymousMethod.ParameterList ?? EmptyParameterList,
                        SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                        anonymousMethod.Body);

                    var invocationExpression = originalInvocationExpression.ReplaceNode(anonymousMethod, lambdaExpression);

                    SymbolInfo originalSymbolInfo = context.SemanticModel.GetSymbolInfo(originalInvocationExpression);
                    Location location = originalInvocationExpression.GetLocation();
                    SymbolInfo newSymbolInfo = context.SemanticModel.GetSpeculativeSymbolInfo(location.SourceSpan.Start, invocationExpression, SpeculativeBindingOption.BindAsExpression);

                    if (originalSymbolInfo.Symbol != newSymbolInfo.Symbol)
                    {
                        return;
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, anonymousMethod.DelegateKeyword.GetLocation()));
        }
    }
}
