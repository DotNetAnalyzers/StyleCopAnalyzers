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
    /// A call to a member from an inherited class begins with <c>base.</c>, and the local class does not contain an
    /// override or implementation of the member.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a call to a member from the base class prefixed
    /// with <c>base.</c>, and there is no local implementation of the member. For example:</para>
    ///
    /// <code language="cs">
    /// string name = base.JoinName("John", "Doe");
    /// </code>
    ///
    /// <para>This rule is in place to prevent a potential source of bugs.Consider a base class which contains the
    /// following virtual method:</para>
    ///
    /// <code language="cs">
    /// public virtual string JoinName(string first, string last)
    /// {
    /// }
    /// </code>
    ///
    /// <para>Another class inherits from this base class but does not provide a local override of this method.
    /// Somewhere within this class, the base class method is called using <c>base.JoinName(...)</c>. This works as
    /// expected. At a later date, someone adds a local override of this method to the class:</para>
    ///
    /// <code language="cs">
    /// public override string JoinName(string first, string last)
    /// {
    ///   return “Bob”;
    /// }
    /// </code>
    ///
    /// <para>At this point, the local call to <c>base.JoinName(...)</c> most likely introduces a bug into the code.
    /// This call will always call the base class method and will cause the local override to be ignored.</para>
    ///
    /// <para>For this reason, calls to members from a base class should not begin with <c>base.</c>, unless a local
    /// override is implemented, and the developer wants to specifically call the base class member. When there is no
    /// local override of the base class member, the call should be prefixed with <c>this.</c> rather than
    /// <c>base.</c>.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1100";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1100Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1100MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1100Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1100.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseExpressionAction = HandleBaseExpression;

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
            context.RegisterSyntaxNodeActionHonorExclusions(BaseExpressionAction, SyntaxKind.BaseExpression);
        }

        private static void HandleBaseExpression(SyntaxNodeAnalysisContext context)
        {
            var baseExpressionSyntax = (BaseExpressionSyntax)context.Node;
            var parent = baseExpressionSyntax.Parent;
            var targetSymbol = context.SemanticModel.GetSymbolInfo(parent, context.CancellationToken);
            if (targetSymbol.Symbol == null)
            {
                return;
            }

            var memberAccessExpression = parent as MemberAccessExpressionSyntax;
            var elementAccessExpression = parent as ElementAccessExpressionSyntax;

            ExpressionSyntax speculativeExpression;

            if (memberAccessExpression != null)
            {
                // make sure to evaluate the complete invocation expression if this is a call, or overload resolution will fail
                speculativeExpression = memberAccessExpression.WithExpression(SyntaxFactory.ThisExpression());
                InvocationExpressionSyntax invocationExpression = memberAccessExpression.Parent as InvocationExpressionSyntax;
                if (invocationExpression != null)
                {
                    speculativeExpression = invocationExpression.WithExpression(speculativeExpression);
                }
            }
            else if (elementAccessExpression != null)
            {
                speculativeExpression = elementAccessExpression.WithExpression(SyntaxFactory.ThisExpression());
            }
            else
            {
                return;
            }

            var speculativeSymbol = context.SemanticModel.GetSpeculativeSymbolInfo(parent.SpanStart, speculativeExpression, SpeculativeBindingOption.BindAsExpression);
            if (speculativeSymbol.Symbol != targetSymbol.Symbol)
            {
                return;
            }

            // Do not prefix calls with base unless local implementation exists
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, baseExpressionSyntax.GetLocation()));
        }
    }
}
