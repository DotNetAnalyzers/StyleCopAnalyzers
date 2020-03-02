// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// A C# partial element is missing a documentation header.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs if a partial element (an element with the partial attribute) is completely
    /// missing a documentation header, or if the header is empty. In C# the following types of elements can be
    /// attributed with the partial attribute: classes, methods.</para>
    ///
    /// <para>When documentation is provided on more than one part of the partial class, the documentation for the two
    /// classes may be merged together to form a single source of documentation. For example, consider the following two
    /// parts of a partial class:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Documentation for the first part of Class1.
    /// /// &lt;/summary&gt;
    /// public partial class Class1
    /// {
    /// }
    ///
    /// /// &lt;summary&gt;
    /// /// Documentation for the second part of Class1.
    /// /// &lt;/summary&gt;
    /// public partial class Class1
    /// {
    /// }
    /// </code>
    ///
    /// <para>These two different parts of the same partial class each provide different documentation for the class.
    /// When the documentation for this class is built into an SDK, the tool building the documentation will either
    /// choose to use only one part of the documentation for the class and ignore the other parts, or, in some cases, it
    /// may merge the two sources of documentation together, to form a string like: "Documentation for the first part of
    /// Class1. Documentation for the second part of Class1.".</para>
    ///
    /// <para>For these reasons, it can be problematic to provide SDK documentation on more than one part of the partial
    /// class. However, it is still advisable to document each part of the class, to increase the readability and
    /// maintainability of the code, and StyleCop will require that each part of the class contain header
    /// documentation.</para>
    ///
    /// <para>This problem is solved through the use of the <c>&lt;content&gt;</c> tag, which can replace the
    /// <c>&lt;summary&gt;</c> tag for partial classes. The recommended practice for documenting partial classes is to
    /// provide the official SDK documentation for the class on the main part of the partial class. This documentation
    /// should be written using the standard <c>&lt;summary&gt;</c> tag. All other parts of the partial class should
    /// omit the <c>&lt;summary&gt;</c> tag completely, and replace it with a <c>&lt;content&gt;</c> tag. This allows
    /// the developer to document all parts of the partial class while still centralizing all of the official SDK
    /// documentation for the class onto one part of the class. The <c>&lt;content&gt;</c> tags will be ignored by the
    /// SDK documentation tools.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1601PartialElementsMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1601PartialElementsMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1601";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1601.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1601Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1601MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1601Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> BaseTypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> BaseTypeDeclarationAction = Analyzer.HandleBaseTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> MethodDeclarationAction = Analyzer.HandleMethodDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(BaseTypeDeclarationAction, BaseTypeDeclarationKinds);
            context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
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
                if (!declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    return;
                }

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
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
                if (!declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    return;
                }

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }
        }
    }
}
