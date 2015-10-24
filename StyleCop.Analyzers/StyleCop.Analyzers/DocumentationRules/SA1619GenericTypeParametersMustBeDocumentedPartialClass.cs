// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

    /// <summary>
    /// A generic, partial C# element is missing documentation for one or more of its generic type parameters, and the
    /// documentation for the element contains a <c>&lt;summary&gt;</c> tag.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when a generic, partial element is missing documentation for one or more
    /// of its generic type parameters, and the documentation for the element contains a <c>&lt;summary&gt;</c> tag
    /// rather than a <c>&lt;content&gt;</c> tag.</para>
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
    /// Class1. Documentation for the second part of Class1."</para>
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
    ///
    /// <para>When a generic element contains a <c>&lt;summary&gt;</c> tag within its documentation header, StyleCop
    /// assumes that this is the main part of the class, and requires the header to contain <c>&lt;typeparam&gt;</c>
    /// tags for each of the generic type parameters on the class. However, if the documentation header for the class
    /// contains a <c>&lt;content&gt;</c> tag rather than a <c>&lt;summary&gt;</c> tag, StyleCop will assume that the
    /// generic type parameters are defined on another part of the class, and will not require <c>&lt;typeparam&gt;</c>
    /// tags on this part of the class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1619GenericTypeParametersMustBeDocumentedPartialClass : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1619GenericTypeParametersMustBeDocumentedPartialClass"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1619";
        private const string Title = "Generic type parameters must be documented partial class";
        private const string MessageFormat = "The documentation for type parameter '{0}' is missing";
        private const string Description = "A generic, partial C# element is missing documentation for one or more of its generic type parameters, and the documentation for the element contains a <summary> tag.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1619.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration);

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
            TypeDeclarationSyntax typeDeclaration = (TypeDeclarationSyntax)context.Node;

            if (typeDeclaration.TypeParameterList == null)
            {
                // We are only interested in generic types
                return;
            }

            if (!typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }

            var documentation = typeDeclaration.GetDocumentationCommentTriviaSyntax();

            if (documentation == null)
            {
                // Don't report if the documentation is missing
                return;
            }

            if (documentation.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Ignore nodes with an <inheritdoc/> tag.
                return;
            }

            if (documentation.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag) == null)
            {
                // Ignore nodes without a <summary> tag.
                return;
            }

            var xmlParameterNames = documentation.Content.GetXmlElements(XmlCommentHelper.TypeParamXmlTag)
                .Select(XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>)
                .Where(x => x != null)
                .ToImmutableArray();

            foreach (var parameter in typeDeclaration.TypeParameterList.Parameters)
            {
                if (!xmlParameterNames.Any(x => x.Identifier.Identifier.ValueText == parameter.Identifier.ValueText))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Identifier.GetLocation(), parameter.Identifier.ValueText));
                }
            }
        }
    }
}
