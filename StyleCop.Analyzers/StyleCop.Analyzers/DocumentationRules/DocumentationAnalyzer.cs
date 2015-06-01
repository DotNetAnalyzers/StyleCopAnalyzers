namespace StyleCop.Analyzers.DocumentationRules
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Base class for rules that enforce the presence of XML documentation comments.
    /// </summary>
    public abstract class DocumentationAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The identifier of the compiler diagnostic warning for missing XML documentation comments.
        /// </summary>
        protected const string CS1591DiagnosticId = @"CS1591";

        /// <summary>
        /// Gets the <see cref="DiagnosticDescriptor"/> to return for a rule violation with the specified
        /// resolved visibility.
        /// </summary>
        /// <param name="visibility">The resolved visibility; one of <see cref="SyntaxKind.PublicKeyword"/>,
        /// <see cref="SyntaxKind.InternalKeyword"/> or <see cref="SyntaxKind.PrivateKeyword"/>.</param>
        /// <param name="context">The analysis context.</param>
        /// <returns>The <see cref="DiagnosticDescriptor"/>, or <c>null</c> if no violation should be reported.</returns>
        protected abstract DiagnosticDescriptor DescriptorFromEffectiveVisibility(SyntaxKind visibility, SyntaxNodeAnalysisContext context);

        /// <summary>
        /// Handles a type declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            BaseTypeDeclarationSyntax declaration = context.Node as BaseTypeDeclarationSyntax;

            bool isNestedInClassOrStruct = this.IsNestedType(declaration);

            if (declaration != null && !XmlCommentHelper.HasDocumentation(declaration) && !declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    EffectiveVisibilityHelper.EffectiveVisibility(declaration.Modifiers, isNestedInClassOrStruct ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword),
                    declaration.Parent as BaseTypeDeclarationSyntax);

                ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), declaration.Identifier);
            }
        }

        /// <summary>
        /// Handles a method declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            MethodDeclarationSyntax declaration = context.Node as MethodDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (declaration.ExplicitInterfaceSpecifier != null)
            {
                // Treat explicit interface implementation as private.
                defaultVisibility = SyntaxKind.PrivateKeyword;
            }
            else if (this.IsInterfaceMemberDeclaration(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && !XmlCommentHelper.HasDocumentation(declaration) && !declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    EffectiveVisibilityHelper.EffectiveVisibility(declaration.Modifiers, defaultVisibility),
                    declaration.Parent as BaseTypeDeclarationSyntax);

                ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), declaration.Identifier);
            }
        }

        /// <summary>
        /// Handles a constructor declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            ConstructorDeclarationSyntax declaration = context.Node as ConstructorDeclarationSyntax;

            if (declaration != null && !XmlCommentHelper.HasDocumentation(declaration))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    EffectiveVisibilityHelper.EffectiveVisibility(declaration.Modifiers, SyntaxKind.PrivateKeyword),
                    declaration.Parent as BaseTypeDeclarationSyntax);

                ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), declaration.Identifier);
            }
        }

        /// <summary>
        /// Handles a destructor declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            DestructorDeclarationSyntax declaration = context.Node as DestructorDeclarationSyntax;

            if (declaration != null && !XmlCommentHelper.HasDocumentation(declaration))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    SyntaxKind.PublicKeyword,
                    declaration.Parent as BaseTypeDeclarationSyntax);

                ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), declaration.Identifier);
            }
        }

        /// <summary>
        /// Handles a property declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax declaration = context.Node as PropertyDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (declaration.ExplicitInterfaceSpecifier != null)
            {
                // Treat explicit interface implementation as private.
                defaultVisibility = SyntaxKind.PrivateKeyword;
            }
            else if (this.IsInterfaceMemberDeclaration(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && !XmlCommentHelper.HasDocumentation(declaration))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    EffectiveVisibilityHelper.EffectiveVisibility(declaration.Modifiers, defaultVisibility),
                    declaration.Parent as BaseTypeDeclarationSyntax);

                ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), declaration.Identifier);
            }
        }

        /// <summary>
        /// Handles an indexer declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            IndexerDeclarationSyntax declaration = context.Node as IndexerDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (declaration.ExplicitInterfaceSpecifier != null)
            {
                // Treat explicit interface implementation as private.
                defaultVisibility = SyntaxKind.PrivateKeyword;
            }
            else if (this.IsInterfaceMemberDeclaration(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && !XmlCommentHelper.HasDocumentation(declaration))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    EffectiveVisibilityHelper.EffectiveVisibility(declaration.Modifiers, defaultVisibility),
                    declaration.Parent as BaseTypeDeclarationSyntax);

                ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), declaration.ThisKeyword);
            }
        }

        /// <summary>
        /// Handles a field declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax declaration = context.Node as FieldDeclarationSyntax;
            var variableDeclaration = declaration?.Declaration;

            if (variableDeclaration != null && !XmlCommentHelper.HasDocumentation(declaration))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    EffectiveVisibilityHelper.EffectiveVisibility(declaration.Modifiers, SyntaxKind.PrivateKeyword),
                    declaration.Parent as BaseTypeDeclarationSyntax);

                var diagnostic = this.DescriptorFromEffectiveVisibility(effective, context);

                foreach (var variable in variableDeclaration.Variables)
                {
                    ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), variable.Identifier);
                }
            }
        }

        /// <summary>
        /// Handles a delegate declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            DelegateDeclarationSyntax declaration = context.Node as DelegateDeclarationSyntax;

            bool isNestedInClassOrStruct = this.IsNestedType(declaration);

            if (declaration != null && !XmlCommentHelper.HasDocumentation(declaration))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    EffectiveVisibilityHelper.EffectiveVisibility(declaration.Modifiers, isNestedInClassOrStruct ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword),
                    declaration.Parent as BaseTypeDeclarationSyntax);

                ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), declaration.Identifier);
            }
        }

        /// <summary>
        /// Handles an event declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            EventDeclarationSyntax declaration = context.Node as EventDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (declaration.ExplicitInterfaceSpecifier != null)
            {
                // Treat explicit interface implementation as private.
                defaultVisibility = SyntaxKind.PrivateKeyword;
            }
            else if (this.IsInterfaceMemberDeclaration(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && !XmlCommentHelper.HasDocumentation(declaration))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    EffectiveVisibilityHelper.EffectiveVisibility(declaration.Modifiers, defaultVisibility),
                    declaration.Parent as BaseTypeDeclarationSyntax);

                ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), declaration.Identifier);
            }
        }

        /// <summary>
        /// Handles an event field declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            EventFieldDeclarationSyntax declaration = context.Node as EventFieldDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (this.IsInterfaceMemberDeclaration(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            var variableDeclaration = declaration?.Declaration;

            if (variableDeclaration != null && !XmlCommentHelper.HasDocumentation(declaration))
            {
                var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                    EffectiveVisibilityHelper.EffectiveVisibility(declaration.Modifiers, defaultVisibility),
                    declaration.Parent as BaseTypeDeclarationSyntax);

                var diagnostic = this.DescriptorFromEffectiveVisibility(effective, context);

                foreach (var variable in variableDeclaration.Variables)
                {
                    ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), variable.Identifier);
                }
            }
        }

        /// <summary>
        /// Handles a partial type declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandlePartialTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            TypeDeclarationSyntax typeDeclaration = context.Node as TypeDeclarationSyntax;
            if (typeDeclaration != null)
            {
                if (typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    if (!XmlCommentHelper.HasDocumentation(typeDeclaration))
                    {
                        bool isNested = typeDeclaration.Parent is BaseTypeDeclarationSyntax;
                        var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                            EffectiveVisibilityHelper.EffectiveVisibility(typeDeclaration.Modifiers, isNested ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword),
                            typeDeclaration.Parent as BaseTypeDeclarationSyntax);

                        ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), typeDeclaration.Identifier);
                    }
                }
            }
        }

        /// <summary>
        /// Handles a partial method declaration.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandlePartialMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            MethodDeclarationSyntax methodDeclaration = context.Node as MethodDeclarationSyntax;
            if (methodDeclaration != null)
            {
                if (methodDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    if (!XmlCommentHelper.HasDocumentation(methodDeclaration))
                    {
                        ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(SyntaxKind.PrivateKeyword, context), methodDeclaration.Identifier);
                    }
                }
            }
        }

        /// <summary>
        /// Handles an enumeration member.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void HandleEnumMember(SyntaxNodeAnalysisContext context)
        {
            EnumMemberDeclarationSyntax enumMemberDeclaration = context.Node as EnumMemberDeclarationSyntax;
            if (enumMemberDeclaration != null)
            {
                if (!XmlCommentHelper.HasDocumentation(enumMemberDeclaration))
                {
                    var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(SyntaxKind.PublicKeyword, enumMemberDeclaration.Parent as BaseTypeDeclarationSyntax);

                    ReportDiagnostic(context, this.DescriptorFromEffectiveVisibility(effective, context), enumMemberDeclaration.Identifier);
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken locationToken)
        {
            if (descriptor == null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(descriptor, locationToken.GetLocation()));
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
