// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Settings.ObjectModel;

    /// <summary>
    /// A constant field is placed beneath a non-constant field.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a constant field is placed beneath a non-constant field. Constants
    /// must be placed above fields to indicate that the two are fundamentally different types of elements with
    /// different considerations for the compiler, different naming requirements, etc.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1203ConstantsMustAppearBeforeFields : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1203ConstantsMustAppearBeforeFields"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1203";
        private const string Title = "Constants must appear before fields";
        private const string MessageFormat = "Constant fields must appear before non-constant fields";
        private const string Description = "A constant field is placed beneath a non-constant field.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1203.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TypeDeclarationAction = HandleTypeDeclaration;

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

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int constantIndex = elementOrder.IndexOf(OrderingTrait.Constant);
            if (constantIndex < 0)
            {
                return;
            }

            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            var members = typeDeclaration.Members;
            var previousFieldConstant = true;
            var previousFieldStatic = false;
            var previousFieldReadonly = false;
            var previousAccessLevel = AccessLevel.NotSpecified;

            foreach (var member in members)
            {
                var field = member as FieldDeclarationSyntax;
                if (field == null)
                {
                    continue;
                }

                AccessLevel currentAccessLevel = MemberOrderHelper.GetAccessLevelForOrdering(field, field.Modifiers);
                bool currentFieldReadonly = field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                bool currentFieldStatic = field.Modifiers.Any(SyntaxKind.StaticKeyword);
                bool compareConst = true;
                for (int j = 0; compareConst && j < constantIndex; j++)
                {
                    switch (elementOrder[j])
                    {
                    case OrderingTrait.Accessibility:
                        if (currentAccessLevel != previousAccessLevel)
                        {
                            compareConst = false;
                        }

                        continue;

                    case OrderingTrait.Readonly:
                        if (currentFieldReadonly != previousFieldReadonly)
                        {
                            compareConst = false;
                        }

                        continue;

                    case OrderingTrait.Static:
                        if (currentFieldStatic != previousFieldStatic)
                        {
                            compareConst = false;
                        }

                        continue;

                    case OrderingTrait.Kind:
                        // Only fields may be marked const, and all fields have the same kind.
                        continue;

                    case OrderingTrait.Constant:
                    default:
                        continue;
                    }
                }

                bool currentFieldConstant = field.Modifiers.Any(SyntaxKind.ConstKeyword);

                if (compareConst)
                {
                    if (!previousFieldConstant && currentFieldConstant)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(member)));
                    }
                }

                previousFieldConstant = currentFieldConstant;
                previousFieldReadonly = currentFieldReadonly;
                previousFieldStatic = currentFieldStatic;
                previousAccessLevel = currentAccessLevel;
            }
        }
    }
}
