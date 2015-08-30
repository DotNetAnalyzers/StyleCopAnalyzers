﻿namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# code element is missing a documentation header.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of Xml documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx"/>.</para>
    ///
    /// <para>A violation of this rule occurs if an element is completely missing a documentation header, or if the
    /// header is empty. In C# the following types of elements can have documentation headers: classes, constructors,
    /// delegates, enums, events, finalizers, indexers, interfaces, methods, properties, and structs.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1600ElementsMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1600ElementsMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1600";
        private const string Title = "Elements must be documented";
        private const string MessageFormat = "Elements must be documented";
        private const string Description = "A C# code element is missing a documentation header.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1600.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            BaseTypeDeclarationSyntax declaration = context.Node as BaseTypeDeclarationSyntax;

            bool isNestedInClassOrStruct = IsNestedType(declaration);

            if (declaration != null && NeedsComment(declaration.Modifiers, isNestedInClassOrStruct ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            MethodDeclarationSyntax declaration = context.Node as MethodDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (IsInterfaceMemberDeclaration(declaration) || declaration.ExplicitInterfaceSpecifier != null)
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            ConstructorDeclarationSyntax declaration = context.Node as ConstructorDeclarationSyntax;

            if (declaration != null && NeedsComment(declaration.Modifiers, SyntaxKind.PrivateKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private static void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            DestructorDeclarationSyntax declaration = context.Node as DestructorDeclarationSyntax;

            if (declaration != null)
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private static void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            PropertyDeclarationSyntax declaration = context.Node as PropertyDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (IsInterfaceMemberDeclaration(declaration) || declaration.ExplicitInterfaceSpecifier != null)
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            IndexerDeclarationSyntax declaration = context.Node as IndexerDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (IsInterfaceMemberDeclaration(declaration) || declaration.ExplicitInterfaceSpecifier != null)
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.ThisKeyword.GetLocation()));
                }
            }
        }

        private static void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            FieldDeclarationSyntax declaration = context.Node as FieldDeclarationSyntax;
            var variableDeclaration = declaration?.Declaration;

            if (variableDeclaration != null && NeedsComment(declaration.Modifiers, SyntaxKind.PrivateKeyword))
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

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            DelegateDeclarationSyntax declaration = context.Node as DelegateDeclarationSyntax;

            bool isNestedInClassOrStruct = IsNestedType(declaration);

            if (declaration != null && NeedsComment(declaration.Modifiers, isNestedInClassOrStruct ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private static void HandleEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            EventDeclarationSyntax declaration = context.Node as EventDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (declaration.ExplicitInterfaceSpecifier != null)
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private static void HandleEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
            {
                return;
            }

            EventFieldDeclarationSyntax declaration = context.Node as EventFieldDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (IsInterfaceMemberDeclaration(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            var variableDeclaration = declaration?.Declaration;

            if (variableDeclaration != null && NeedsComment(declaration.Modifiers, defaultVisibility))
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

        private static bool NeedsComment(SyntaxTokenList modifiers, SyntaxKind defaultModifier)
        {
            if (!(modifiers.Any(SyntaxKind.PublicKeyword)
                || modifiers.Any(SyntaxKind.ProtectedKeyword)
                || modifiers.Any(SyntaxKind.InternalKeyword)
                || defaultModifier == SyntaxKind.PublicKeyword
                || defaultModifier == SyntaxKind.ProtectedKeyword
                || defaultModifier == SyntaxKind.InternalKeyword))
            {
                return false;
            }

            // Also ignore partial classes because they get reported as SA1601
            return !modifiers.Any(SyntaxKind.PartialKeyword);
        }

        private static bool IsNestedType(BaseTypeDeclarationSyntax typeDeclaration)
        {
            return typeDeclaration?.Parent is BaseTypeDeclarationSyntax;
        }

        private static bool IsNestedType(DelegateDeclarationSyntax delegateDeclaration)
        {
            return delegateDeclaration?.Parent is BaseTypeDeclarationSyntax;
        }

        private static bool IsInterfaceMemberDeclaration(SyntaxNode declaration)
        {
            return declaration?.Parent is InterfaceDeclarationSyntax;
        }
    }
}
