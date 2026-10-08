// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// The C# code includes an empty string, written as <c>""</c>.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains an empty string. For example:</para>
    ///
    /// <code language="csharp">
    /// string s = "";
    /// </code>
    ///
    /// <para>This will cause the compiler to embed an empty string into the compiled code. Rather than including a
    /// hard-coded empty string, use the static <see cref="string.Empty"/> field:</para>
    ///
    /// <code language="csharp">
    /// string s = string.Empty;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1122UseStringEmptyForEmptyStrings : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1122UseStringEmptyForEmptyStrings"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1122";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1122.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1122Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1122MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1122Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> StringLiteralExpressionAction = HandleStringLiteralExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> InterpolatedStringExpressionAction = HandleInterpolatedStringExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(StringLiteralExpressionAction, SyntaxKind.StringLiteralExpression);
            context.RegisterSyntaxNodeAction(InterpolatedStringExpressionAction, SyntaxKind.InterpolatedStringExpression);
        }

        private static void HandleStringLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            LiteralExpressionSyntax literalExpression = (LiteralExpressionSyntax)context.Node;
            var token = literalExpression.Token;

            // The token kind check is needed: a UTF-8 string literal such as ""u8 is also a string literal expression
            // with an empty value, but it is a ReadOnlySpan<byte> that string.Empty can't replace. A single-line raw
            // string literal can never be empty, so only the multi-line form needs to be checked.
            if (!token.IsKind(SyntaxKind.StringLiteralToken) && !token.IsKind(SyntaxKindEx.MultiLineRawStringLiteralToken))
            {
                return;
            }

            // Check the value first, because it is much cheaper than walking up the tree in HasToBeConstant.
            if (token.ValueText != string.Empty)
            {
                return;
            }

            if (HasToBeConstant(literalExpression))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, literalExpression.GetLocation()));
        }

        private static void HandleInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
        {
            var interpolatedStringExpression = (InterpolatedStringExpressionSyntax)context.Node;

            // Only an interpolated string without any content at all is considered empty
            if (interpolatedStringExpression.Contents.Count > 0)
            {
                return;
            }

            if (HasToBeConstant(interpolatedStringExpression))
            {
                return;
            }

            if (!CanBeReplacedByString(context, interpolatedStringExpression))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, interpolatedStringExpression.GetLocation()));
        }

        /// <summary>
        /// Determines whether a string expression such as <c>string.Empty</c> can take the place of an interpolated
        /// string. Unlike a string literal, an interpolated string can also be converted to
        /// <c>FormattableString</c>, <see cref="IFormattable"/> or an interpolated string handler
        /// type, and a string can't be converted to any of those.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <param name="interpolatedStringExpression">The interpolated string expression.</param>
        /// <returns><see langword="true"/> if a string can replace the interpolated string; otherwise, <see langword="false"/>.</returns>
        private static bool CanBeReplacedByString(SyntaxNodeAnalysisContext context, InterpolatedStringExpressionSyntax interpolatedStringExpression)
        {
            var convertedType = context.SemanticModel.GetTypeInfo(interpolatedStringExpression, context.CancellationToken).ConvertedType;
            if (convertedType == null
                || convertedType.TypeKind == TypeKind.Error
                || convertedType.SpecialType == SpecialType.System_String)
            {
                return true;
            }

            var compilation = (CSharpCompilation)context.SemanticModel.Compilation;
            var conversion = compilation.ClassifyConversion(compilation.GetSpecialType(SpecialType.System_String), convertedType);
            return conversion.Exists && conversion.IsImplicit;
        }

        private static bool HasToBeConstant(ExpressionSyntax expression)
        {
            ExpressionSyntax outermostExpression = FindOutermostExpression(expression);

            if (outermostExpression.Parent.IsKind(SyntaxKind.AttributeArgument)
                || outermostExpression.Parent.IsKind(SyntaxKind.CaseSwitchLabel)
                || outermostExpression.Parent.IsKind(SyntaxKindEx.ConstantPattern))
            {
                return true;
            }

            if (outermostExpression.Parent is EqualsValueClauseSyntax equalsValueClause)
            {
                if (equalsValueClause.Parent is ParameterSyntax)
                {
                    return true;
                }

                if (!(equalsValueClause.Parent is VariableDeclaratorSyntax variableDeclaratorSyntax) || !(variableDeclaratorSyntax?.Parent is VariableDeclarationSyntax variableDeclarationSyntax))
                {
                    return false;
                }

                if (variableDeclarationSyntax.Parent is FieldDeclarationSyntax fieldDeclarationSyntax
                    && fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    return true;
                }

                if (variableDeclarationSyntax.Parent is LocalDeclarationStatementSyntax localDeclarationStatementSyntax
                    && localDeclarationStatementSyntax.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    return true;
                }
            }

            return false;
        }

        private static ExpressionSyntax FindOutermostExpression(ExpressionSyntax node)
        {
            while (true)
            {
                if (!(node.Parent is ExpressionSyntax parent))
                {
                    break;
                }

                node = parent;
            }

            return node;
        }
    }
}
