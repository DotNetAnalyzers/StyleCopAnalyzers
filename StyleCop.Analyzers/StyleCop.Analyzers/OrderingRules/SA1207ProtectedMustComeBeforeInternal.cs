// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The keyword <c>protected</c> is positioned after the keyword <c>internal</c> within the declaration of a
    /// protected internal C# element.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a protected internal element's access modifiers are written as
    /// <c>internal protected</c>. In reality, an element with the keywords <c>protected internal</c> will have the same
    /// access level as an element with the keywords <c>internal protected</c>. To make the code easier to read and more
    /// consistent, StyleCop standardizes the ordering of these keywords, so that a protected internal element will
    /// always be described as such, and never as internal protected. This can help to reduce confusion about whether
    /// these access levels are indeed the same.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1207ProtectedMustComeBeforeInternal : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1207ProtectedMustComeBeforeInternal"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1207";
        private const string Title = "Protected must come before internal";
        private const string MessageFormat = "The keyword 'protected' must come before 'internal'.";
        private const string Description = "The keyword 'protected' is positioned after the keyword 'internal' within the declaration of a protected internal C# element.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1207.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> HandledSyntaxKinds =
            ImmutableArray.Create(
                SyntaxKind.ClassDeclaration,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.StructDeclaration);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> DeclarationAction = HandleDeclaration;

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
            context.RegisterSyntaxNodeActionHonorExclusions(DeclarationAction, HandledSyntaxKinds);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
        {
            var childTokens = context.Node?.ChildTokens();
            if (childTokens == null)
            {
                return;
            }

            bool internalKeywordFound = false;
            foreach (var childToken in childTokens)
            {
                if (childToken.IsKind(SyntaxKind.InternalKeyword))
                {
                    internalKeywordFound = true;
                    continue;
                }

                if (internalKeywordFound && childToken.IsKind(SyntaxKind.ProtectedKeyword))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, childToken.GetLocation()));
                    break;
                }
            }
        }
    }
}
