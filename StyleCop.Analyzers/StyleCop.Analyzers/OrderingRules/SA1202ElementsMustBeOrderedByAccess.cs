// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// An element within a C# code file is out of order within regard to access level, in relation to other elements in
    /// the code.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code elements within a file do not follow a standard ordering
    /// scheme based on access level.</para>
    ///
    /// <para>To comply with this rule, adjacent elements of the same type must be positioned in the following order by
    /// access level:</para>
    ///
    /// <list type="bullet">
    /// <item>public</item>
    /// <item>internal</item>
    /// <item>protected internal</item>
    /// <item>protected</item>
    /// <item>private</item>
    /// </list>
    ///
    /// <para>Complying with a standard ordering scheme based on access level can increase the readability and
    /// maintainability of the file and make it easier to identify the public interface that is being exposed from a
    /// class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1202ElementsMustBeOrderedByAccess : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1202ElementsMustBeOrderedByAccess"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1202";
        private const string Title = "Elements must be ordered by access";
        private const string MessageFormat = "All {0} {1} must come before {2} {1}.";
        private const string Description = "An element within a C# code file is out of order within regard to access level, in relation to other elements in the code.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1202.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

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

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;
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

            foreach (var member in members)
            {
                var currentSyntaxKind = member.Kind();
                currentSyntaxKind = currentSyntaxKind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : currentSyntaxKind;
                AccessLevel currentAccessLevel;
                var modifiers = member.GetModifiers();
                if ((currentSyntaxKind == SyntaxKind.ConstructorDeclaration && modifiers.Any(SyntaxKind.StaticKeyword))
                    || (currentSyntaxKind == SyntaxKind.MethodDeclaration && (member as MethodDeclarationSyntax)?.ExplicitInterfaceSpecifier != null)
                    || (currentSyntaxKind == SyntaxKind.PropertyDeclaration && (member as PropertyDeclarationSyntax)?.ExplicitInterfaceSpecifier != null)
                    || (currentSyntaxKind == SyntaxKind.IndexerDeclaration && (member as IndexerDeclarationSyntax)?.ExplicitInterfaceSpecifier != null))
                {
                    currentAccessLevel = AccessLevel.Public;
                }
                else
                {
                    currentAccessLevel = AccessLevelHelper.GetAccessLevel(modifiers);
                    currentAccessLevel = currentAccessLevel == AccessLevel.NotSpecified ? defaultAccessLevel : currentAccessLevel;
                }

                if (previousAccessLevel != AccessLevel.NotSpecified
                    && currentSyntaxKind == previousSyntaxKind
                    && currentAccessLevel > previousAccessLevel)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptor,
                            NamedTypeHelpers.GetNameOrIdentifierLocation(member),
                            AccessLevelHelper.GetName(currentAccessLevel),
                            MemberNames[currentSyntaxKind],
                            AccessLevelHelper.GetName(previousAccessLevel)));
                }

                previousSyntaxKind = currentSyntaxKind;
                previousAccessLevel = currentAccessLevel;
            }
        }
    }
}
