namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Linq;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Helpers;
    using System;

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
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "A C# code element is missing a documentation header.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1600.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            BaseTypeDeclarationSyntax declaration = context.Node as BaseTypeDeclarationSyntax;

            bool isNestedInClassOrStruct = this.IsNestedType(declaration);

            if (declaration != null && this.NeedsComment(declaration.Modifiers, isNestedInClassOrStruct ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            MethodDeclarationSyntax declaration = context.Node as MethodDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (this.IsInterfaceMemberDeclaration(declaration) || declaration.ExplicitInterfaceSpecifier != null)
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && this.NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            ConstructorDeclarationSyntax declaration = context.Node as ConstructorDeclarationSyntax;

            if (declaration != null && this.NeedsComment(declaration.Modifiers, SyntaxKind.PrivateKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            DestructorDeclarationSyntax declaration = context.Node as DestructorDeclarationSyntax;

            if (declaration != null)
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax declaration = context.Node as PropertyDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (this.IsInterfaceMemberDeclaration(declaration) || declaration.ExplicitInterfaceSpecifier != null)
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && this.NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            IndexerDeclarationSyntax declaration = context.Node as IndexerDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (this.IsInterfaceMemberDeclaration(declaration) || declaration.ExplicitInterfaceSpecifier != null)
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && this.NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.ThisKeyword.GetLocation()));
                }
            }
        }

        private void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax declaration = context.Node as FieldDeclarationSyntax;
            var variableDeclaration = declaration?.Declaration;

            if (variableDeclaration != null && this.NeedsComment(declaration.Modifiers, SyntaxKind.PrivateKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    var locations = variableDeclaration.Variables.Select(v => v.Identifier.GetLocation()).ToArray();
                    foreach (var location in locations)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                    }
                }
            }
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            DelegateDeclarationSyntax declaration = context.Node as DelegateDeclarationSyntax;

            bool isNestedInClassOrStruct = this.IsNestedType(declaration);

            if (declaration != null && this.NeedsComment(declaration.Modifiers, isNestedInClassOrStruct ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            EventDeclarationSyntax declaration = context.Node as EventDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (declaration.ExplicitInterfaceSpecifier != null)
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && this.NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            EventFieldDeclarationSyntax declaration = context.Node as EventFieldDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (this.IsInterfaceMemberDeclaration(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            var variableDeclaration = declaration?.Declaration;

            if (variableDeclaration != null && this.NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    var locations = variableDeclaration.Variables.Select(v => v.Identifier.GetLocation()).ToArray();
                    foreach (var location in locations)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                    }
                }
            }
        }

        private bool NeedsComment(SyntaxTokenList modifiers, SyntaxKind defaultModifier)
        {

            return (modifiers.Any(SyntaxKind.PublicKeyword)
                || modifiers.Any(SyntaxKind.ProtectedKeyword)
                || modifiers.Any(SyntaxKind.InternalKeyword)
                || defaultModifier == SyntaxKind.PublicKeyword
                || defaultModifier == SyntaxKind.ProtectedKeyword
                || defaultModifier == SyntaxKind.InternalKeyword)
                // Ignore partial classes because they get reported as SA1601
                && !modifiers.Any(SyntaxKind.PartialKeyword);
        }

        private bool IsNestedType(BaseTypeDeclarationSyntax typeDeclaration)
        {
            return typeDeclaration?.Parent is BaseTypeDeclarationSyntax;
        }

        private bool IsNestedType(DelegateDeclarationSyntax delegateDeclaration)
        {
            return delegateDeclaration?.Parent is BaseTypeDeclarationSyntax;
        }

        private bool IsInterfaceMemberDeclaration(SyntaxNode declaration)
        {
            return declaration?.Parent is InterfaceDeclarationSyntax;
        }
    }
}
