namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A C# file contains code which is unnecessary and can be removed without changing the overall logic of the code.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the file contains code which can be removed without changing the
    /// overall logic of the code.</para>
    ///
    /// <para>For example, the following try-catch statement could be removed completely since the try and catch blocks
    /// are both empty.</para>
    ///
    /// <code language="csharp">
    /// try
    /// {
    /// }
    /// catch (Exception ex)
    /// {
    /// }
    /// </code>
    ///
    /// <para>The try-finally statement below does contain code within the try block, but it does not contain any catch
    /// blocks, and the finally block is empty. Thus, the try-finally is not performing any useful function and can be
    /// removed.</para>
    ///
    /// <code language="csharp">
    /// try
    /// {
    ///     this.Method();
    /// }
    /// finally
    /// {
    /// }
    /// </code>
    ///
    /// <para>As a final example, the unsafe statement below is empty, and thus provides no value.</para>
    ///
    /// <code language="csharp">
    /// unsafe
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1409RemoveUnnecessaryCode : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1409RemoveUnnecessaryCode"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1409";
        private const string Title = "Remove unnecessary code";
        private const string MessageFormat = "Remove unnecessary code";
        private const string Category = "StyleCop.CSharp.MaintainabilityRules";
        private const string Description = "A C# file contains code which is unnecessary and can be removed without changing the overall logic of the code.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1409.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink, customTags: new[] { WellKnownDiagnosticTags.Unnecessary });

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.HandleTryStatements, SyntaxKind.TryStatement);
            context.RegisterSyntaxNodeAction(this.HandleIfStatements, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(this.HandleUnsafeStatements, SyntaxKind.UnsafeStatement);
            context.RegisterSyntaxNodeAction(this.HandleCheckedStatements, SyntaxKind.CheckedStatement);
            context.RegisterSyntaxNodeAction(this.HandleCheckedStatements, SyntaxKind.UncheckedStatement);

            context.RegisterSyntaxNodeAction(this.HandleConstructorDecleration, SyntaxKind.ConstructorDeclaration);
        }

        private void HandleTryStatements(SyntaxNodeAnalysisContext context)
        {
            TryStatementSyntax tryStatement = context.Node as TryStatementSyntax;

            // Empty try block
            if (tryStatement.Block.Statements.Count == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, tryStatement.TryKeyword.GetLocation()));
            }

            // Empty Finally Block

            if (tryStatement != null)
            {
                if (tryStatement.Catches.Count == 0)
                {
                    if (tryStatement.Finally != null && tryStatement.Finally.Block.Statements.Count == 0)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, tryStatement.Finally.FinallyKeyword.GetLocation()));
                    }
                }
            }
        }

        private void HandleIfStatements(SyntaxNodeAnalysisContext context)
        {
            IfStatementSyntax ifStatement = context.Node as IfStatementSyntax;

            if (ifStatement != null)
            {
                if (ifStatement.Else != null && !ifStatement.Else.IsMissing)
                {
                    if (ifStatement.Else.Statement.IsKind(SyntaxKind.EmptyStatement)
                        || (ifStatement.Else.Statement.IsKind(SyntaxKind.Block)
                        && ((BlockSyntax)ifStatement.Else.Statement).Statements.Count == 0))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, ifStatement.Else.ElseKeyword.GetLocation()));
                    }
                }
            }
        }

        private void HandleUnsafeStatements(SyntaxNodeAnalysisContext context)
        {
            UnsafeStatementSyntax unsafeStatement = context.Node as UnsafeStatementSyntax;

            if (unsafeStatement != null)
            {
                if (unsafeStatement.Block.Statements.Count == 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, unsafeStatement.UnsafeKeyword.GetLocation()));
                }
            }
        }

        private void HandleCheckedStatements(SyntaxNodeAnalysisContext context)
        {
            CheckedStatementSyntax checkedStatement = context.Node as CheckedStatementSyntax;

            if (checkedStatement != null)
            {
                if (checkedStatement.Block.Statements.Count == 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, checkedStatement.Keyword.GetLocation()));
                }
            }
        }

        private void HandleConstructorDecleration(SyntaxNodeAnalysisContext context)
        {
            ConstructorDeclarationSyntax constructorDecleration = context.Node as ConstructorDeclarationSyntax;

            if (constructorDecleration != null)
            {
                if (constructorDecleration.Body.Statements.Count == 0 && constructorDecleration.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, constructorDecleration.GetLocation()));
                }
            }
        }
    }
}
