// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
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
        private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
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
            if (syntaxKind == SyntaxKind.InterfaceDeclaration)
            {
                switch (documentationSettings.DocumentInterfaces)
                {
                case InterfaceDocumentationMode.All:
                    // DocumentInterfaces => all interfaces should be documented
                    return true;
                case InterfaceDocumentationMode.Exposed:
                    // DocumentInterfaces => only externally visible (exposed) interfaces should be documented
                    return declaredAccessibility != Accessibility.Internal;
                default:
                    // DocumentInterfaces => no interfaces should be exposed
                    return false;
                }
            }

            if (parentSyntaxKind == SyntaxKind.InterfaceDeclaration)
            {
                bool isPublicInterfaceMember = declaredAccessibility is Accessibility.Public or Accessibility.NotApplicable;
                if (!isPublicInterfaceMember)
                {
                    return documentationSettings.DocumentPrivateElements;
                }

                // At this point we know the declared accessibility is public
                switch (documentationSettings.DocumentInterfaces)
                {
                case InterfaceDocumentationMode.All:
                case InterfaceDocumentationMode.Exposed:
                    return true;

                default:
                    // DocumentInterfaces => no interface members should be documented
                    return false;
                }
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
                bool isRecord = declaration.IsKind(SyntaxKindEx.RecordDeclaration) || declaration.IsKind(SyntaxKindEx.RecordStructDeclaration);
                if (!isRecord && declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // Handled by SA1601
                    return;
                }

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                bool needsComment = NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility);
                if (isRecord)
                {
                    HandleRecordParameters(context, (TypeDeclarationSyntax)declaration, needsComment);
                }

                if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // The type documentation is handled by SA1601. Record parameters are checked above.
                    return;
                }

                if (needsComment)
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
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
                if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // Handled by SA1601
                    return;
                }

                if (declaration.ExplicitInterfaceSpecifier != null)
                {
                    return;
                }

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
                if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // Handled by SA1601
                    return;
                }

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
                if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // Handled by SA1601
                    return;
                }

                if (declaration.ExplicitInterfaceSpecifier != null)
                {
                    return;
                }

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
                if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // Handled by SA1601
                    return;
                }

                if (declaration.ExplicitInterfaceSpecifier != null)
                {
                    return;
                }

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
                if (declaration.ExplicitInterfaceSpecifier != null)
                {
                    return;
                }

                if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // Handled by SA1601
                    return;
                }

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
                if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // Handled by SA1601
                    return;
                }

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

            private static void HandleRecordParameters(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration, bool needsComment)
            {
                var parameterList = typeDeclaration.ParameterList();
                if (!needsComment || parameterList == null || parameterList.Parameters.Count == 0)
                {
                    return;
                }

                var documentation = typeDeclaration.GetDocumentationCommentTriviaSyntax();
                if (documentation?.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
                {
                    return;
                }

                var documentedParameters = new HashSet<string>(StringComparer.Ordinal);
                if (documentation != null)
                {
                    XElement completeDocumentation = null;
                    if (documentation.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag) != null)
                    {
                        var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken);
                        if (declaredSymbol != null)
                        {
                            var rawDocumentation = declaredSymbol.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                            completeDocumentation = XElement.Parse(rawDocumentation ?? "<doc></doc>", LoadOptions.None);
                        }
                    }

                    if (completeDocumentation != null)
                    {
                        if (completeDocumentation.Elements(XmlCommentHelper.InheritdocXmlTag).Any())
                        {
                            return;
                        }

                        foreach (var element in completeDocumentation.Elements(XmlCommentHelper.ParamXmlTag))
                        {
                            var name = element.Attribute("name");
                            if (name != null)
                            {
                                documentedParameters.Add(name.Value);
                            }
                        }
                    }
                    else
                    {
                        foreach (var element in documentation.Content.GetXmlElements(XmlCommentHelper.ParamXmlTag))
                        {
                            var name = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(element);
                            if (name != null)
                            {
                                documentedParameters.Add(name.Identifier.Identifier.ValueText);
                            }
                        }
                    }
                }

                foreach (var parameter in parameterList.Parameters)
                {
                    if (!documentedParameters.Contains(parameter.Identifier.ValueText))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Identifier.GetLocation()));
                    }
                }
            }
        }
    }
}
