// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// A C# element containing opening and closing braces is written completely on a single line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when an element that is wrapped in opening and closing braces is written
    /// on a single line. For example:</para>
    /// <code language="csharp">
    /// public object Method() { return null; }
    /// </code>
    ///
    /// <para>When StyleCop checks this code, a violation of this rule will occur because the entire method is written
    /// on one line. The method should be written across multiple lines, with the opening and closing braces each on
    /// their own line, as follows:</para>
    ///
    /// <code language="csharp">
    /// public object Method()
    /// {
    ///     return null;
    /// }
    /// </code>
    ///
    /// <para>As an exception to this rule, accessors within properties, events, or indexers are allowed to be written
    /// all on a single line, as long as the accessor is short.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1502ElementMustNotBeOnASingleLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1502ElementMustNotBeOnASingleLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1502";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1502.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1502Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1502MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1502Description), LayoutResources.ResourceManager, typeof(LayoutResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> BaseTypeDeclarationAction = HandleBaseTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BasePropertyDeclarationAction = HandleBasePropertyDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseMethodDeclarationAction = HandleBaseMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> LocalFunctionStatementAction = HandleLocalFunctionStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(BaseTypeDeclarationAction, SyntaxKinds.BaseTypeDeclaration);
            context.RegisterSyntaxNodeAction(BasePropertyDeclarationAction, SyntaxKinds.BasePropertyDeclaration);
            context.RegisterSyntaxNodeAction(BaseMethodDeclarationAction, SyntaxKinds.BaseMethodDeclaration);
            context.RegisterSyntaxNodeAction(LocalFunctionStatementAction, SyntaxKindEx.LocalFunctionStatement);
            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
        }

        private static void HandleBaseTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (BaseTypeDeclarationSyntax)context.Node;
            CheckViolation(context, typeDeclaration.OpenBraceToken, typeDeclaration.CloseBraceToken);
        }

        private static void HandleBasePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var basePropertyDeclaration = (BasePropertyDeclarationSyntax)context.Node;

            // The AccessorList will be null when an expression body is present.
            if (basePropertyDeclaration.AccessorList != null)
            {
                bool isAutoProperty = basePropertyDeclaration.AccessorList.Accessors.All(accessor => accessor.Body == null);
                if (!isAutoProperty)
                {
                    CheckViolation(context, basePropertyDeclaration.AccessorList.OpenBraceToken, basePropertyDeclaration.AccessorList.CloseBraceToken);
                }
            }
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var baseMethodDeclaration = (BaseMethodDeclarationSyntax)context.Node;

            // Method declarations in interfaces will have an empty body.
            if (baseMethodDeclaration.Body != null)
            {
                CheckViolation(context, baseMethodDeclaration.Body.OpenBraceToken, baseMethodDeclaration.Body.CloseBraceToken);
            }
        }

        private static void HandleLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntaxWrapper)context.Node;

            // Expression-bodied local functions do not have a body
            if (localFunctionStatement.Body != null)
            {
                CheckViolation(context, localFunctionStatement.Body.OpenBraceToken, localFunctionStatement.Body.CloseBraceToken);
            }
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
            CheckViolation(context, namespaceDeclaration.OpenBraceToken, namespaceDeclaration.CloseBraceToken);
        }

        private static void CheckViolation(SyntaxNodeAnalysisContext context, SyntaxToken openBraceToken, SyntaxToken closeBraceToken)
        {
            if (openBraceToken.IsKind(SyntaxKind.None) || closeBraceToken.IsKind(SyntaxKind.None))
            {
                return;
            }

            var openingBraceLineSpan = openBraceToken.GetLineSpan();
            var closingBraceLineSpan = closeBraceToken.GetLineSpan();

            if (openingBraceLineSpan.EndLinePosition.Line == closingBraceLineSpan.StartLinePosition.Line)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, openBraceToken.GetLocation()));
            }
        }
    }
}
