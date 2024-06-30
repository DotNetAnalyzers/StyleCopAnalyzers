// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// A C# code element is missing a documentation header.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of Xml documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs if an element is completely missing a documentation header, or if the
    /// header is empty. In C# the following types of elements can have documentation headers: classes, constructors,
    /// delegates, enums, events, finalizers, indexers, interfaces, methods, properties, records, and structs.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1600ElementsMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1600ElementsMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1600";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1600.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1600Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1600MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1600Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> BaseTypeDeclarationAction = Analyzer.HandleBaseTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> MethodDeclarationAction = Analyzer.HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ConstructorDeclarationAction = Analyzer.HandleConstructorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> DestructorDeclarationAction = Analyzer.HandleDestructorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> PropertyDeclarationAction = Analyzer.HandlePropertyDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> IndexerDeclarationAction = Analyzer.HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> FieldDeclarationAction = Analyzer.HandleFieldDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> DelegateDeclarationAction = Analyzer.HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> EventDeclarationAction = Analyzer.HandleEventDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> EventFieldDeclarationAction = Analyzer.HandleEventFieldDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        public static bool NeedsComment(DocumentationSettings documentationSettings, SyntaxKind syntaxKind, SyntaxKind parentSyntaxKind, Accessibility declaredAccessibility, Accessibility effectiveAccessibility)
        {
            if (documentationSettings.DocumentInterfaces
                && (syntaxKind == SyntaxKind.InterfaceDeclaration || parentSyntaxKind == SyntaxKind.InterfaceDeclaration))
            {
                // DocumentInterfaces => all interfaces should be documented
                return true;
            }

            if (syntaxKind == SyntaxKind.FieldDeclaration && documentationSettings.DocumentPrivateFields)
            {
                // DocumentPrivateFields => all fields should be documented
                return true;
            }

            if (documentationSettings.DocumentPrivateElements)
            {
                if (syntaxKind == SyntaxKind.FieldDeclaration && declaredAccessibility == Accessibility.Private)
                {
                    // Handled by DocumentPrivateFields
                    return false;
                }

                // DocumentPrivateMembers => everything except declared private fields should be documented
                return true;
            }

            switch (effectiveAccessibility)
            {
            case Accessibility.Public:
            case Accessibility.Protected:
            case Accessibility.ProtectedOrInternal:
                // These items are part of the exposed API surface => document if configured
                return documentationSettings.DocumentExposedElements;

            case Accessibility.ProtectedAndInternal:
            case Accessibility.Internal:
                // These items are part of the internal API surface => document if configured
                return documentationSettings.DocumentInternalElements;

            case Accessibility.NotApplicable:
            case Accessibility.Private:
            default:
                return false;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(context =>
            {
                context.RegisterSyntaxNodeAction(BaseTypeDeclarationAction, SyntaxKinds.BaseTypeDeclaration);
                context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
                context.RegisterSyntaxNodeAction(ConstructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
                context.RegisterSyntaxNodeAction(DestructorDeclarationAction, SyntaxKind.DestructorDeclaration);
                context.RegisterSyntaxNodeAction(PropertyDeclarationAction, SyntaxKind.PropertyDeclaration);
                context.RegisterSyntaxNodeAction(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
                context.RegisterSyntaxNodeAction(FieldDeclarationAction, SyntaxKind.FieldDeclaration);
                context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
                context.RegisterSyntaxNodeAction(EventDeclarationAction, SyntaxKind.EventDeclaration);
                context.RegisterSyntaxNodeAction(EventFieldDeclarationAction, SyntaxKind.EventFieldDeclaration);
            });
        }

        private static class Analyzer
        {
            public static void HandleBaseTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                BaseTypeDeclarationSyntax declaration = (BaseTypeDeclarationSyntax)context.Node;
                if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // Handled by SA1601
                    return;
                }

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (!NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    return;
                }

                XmlComment xmlComment = context.GetParsedXmlComment(context.Node);

                if (xmlComment.IsMissing)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }

                if (xmlComment.HasInheritdoc)
                {
                    return;
                }

                if (context.Node is TypeDeclarationSyntax typeDeclaration && RecordDeclarationSyntaxWrapper.IsInstance(typeDeclaration))
                {
                    RecordDeclarationSyntaxWrapper recordDeclaration = (RecordDeclarationSyntaxWrapper)typeDeclaration;

                    IEnumerable<ParameterSyntax> parameters = recordDeclaration.ParameterList?.Parameters ?? Enumerable.Empty<ParameterSyntax>();

                    foreach (var parameter in parameters)
                    {
                        if (!xmlComment.DocumentedParameterNames.Contains(parameter.Identifier.ValueText))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Identifier.GetLocation()));
                        }
                    }
                }
            }

            public static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                MethodDeclarationSyntax declaration = (MethodDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                ConstructorDeclarationSyntax declaration = (ConstructorDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public static void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                DestructorDeclarationSyntax declaration = (DestructorDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public static void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                PropertyDeclarationSyntax declaration = (PropertyDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                IndexerDeclarationSyntax declaration = (IndexerDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.ThisKeyword.GetLocation()));
                    }
                }
            }

            public static void HandleFieldDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                FieldDeclarationSyntax declaration = (FieldDeclarationSyntax)context.Node;
                var variableDeclaration = declaration.Declaration;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (variableDeclaration != null && NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        var locations = variableDeclaration.Variables.Select(v => v.Identifier.GetLocation());
                        foreach (var location in locations)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                        }
                    }
                }
            }

            public static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                DelegateDeclarationSyntax declaration = (DelegateDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public static void HandleEventDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                EventDeclarationSyntax declaration = (EventDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public static void HandleEventFieldDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                EventFieldDeclarationSyntax declaration = (EventFieldDeclarationSyntax)context.Node;
                VariableDeclarationSyntax variableDeclaration = declaration.Declaration;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (variableDeclaration != null && NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        var locations = variableDeclaration.Variables.Select(v => v.Identifier.GetLocation());
                        foreach (var location in locations)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                        }
                    }
                }
            }
        }
    }
}
