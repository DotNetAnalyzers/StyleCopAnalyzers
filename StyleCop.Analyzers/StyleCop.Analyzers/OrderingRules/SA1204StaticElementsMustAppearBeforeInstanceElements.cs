// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A static element is positioned beneath an instance element of the same type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a static element is positioned beneath an instance element of the
    /// same type. All static elements must be placed above all instance elements of the same type to make it easier to
    /// see the interface exposed from the instance and static version of the class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1204StaticElementsMustAppearBeforeInstanceElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1204StaticElementsMustAppearBeforeInstanceElements"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1204";
        private const string Title = "Static elements must appear before instance elements";
        private const string MessageFormat = "All {0} static {1} must appear before {0} non-static {1}.";
        private const string Description = "A static element is positioned beneath an instance element of the same type.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1204.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> TypeDeclarationAction = HandleTypeDeclaration;

        private static readonly Dictionary<SyntaxKind, string> MemberNames = new Dictionary<SyntaxKind, string>
        {
            [SyntaxKind.DelegateDeclaration] = "delegates",
            [SyntaxKind.EnumDeclaration] = "enums",
            [SyntaxKind.InterfaceDeclaration] = "interfaces",
            [SyntaxKind.StructDeclaration] = "structs",
            [SyntaxKind.ClassDeclaration] = "classes",
            [SyntaxKind.FieldDeclaration] = "fields",
            [SyntaxKind.ConstructorDeclaration] = "constructors",
            [SyntaxKind.EventDeclaration] = "events",
            [SyntaxKind.PropertyDeclaration] = "properties",
            [SyntaxKind.IndexerDeclaration] = "indexers",
            [SyntaxKind.MethodDeclaration] = "methods",
            [SyntaxKind.ConversionOperatorDeclaration] = "conversions",
            [SyntaxKind.OperatorDeclaration] = "operators"
        };

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
            context.RegisterSyntaxNodeActionHonorExclusions(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeActionHonorExclusions(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(TypeDeclarationAction, TypeDeclarationKinds);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            HandleMemberList(context, compilationUnit.Members, AccessLevel.Internal);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (NamespaceDeclarationSyntax)context.Node;

            HandleMemberList(context, compilationUnit.Members, AccessLevel.Internal);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            HandleMemberList(context, typeDeclaration.Members, AccessLevel.Private);
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members, AccessLevel defaultAccessLevel)
        {
            var previousSyntaxKind = SyntaxKind.None;
            var previousAccessLevel = AccessLevel.NotSpecified;
            var previousMemberStatic = true;
            foreach (var member in members)
            {
                var currentSyntaxKind = member.Kind();
                currentSyntaxKind = currentSyntaxKind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : currentSyntaxKind;
                var modifiers = member.GetModifiers();
                var currentMemberStatic = modifiers.Any(SyntaxKind.StaticKeyword);
                var currentMemberConst = modifiers.Any(SyntaxKind.ConstKeyword);
                AccessLevel currentAccessLevel;
                if ((currentSyntaxKind == SyntaxKind.ConstructorDeclaration && modifiers.Any(SyntaxKind.StaticKeyword))
                    || (currentSyntaxKind == SyntaxKind.MethodDeclaration && (member as MethodDeclarationSyntax)?.ExplicitInterfaceSpecifier != null)
                    || (currentSyntaxKind == SyntaxKind.PropertyDeclaration && (member as PropertyDeclarationSyntax)?.ExplicitInterfaceSpecifier != null)
                    || (currentSyntaxKind == SyntaxKind.IndexerDeclaration && (member as IndexerDeclarationSyntax)?.ExplicitInterfaceSpecifier != null))
                {
                    currentAccessLevel = AccessLevel.Public;
                }
                else
                {
                    currentAccessLevel = AccessLevelHelper.GetAccessLevel(member.GetModifiers());
                    currentAccessLevel = currentAccessLevel == AccessLevel.NotSpecified ? defaultAccessLevel : currentAccessLevel;
                }

                if (currentSyntaxKind == previousSyntaxKind
                    && currentAccessLevel == previousAccessLevel
                    && !previousMemberStatic
                    && currentMemberStatic
                    && !currentMemberConst)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptor,
                            NamedTypeHelpers.GetNameOrIdentifierLocation(member),
                            AccessLevelHelper.GetName(currentAccessLevel),
                            MemberNames[currentSyntaxKind]));
                }

                previousSyntaxKind = currentSyntaxKind;
                previousAccessLevel = currentAccessLevel;
                previousMemberStatic = currentMemberStatic || currentMemberConst;
            }
        }
    }
}
