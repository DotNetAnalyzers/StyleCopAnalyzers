﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
    /// A C# statement contains a complex conditional expression which omits parenthesis around operators.
    /// </summary>
    /// <remarks>
    /// <para>C# maintains a hierarchy of precedence for conditional operators. It is possible in C# to string multiple
    /// conditional operations together in one statement without wrapping any of the operations in parenthesis, in which
    /// case the compiler will automatically set the order and precedence of the operations based on these
    /// pre-established rules. For example:</para>
    ///
    /// <code language="csharp">
    /// if (x || y &amp;&amp; z &amp;&amp; a || b)
    /// {
    /// }
    /// </code>
    ///
    /// <para>Although this code is legal, it is not highly readable or maintainable. In order to achieve full
    /// understanding of this code, the developer must know and understand the basic operator precedence rules in
    /// C#.</para>
    ///
    /// <para>This rule is intended to increase the readability and maintainability of this type of code, and to reduce
    /// the risk of introducing bugs later, by forcing the developer to insert parenthesis to explicitly declare the
    /// operator precedence. For example, a developer could write this code as:</para>
    ///
    /// <code language="csharp">
    /// if ((x || y) &amp;&amp; z &amp;&amp; (a || b))
    /// {
    /// }
    /// </code>
    ///
    /// <para>or:</para>
    ///
    /// <code language="csharp">
    /// if (x || (y &amp;&amp; z &amp;&amp; a) || b)
    /// {
    /// }
    /// </code>
    ///
    /// <para>Inserting parenthesis makes the code more obvious and easy to understand, and removes the need for the
    /// reader to make assumptions about the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1408ConditionalExpressionsMustDeclarePrecedence : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1408ConditionalExpressionsMustDeclarePrecedence"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1408";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1408.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1408Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1408MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1408Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> HandledBinaryExpressionKinds =
            ImmutableArray.Create(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression);

        private static readonly Action<SyntaxNodeAnalysisContext> BinaryExpressionAction = HandleBinaryExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(BinaryExpressionAction, HandledBinaryExpressionKinds);
        }

        private static void HandleBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            BinaryExpressionSyntax binSyntax = (BinaryExpressionSyntax)context.Node;

            if (binSyntax.Left is BinaryExpressionSyntax left)
            {
                // Check if the operations are of the same kind
                if (left.OperatorToken.IsKind(SyntaxKind.AmpersandAmpersandToken) || left.OperatorToken.IsKind(SyntaxKind.BarBarToken))
                {
                    if (!IsSameFamily(binSyntax.OperatorToken, left.OperatorToken))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, left.GetLocation()));
                    }
                }
            }

            if (binSyntax.Right is BinaryExpressionSyntax right)
            {
                // Check if the operations are of the same kind
                if (right.OperatorToken.IsKind(SyntaxKind.AmpersandAmpersandToken) || right.OperatorToken.IsKind(SyntaxKind.BarBarToken))
                {
                    if (!IsSameFamily(binSyntax.OperatorToken, right.OperatorToken))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, right.GetLocation()));
                    }
                }
            }
        }

        private static bool IsSameFamily(SyntaxToken operatorToken1, SyntaxToken operatorToken2)
        {
            return (operatorToken1.IsKind(SyntaxKind.AmpersandAmpersandToken) && operatorToken2.IsKind(SyntaxKind.AmpersandAmpersandToken))
             || (operatorToken1.IsKind(SyntaxKind.BarBarToken) && operatorToken2.IsKind(SyntaxKind.BarBarToken));
        }
    }
}
