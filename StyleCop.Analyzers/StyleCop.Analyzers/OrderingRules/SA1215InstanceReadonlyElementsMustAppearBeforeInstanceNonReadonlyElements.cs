// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// An instance readonly element is positioned beneath an instance non-readonly element of the same type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when an instance readonly element is positioned beneath an instance
    /// non-readonly element of the same type.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1215";
        private const string Title = "Instance readonly elements must appear before instance non-readonly elements";
        private const string MessageFormat = "All {0} readonly fields must appear before {0} non-readonly fields.";
        private const string Description = "An instance readonly element is positioned beneath an instance non-readonly element of the same type.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1215.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> TypeDeclarationAction = HandleTypeDeclaration;

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
            context.RegisterSyntaxNodeActionHonorExclusions(TypeDeclarationAction, TypeDeclarationKinds);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            var previousFieldReadonly = true;
            var previousAccessLevel = AccessLevel.NotSpecified;
            var previousMemberStaticOrConst = true;
            foreach (var member in typeDeclaration.Members)
            {
                var field = member as FieldDeclarationSyntax;
                if (field == null)
                {
                    continue;
                }

                var currentFieldReadonly = field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                var currentAccessLevel = AccessLevelHelper.GetAccessLevel(field.Modifiers);
                currentAccessLevel = currentAccessLevel == AccessLevel.NotSpecified ? AccessLevel.Private : currentAccessLevel;
                var currentMemberStaticOrConst = field.Modifiers.Any(SyntaxKind.StaticKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword);
                if (currentAccessLevel == previousAccessLevel
                    && !currentMemberStaticOrConst
                    && !previousMemberStaticOrConst
                    && currentFieldReadonly
                    && !previousFieldReadonly)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(field), AccessLevelHelper.GetName(currentAccessLevel)));
                }

                previousFieldReadonly = currentFieldReadonly;
                previousAccessLevel = currentAccessLevel;
                previousMemberStaticOrConst = currentMemberStaticOrConst;
            }
        }
    }
}
