// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// A using directive which declares a member of the <see cref="System"/> namespace appears after a using directive
    /// which declares a member of a different namespace, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a using directive for the <see cref="System"/> namespace is placed
    /// after a non-<see cref="System"/> using directive. Placing all <see cref="System"/> using directives at the top
    /// of the using directives can make the code cleaner and easier to read, and can help make it easier to identify
    /// the namespaces that are being used by the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1208";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1208.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1208Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1208MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1208Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> BaseNamespaceDeclarationAction = HandleBaseNamespaceDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(context =>
            {
                context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
                context.RegisterSyntaxNodeAction(BaseNamespaceDeclarationAction, SyntaxKinds.BaseNamespaceDeclaration);
            });
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            if (!settings.OrderingRules.SystemUsingDirectivesFirst)
            {
                return;
            }

            var compilationUnit = (CompilationUnitSyntax)context.Node;

            var usings = compilationUnit.Usings;

            ProcessUsingsAndReportDiagnostic(usings, context);
        }

        private static void HandleBaseNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            if (!settings.OrderingRules.SystemUsingDirectivesFirst)
            {
                return;
            }

            var namespaceDeclaration = (BaseNamespaceDeclarationSyntaxWrapper)context.Node;
            var usings = namespaceDeclaration.Usings;
            ProcessUsingsAndReportDiagnostic(usings, context);
        }

        private static void ProcessUsingsAndReportDiagnostic(SyntaxList<UsingDirectiveSyntax> usings, SyntaxNodeAnalysisContext context)
        {
            string systemUsingDirectivesShouldBeBeforeThisName = null;
            for (var i = 1; i < usings.Count; i++)
            {
                var usingDirective = usings[i];

                if (usingDirective.Alias != null || !usingDirective.StaticKeyword.IsKind(SyntaxKind.None) || usingDirective.IsPrecededByPreprocessorDirective())
                {
                    continue;
                }

                if (usingDirective.IsSystemUsingDirective() && !usingDirective.HasNamespaceAliasQualifier())
                {
                    if (systemUsingDirectivesShouldBeBeforeThisName != null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, usingDirective.GetLocation(), usingDirective.Name.ToNormalizedString(), systemUsingDirectivesShouldBeBeforeThisName));
                        continue;
                    }

                    var previousUsing = usings[i - 1];
                    if (previousUsing.GlobalKeyword().IsKind(SyntaxKind.GlobalKeyword) != usingDirective.GlobalKeyword().IsKind(SyntaxKind.GlobalKeyword))
                    {
                        // Only compare usings with the same 'global' modifier
                        continue;
                    }

                    if (!previousUsing.IsSystemUsingDirective()
                        || previousUsing.HasNamespaceAliasQualifier()
                        || !previousUsing.StaticKeyword.IsKind(SyntaxKind.None))
                    {
                        systemUsingDirectivesShouldBeBeforeThisName = previousUsing.Name.ToNormalizedString();
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, usingDirective.GetLocation(), usingDirective.Name.ToNormalizedString(), systemUsingDirectivesShouldBeBeforeThisName));
                    }
                }
            }
        }
    }
}
