// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The XML documentation header for a C# constructor does not contain the appropriate summary text.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs when the summary tag within the documentation header for a constructor
    /// does not begin with the proper text.</para>
    ///
    /// <para>The rule is intended to standardize the summary text for a constructor based on the access level of the
    /// constructor. The summary for a non-private instance constructor should begin with "Initializes a new instance of
    /// the {class name} class." For example, the following shows the constructor for the <c>Customer</c> class.</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Initializes a new instance of the Customer class.
    /// /// &lt;/summary&gt;
    /// public Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>It is possible to embed other tags into the summary text. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Initializes a new instance of the &lt;see cref="Customer"/&gt; class.
    /// /// &lt;/summary&gt;
    /// public Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>If the class contains generic parameters, these can be annotated within the <c>cref</c> link using either
    /// of the following two formats:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Initializes a new instance of the &lt;see cref="Customer`1"/&gt; class.
    /// /// &lt;/summary&gt;
    /// public Customer()
    /// {
    /// }
    ///
    /// /// &lt;summary&gt;
    /// /// Initializes a new instance of the &lt;see cref="Customer{T}"/&gt; class.
    /// /// &lt;/summary&gt;
    /// public Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>If the constructor is <see langword="static"/>, the summary text should begin with "Initializes static
    /// members of the {class name} class.” For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Initializes static members of the Customer class.
    /// /// &lt;/summary&gt;
    /// public static Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>Private instance constructors should use the summary text "Prevents a default instance of the {class name}
    /// class from being created.".</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Prevents a default instance of the Customer class from being created.
    /// /// &lt;/summary&gt;
    /// private Customer()
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1642ConstructorSummaryDocumentationMustBeginWithStandardText : StandardTextDiagnosticBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1642";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1642.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1642Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1642MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1642Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ConstructorDeclarationAction = HandleConstructorDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(context =>
            {
                context.RegisterSyntaxNodeAction(ConstructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
            });
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var constructorDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;

            var culture = settings.DocumentationRules.DocumentationCultureInfo;
            var resourceManager = DocumentationResources.ResourceManager;

            var parent = constructorDeclarationSyntax.Parent;
            bool isStruct = parent != null &&
                (parent.IsKind(SyntaxKind.StructDeclaration) || parent.IsKind(SyntaxKindEx.RecordStructDeclaration));
            var typeKindText = resourceManager.GetString(isStruct ? nameof(DocumentationResources.TypeTextStruct) : nameof(DocumentationResources.TypeTextClass), culture);

            if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                HandleDeclaration(
                    context,
                    string.Format(resourceManager.GetString(nameof(DocumentationResources.StaticConstructorStandardTextFirstPart), culture), typeKindText),
                    string.Format(resourceManager.GetString(nameof(DocumentationResources.StaticConstructorStandardTextSecondPart), culture), typeKindText),
                    Descriptor);
            }
            else if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                var privateConstructorMatch = HandleDeclaration(
                    context,
                    string.Format(resourceManager.GetString(nameof(DocumentationResources.PrivateConstructorStandardTextFirstPart), culture), typeKindText),
                    string.Format(
                        resourceManager.GetString(nameof(DocumentationResources.PrivateConstructorStandardTextSecondPart), culture),
                        typeKindText),
                    null);

                if (privateConstructorMatch == MatchResult.FoundMatch)
                {
                    return;
                }

                // also allow the non-private wording for private constructors
                HandleDeclaration(
                    context,
                    string.Format(
                        resourceManager.GetString(nameof(DocumentationResources.NonPrivateConstructorStandardTextFirstPart), culture),
                        typeKindText),
                    string.Format(
                        resourceManager.GetString(nameof(DocumentationResources.NonPrivateConstructorStandardTextSecondPart), culture),
                        typeKindText),
                    Descriptor);
            }
            else
            {
                HandleDeclaration(
                    context,
                    string.Format(
                        resourceManager.GetString(nameof(DocumentationResources.NonPrivateConstructorStandardTextFirstPart), culture),
                        typeKindText),
                    string.Format(
                        resourceManager.GetString(nameof(DocumentationResources.NonPrivateConstructorStandardTextSecondPart), culture),
                        typeKindText),
                    Descriptor);
            }
        }
    }
}
