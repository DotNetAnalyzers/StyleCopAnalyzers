namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;




    /// <summary>
    /// A C# statement contains a complex arithmetic expression which omits parenthesis around operators.
    /// </summary>
    /// <remarks>
    /// <para>C# maintains a hierarchy of precedence for arithmetic operators. It is possible in C# to string multiple
    /// arithmetic operations together in one statement without wrapping any of the operations in parenthesis, in which
    /// case the compiler will automatically set the order and precedence of the operations based on these
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
    /// operator precedence. The example below shows multiple arithmetic operations surrounded by parenthesis:</para>
    ///
    /// <code language="csharp">
    /// int x = 5 + (y * ((b / 6) % z)) - 2;
    /// </code>
    ///
    /// <para>Inserting parenthesis makes the code more obvious and easy to understand, and removes the need for the
    /// reader to make assumptions about the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1407ArithmeticExpressionsMustDeclarePrecedence : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1407";
        private const string Title = "Arithmetic expressions must declare precedence";
        private const string MessageFormat = "Arithmetic expressions must declare precedence";
        private const string Category = "StyleCop.CSharp.MaintainabilityRules";
        private const string Description = "A C# statement contains a complex arithmetic expression which omits parenthesis around operators.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1407.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.HandleMathExpression, SyntaxKind.AddExpression);
            context.RegisterSyntaxNodeAction(this.HandleMathExpression, SyntaxKind.SubtractExpression);
            context.RegisterSyntaxNodeAction(this.HandleMathExpression, SyntaxKind.MultiplyExpression);
            context.RegisterSyntaxNodeAction(this.HandleMathExpression, SyntaxKind.DivideExpression);
            context.RegisterSyntaxNodeAction(this.HandleMathExpression, SyntaxKind.ModuloExpression);
            context.RegisterSyntaxNodeAction(this.HandleMathExpression, SyntaxKind.LeftShiftExpression);
            context.RegisterSyntaxNodeAction(this.HandleMathExpression, SyntaxKind.RightShiftExpression);
        }

        private void HandleMathExpression(SyntaxNodeAnalysisContext context)
        {
            BinaryExpressionSyntax binSyntax = context.Node as BinaryExpressionSyntax;

            if (binSyntax != null)
            {
                if (binSyntax.Left is BinaryExpressionSyntax)
                {
                    // Check if the operations are of the same kind

                    var left = (BinaryExpressionSyntax)binSyntax.Left;

                    if (!this.IsSameFamily(binSyntax.OperatorToken, left.OperatorToken))
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, left.GetLocation()));
                }
                if (binSyntax.Right is BinaryExpressionSyntax)
                {
                    // Check if the operations are of the same kind

                    var right = (BinaryExpressionSyntax)binSyntax.Right;

                    if (!this.IsSameFamily(binSyntax.OperatorToken, right.OperatorToken))
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, right.GetLocation()));
                }
            }
        }

        private bool IsSameFamily(SyntaxToken operatorToken1, SyntaxToken operatorToken2)
        {
            bool isSameFamily = false;
            isSameFamily |= (operatorToken1.IsKind(SyntaxKind.PlusToken) || operatorToken1.IsKind(SyntaxKind.MinusToken))
                && (operatorToken2.IsKind(SyntaxKind.PlusToken) || operatorToken2.IsKind(SyntaxKind.MinusToken));
            isSameFamily |= (operatorToken1.IsKind(SyntaxKind.AsteriskToken) || operatorToken1.IsKind(SyntaxKind.SlashToken))
                && (operatorToken2.IsKind(SyntaxKind.AsteriskToken) || operatorToken2.IsKind(SyntaxKind.SlashToken));
            isSameFamily |= (operatorToken1.IsKind(SyntaxKind.LessThanLessThanToken) || operatorToken1.IsKind(SyntaxKind.GreaterThanGreaterThanToken))
                && (operatorToken2.IsKind(SyntaxKind.LessThanLessThanToken) || operatorToken2.IsKind(SyntaxKind.GreaterThanGreaterThanToken));

            return isSameFamily;
        }
    }
}