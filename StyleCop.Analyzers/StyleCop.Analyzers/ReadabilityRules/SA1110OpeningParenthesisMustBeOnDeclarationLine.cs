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
    using SpacingRules;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or
    /// indexer, is not placed on the same line as the method or indexer name.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening bracket of a method or indexer call or declaration is not
    /// placed on the same line as the method or indexer. The following examples show correct placement of the opening
    /// bracket:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string last)
    /// {
    ///     return JoinStrings(
    ///         first, last);
    /// }
    ///
    /// public int this[int x]
    /// {
    ///     get { return this.items[x]; }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1110OpeningParenthesisMustBeOnDeclarationLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1110OpeningParenthesisMustBeOnDeclarationLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1110";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1110Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1110MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1110Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1110.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConstructorDeclarationAction = HandleConstructorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ElementAccessExpressionAction = HandleElementAccessExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeAction = HandleAttribute;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ArrayCreationExpressionAction = HandleArrayCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> OperatorDeclarationAction = HandleOperatorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConversionOperatorDeclarationAction = HandleConversionOperatorDeclaration;

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
            context.RegisterSyntaxNodeActionHonorExclusions(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ElementAccessExpressionAction, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(AttributeAction, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ArrayCreationExpressionAction, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(OperatorDeclarationAction, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ConversionOperatorDeclarationAction, SyntaxKind.ConversionOperatorDeclaration);
        }

        private static void HandleConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperator = (ConversionOperatorDeclarationSyntax)context.Node;

            var identifierName = conversionOperator.ChildNodes()
                .OfType<IdentifierNameSyntax>()
                .FirstOrDefault();
            if (identifierName == null || identifierName.Identifier.IsMissing)
            {
                return;
            }

            var parameterListSyntax = conversionOperator.ParameterList;

            if (parameterListSyntax != null && !parameterListSyntax.OpenParenToken.IsMissing)
            {
                bool preserveLayout = parameterListSyntax.Parameters.Any();
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, parameterListSyntax.OpenParenToken, preserveLayout);
            }
        }

        private static void HandleOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            if (operatorDeclaration.OperatorToken.IsMissing)
            {
                return;
            }

            var parameterListSyntax = operatorDeclaration.ParameterList;

            if (parameterListSyntax != null && !parameterListSyntax.OpenParenToken.IsMissing)
            {
                bool preserveLayout = parameterListSyntax.Parameters.Any();
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, parameterListSyntax.OpenParenToken, preserveLayout);
            }
        }

        private static void HandleArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var array = (ArrayCreationExpressionSyntax)context.Node;

            if (array.Type.IsMissing ||
                array.Type.ElementType == null ||
                !array.Type.RankSpecifiers.Any())
            {
                return;
            }

            var firstSize = array.Type.RankSpecifiers.First();

            if (!firstSize.OpenBracketToken.IsMissing)
            {
                bool preserveLayout = firstSize.Sizes.Any();
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, firstSize.OpenBracketToken, preserveLayout);
            }
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.DelegateKeyword.IsMissing ||
                anonymousMethod.ParameterList == null ||
                anonymousMethod.ParameterList.IsMissing ||
                anonymousMethod.ParameterList.OpenParenToken.IsMissing)
            {
                return;
            }

            bool preserveLayout = anonymousMethod.ParameterList.Parameters.Any();
            CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, anonymousMethod.ParameterList.OpenParenToken, preserveLayout);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            if (!delegateDeclaration.Identifier.IsMissing &&
                delegateDeclaration.ParameterList != null &&
                !delegateDeclaration.ParameterList.OpenParenToken.IsMissing)
            {
                bool preserveLayout = delegateDeclaration.ParameterList.Parameters.Any();
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, delegateDeclaration.ParameterList.OpenParenToken, preserveLayout);
            }
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;

            var qualifiedNameSyntax = attribute.Name as QualifiedNameSyntax;
            IdentifierNameSyntax identifierNameSyntax = null;
            if (qualifiedNameSyntax != null)
            {
                identifierNameSyntax = qualifiedNameSyntax.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            }
            else
            {
                identifierNameSyntax = attribute.Name as IdentifierNameSyntax;
            }

            if (identifierNameSyntax != null)
            {
                if (attribute.ArgumentList != null &&
                    !attribute.ArgumentList.OpenParenToken.IsMissing &&
                    !identifierNameSyntax.Identifier.IsMissing)
                {
                    bool preserveLayout = attribute.ArgumentList.Arguments.Any();
                    CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, attribute.ArgumentList.OpenParenToken, preserveLayout);
                }
            }
        }

        private static void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            if (elementAccess.Expression == null ||
                elementAccess.ArgumentList.IsMissing ||
                elementAccess.ArgumentList.OpenBracketToken.IsMissing)
            {
                return;
            }

            bool preserveLayout = elementAccess.ArgumentList.Arguments.Any();
            CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, elementAccess.ArgumentList.OpenBracketToken, preserveLayout);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext obj)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)obj.Node;

            if (!indexerDeclaration.ThisKeyword.IsMissing &&
                indexerDeclaration.ParameterList != null &&
                !indexerDeclaration.ParameterList.IsMissing &&
                !indexerDeclaration.ParameterList.OpenBracketToken.IsMissing)
            {
                bool preserveLayout = indexerDeclaration.ParameterList.Parameters.Any();
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(obj, indexerDeclaration.ParameterList.OpenBracketToken, preserveLayout);
            }
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
            var identifier = GetIdentifier(objectCreation);

            if (!identifier.HasValue
                || identifier.Value.IsMissing)
            {
                return;
            }

            if (objectCreation.ArgumentList != null
                && !objectCreation.ArgumentList.OpenParenToken.IsMissing)
            {
                bool preserveLayout = objectCreation.ArgumentList.Arguments.Any();
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, objectCreation.ArgumentList.OpenParenToken, preserveLayout);
            }
        }

        private static SyntaxToken? GetIdentifier(ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
        {
            switch (objectCreationExpressionSyntax.Type.Kind())
            {
            case SyntaxKind.QualifiedName:
                var qualifiedNameSyntax = (QualifiedNameSyntax)objectCreationExpressionSyntax.Type;
                var identifierNameSyntax = qualifiedNameSyntax.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                return identifierNameSyntax?.Identifier;

            case SyntaxKind.IdentifierName:
                return ((IdentifierNameSyntax)objectCreationExpressionSyntax.Type).Identifier;

            case SyntaxKind.GenericName:
                return ((GenericNameSyntax)objectCreationExpressionSyntax.Type).Identifier;

            default:
                return null;
            }
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var identifierNameSyntax = invocationExpression.Expression as IdentifierNameSyntax ??
                                                        invocationExpression.Expression.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault();

            if (identifierNameSyntax != null)
            {
                if (invocationExpression.ArgumentList != null &&
                    !invocationExpression.ArgumentList.OpenParenToken.IsMissing &&
                    !identifierNameSyntax.Identifier.IsMissing)
                {
                    bool preserveLayout = invocationExpression.ArgumentList.Arguments.Any();
                    CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, invocationExpression.ArgumentList.OpenParenToken, preserveLayout);
                }
            }
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructotDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;
            if (constructotDeclarationSyntax.ParameterList != null
                && !constructotDeclarationSyntax.ParameterList.OpenParenToken.IsMissing
                && !constructotDeclarationSyntax.Identifier.IsMissing)
            {
                bool preserveLayout = constructotDeclarationSyntax.ParameterList.Parameters.Any();
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, constructotDeclarationSyntax.ParameterList.OpenParenToken, preserveLayout);
            }
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            if (methodDeclaration.ParameterList != null
                && !methodDeclaration.ParameterList.OpenParenToken.IsMissing
                && !methodDeclaration.Identifier.IsMissing)
            {
                bool preserveLayout = methodDeclaration.ParameterList.Parameters.Any();
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, methodDeclaration.ParameterList.OpenParenToken, preserveLayout);
            }
        }

        private static void CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(SyntaxNodeAnalysisContext context, SyntaxToken openToken, bool preserveLayout)
        {
            var previousToken = openToken.GetPreviousToken();
            if (previousToken.IsMissing)
            {
                return;
            }

            var prevTokenLine = previousToken.GetLineSpan();
            var openParenLine = openToken.GetLineSpan();
            if (prevTokenLine.IsValid &&
                openParenLine.IsValid &&
                openParenLine.StartLinePosition.Line != prevTokenLine.StartLinePosition.Line)
            {
                var properties = preserveLayout ? TokenSpacingProperties.RemovePrecedingPreserveLayout : TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, openToken.GetLocation(), properties));
            }
        }
    }
}
