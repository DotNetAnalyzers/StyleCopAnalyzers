// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using SpacingRules;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or
    /// indexer, is not placed on the same line as the opening bracket when the element does not take any parameters.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a method or indexer does not take any parameters and the closing
    /// bracket of a call or declaration for the method or indexer is not placed on the same line as the opening
    /// bracket. The following example shows correct placement of the closing parenthesis:</para>
    /// <code language="csharp">
    /// public string GetName()
    /// {
    ///     return this.name.Trim();
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1112";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1112Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1112MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1112Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1112.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConstructorDeclarationAction = HandleConstructorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;

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
            context.RegisterSyntaxNodeActionHonorExclusions(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ConstructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(InvocationExpressionAction, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ObjectCreationExpressionAction, SyntaxKind.ObjectCreationExpression);
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
            if (objectCreation.ArgumentList == null ||
                objectCreation.ArgumentList.IsMissing ||
                objectCreation.ArgumentList.Arguments.Count > 0)
            {
                return;
            }

            if (!objectCreation.ArgumentList.OpenParenToken.IsMissing &&
                    !objectCreation.ArgumentList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfOpenAndCloseTokensAreTheSame(
                    context,
                    objectCreation.ArgumentList.OpenParenToken,
                    objectCreation.ArgumentList.CloseParenToken);
            }
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            if (invocationExpression.ArgumentList == null ||
                invocationExpression.ArgumentList.IsMissing ||
                invocationExpression.ArgumentList.Arguments.Any())
            {
                return;
            }

            if (!invocationExpression.ArgumentList.OpenParenToken.IsMissing &&
                !invocationExpression.ArgumentList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfOpenAndCloseTokensAreTheSame(
                    context,
                    invocationExpression.ArgumentList.OpenParenToken,
                    invocationExpression.ArgumentList.CloseParenToken);
            }
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructotDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;
            HandleBaseMethodDeclaration(context, constructotDeclarationSyntax);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            HandleBaseMethodDeclaration(context, methodDeclaration);
        }

        private static void HandleBaseMethodDeclaration(
            SyntaxNodeAnalysisContext context,
            BaseMethodDeclarationSyntax baseMethodDeclarationSyntax)
        {
            var parameterListSyntax =
                baseMethodDeclarationSyntax.ParameterList;

            if (parameterListSyntax != null && !parameterListSyntax.Parameters.Any())
            {
                if (!parameterListSyntax.OpenParenToken.IsMissing &&
                    !parameterListSyntax.CloseParenToken.IsMissing)
                {
                    CheckIfLocationOfOpenAndCloseTokensAreTheSame(context, parameterListSyntax.OpenParenToken, parameterListSyntax.CloseParenToken);
                }
            }
        }

        private static void CheckIfLocationOfOpenAndCloseTokensAreTheSame(
            SyntaxNodeAnalysisContext context,
            SyntaxToken openToken,
            SyntaxToken closeToken)
        {
            var closeParenLine = closeToken.GetLineSpan();
            var openParenLine = openToken.GetLineSpan();
            if (closeParenLine.IsValid &&
                openParenLine.IsValid &&
                openParenLine.StartLinePosition.Line != closeParenLine.StartLinePosition.Line)
            {
                var properties = TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, closeToken.GetLocation(), properties));
            }
        }
    }
}
