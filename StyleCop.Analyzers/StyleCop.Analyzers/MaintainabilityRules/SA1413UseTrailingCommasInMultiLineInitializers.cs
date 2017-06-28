// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A multi-line initializer should use a comma on the last item.
    /// </summary>
    /// <remarks>
    /// <para>The last statement in a multi-line C# initializer is missing a trailing comma.</para>
    ///
    /// <para>A violation of this rule occurs when the last statement of a C# initializer is missing a trailing comma.
    /// For example, the following code would generate one instance of this violation:</para>
    ///
    /// <code language="csharp">
    /// var x = new Barnacle
    /// {
    ///     Age = 100,
    ///     Height = 0.2M,
    ///     Weight = 0.88M
    /// };
    /// </code>
    ///
    /// <para>The following code would not produce any violations:</para>
    ///
    /// <code language="csharp">
    /// var x = new Barnacle
    /// {
    ///     Age = 100,
    ///     Height = 0.2M,
    ///     Weight = 0.88M,
    /// };
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1413UseTrailingCommasInMultiLineInitializers : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1413UseTrailingCommasInMultiLineInitializers"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1413";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1413Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1413MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1413Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1413.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> HandleObjectInitializerAction = HandleObjectInitializer;
        private static readonly Action<SyntaxNodeAnalysisContext> HandleAnonymousObjectInitializerAction = HandleAnonymousObjectInitializer;
        private static readonly Action<SyntaxNodeAnalysisContext> HandleEnumDeclarationAction = HandleEnumDeclaration;

        private static readonly ImmutableArray<SyntaxKind> ObjectInitializerKinds =
            ImmutableArray.Create(SyntaxKind.ObjectInitializerExpression, SyntaxKind.ArrayInitializerExpression, SyntaxKind.CollectionInitializerExpression);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(HandleObjectInitializerAction, ObjectInitializerKinds);
            context.RegisterSyntaxNodeAction(HandleAnonymousObjectInitializerAction, SyntaxKind.AnonymousObjectCreationExpression);
            context.RegisterSyntaxNodeAction(HandleEnumDeclarationAction, SyntaxKind.EnumDeclaration);
        }

        private static void HandleEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var initializer = (EnumDeclarationSyntax)context.Node;
            var lastMember = initializer.Members.LastOrDefault();
            if (lastMember == null || !initializer.SpansMultipleLines())
            {
                return;
            }

            if (initializer.Members.Count != initializer.Members.SeparatorCount)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, lastMember.GetLocation()));
            }
        }

        private static void HandleObjectInitializer(SyntaxNodeAnalysisContext context)
        {
            var initializer = (InitializerExpressionSyntax)context.Node;
            if (initializer == null || !initializer.SpansMultipleLines())
            {
                return;
            }

            if (initializer.Expressions.SeparatorCount < initializer.Expressions.Count)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, initializer.Expressions.Last().GetLocation()));
            }
        }

        private static void HandleAnonymousObjectInitializer(SyntaxNodeAnalysisContext context)
        {
            var initializer = (AnonymousObjectCreationExpressionSyntax)context.Node;
            if (initializer == null || !initializer.SpansMultipleLines())
            {
                return;
            }

            if (initializer.Initializers.SeparatorCount < initializer.Initializers.Count)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, initializer.Initializers.Last().GetLocation()));
            }
        }
    }
}
