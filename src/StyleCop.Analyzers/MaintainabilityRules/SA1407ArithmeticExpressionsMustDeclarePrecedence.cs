// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// A C# statement contains a complex operator expression which omits parenthesis around operators.
    /// </summary>
    /// <remarks>
    /// <para>C# maintains a hierarchy of precedence for arithmetic, bitwise, and shift operators. It is possible in C#
    /// to string multiple operations together in one statement without wrapping any of the operations in parenthesis, in
    /// which case the compiler will automatically set the order and precedence of the operations based on these
    /// pre-established rules. For example:</para>
    ///
    /// <code language="csharp">
    /// int x = 5 + y * b / 6 % z - 2;
    /// </code>
    ///
    /// <para>Although this code is legal, it is not highly readable or maintainable. In order to achieve full
    /// understanding of this code, the developer must know and understand the basic operator precedence rules in
    /// C#.</para>
    ///
    /// <para>This rule is intended to increase the readability and maintainability of this type of code, and to reduce
    /// the risk of introducing bugs later, by forcing the developer to insert parenthesis to explicitly declare the
    /// operator precedence. The rule covers bitwise <c>&amp;</c>, <c>|</c>, and <c>^</c> operators, as well as
    /// <c>&lt;&lt;</c>, <c>&gt;&gt;</c>, and <c>&gt;&gt;&gt;</c> shift operators. The example below shows multiple
    /// arithmetic operations surrounded by parenthesis:</para>
    ///
    /// <code language="csharp">
    /// int x = 5 + (y * ((b / 6) % z)) - 2;
    /// </code>
    ///
    /// <para>Inserting parenthesis makes the code more obvious and easy to understand, and removes the need for the
    /// reader to make assumptions about the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1407ArithmeticExpressionsMustDeclarePrecedence : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1407ArithmeticExpressionsMustDeclarePrecedence"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1407";
        private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1407Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1407MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1407Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> HandledBinaryExpressionKinds =
            ImmutableArray.Create(
                SyntaxKind.AddExpression,
                SyntaxKind.SubtractExpression,
                SyntaxKind.MultiplyExpression,
                SyntaxKind.DivideExpression,
                SyntaxKind.ModuloExpression,
                SyntaxKind.BitwiseAndExpression,
                SyntaxKind.ExclusiveOrExpression,
                SyntaxKind.BitwiseOrExpression,
                SyntaxKind.LeftShiftExpression,
                SyntaxKind.RightShiftExpression,
                SyntaxKindEx.UnsignedRightShiftExpression);

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
                if (!IsSameFamily(binSyntax, left))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, left.GetLocation()));
                }
            }

            if (binSyntax.Right is BinaryExpressionSyntax right)
            {
                // Check if the operations are of the same kind
                if (!IsSameFamily(binSyntax, right))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, right.GetLocation()));
                }
            }
        }

        private static bool IsSameFamily(BinaryExpressionSyntax expression1, BinaryExpressionSyntax expression2)
        {
            SyntaxKind kind1 = expression1.Kind();
            SyntaxKind kind2 = expression2.Kind();
            bool isSameFamily = false;
            isSameFamily |= (kind1 == SyntaxKind.AddExpression || kind1 == SyntaxKind.SubtractExpression)
                && (kind2 == SyntaxKind.AddExpression || kind2 == SyntaxKind.SubtractExpression);
            isSameFamily |= (kind1 == SyntaxKind.MultiplyExpression || kind1 == SyntaxKind.DivideExpression)
                && (kind2 == SyntaxKind.MultiplyExpression || kind2 == SyntaxKind.DivideExpression);
            isSameFamily |= kind1 == SyntaxKind.BitwiseAndExpression && kind2 == SyntaxKind.BitwiseAndExpression;
            isSameFamily |= kind1 == SyntaxKind.ExclusiveOrExpression && kind2 == SyntaxKind.ExclusiveOrExpression;
            isSameFamily |= kind1 == SyntaxKind.BitwiseOrExpression && kind2 == SyntaxKind.BitwiseOrExpression;
            isSameFamily |= IsShiftExpression(kind1) && IsShiftExpression(kind2);

            return isSameFamily;
        }

        private static bool IsShiftExpression(SyntaxKind kind)
        {
            return kind == SyntaxKind.LeftShiftExpression
                || kind == SyntaxKind.RightShiftExpression
                || kind == SyntaxKindEx.UnsignedRightShiftExpression;
        }
    }
}
