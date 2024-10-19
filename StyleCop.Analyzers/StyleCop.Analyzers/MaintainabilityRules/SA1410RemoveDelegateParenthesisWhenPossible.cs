// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A call to a C# anonymous method does not contain any method parameters, yet the statement still includes
    /// parenthesis.
    /// </summary>
    /// <remarks>
    /// <para>When an anonymous method does not contain any method parameters, the parenthesis around the parameters are
    /// optional.</para>
    ///
    /// <para>A violation of this rule occurs when the parenthesis are present on an anonymous method call which takes
    /// no method parameters. For example:</para>
    ///
    /// <code language="csharp">
    /// this.Method(delegate() { return 2; });
    /// </code>
    ///
    /// <para>The parenthesis are unnecessary and should be removed:</para>
    ///
    /// <code language="csharp">
    /// this.Method(delegate { return 2; });
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1410RemoveDelegateParenthesisWhenPossible : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1410RemoveDelegateParenthesisWhenPossible"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1410";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1410.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1410Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1410MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1410Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.Unnecessary);

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
            var syntax = (AnonymousMethodExpressionSyntax)context.Node;

            // ignore if no parameter list exists
            if (syntax.ParameterList == null)
            {
                return;
            }

            // ignore if parameter list is not empty
            if (syntax.ParameterList.Parameters.Count > 0)
            {
                return;
            }

            // if the delegate is passed as a parameter, verify that there is no ambiguity.
            if (syntax.Parent.IsKind(SyntaxKind.Argument))
            {
                var argumentSyntax = (ArgumentSyntax)syntax.Parent;
                var argumentListSyntax = (ArgumentListSyntax)argumentSyntax.Parent;

                switch (argumentListSyntax.Parent.Kind())
                {
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.InvocationExpression:
                    if (HasAmbiguousOverload(context, syntax, argumentListSyntax.Parent))
                    {
                        return;
                    }

                    break;
                }
            }

            // Remove delegate parenthesis when possible
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.ParameterList.GetLocation()));
        }

        private static bool HasAmbiguousOverload(SyntaxNodeAnalysisContext context, AnonymousMethodExpressionSyntax anonymousMethodExpression, SyntaxNode methodCallSyntax)
        {
            var nodeForSpeculation = methodCallSyntax.ReplaceNode(anonymousMethodExpression, anonymousMethodExpression.WithParameterList(null));
            var speculativeSymbolInfo = context.SemanticModel.GetSpeculativeSymbolInfo(methodCallSyntax.SpanStart, nodeForSpeculation, SpeculativeBindingOption.BindAsExpression);
            return speculativeSymbolInfo.Symbol == null;
        }
    }
}
