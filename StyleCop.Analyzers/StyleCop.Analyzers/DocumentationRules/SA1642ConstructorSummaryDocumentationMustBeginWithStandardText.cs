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

    /// <summary>
    /// The XML documentation header for a C# constructor does not contain the appropriate summary text.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when the summary tag within the documentation header for a constructor
    /// does not begin with the proper text.</para>
    ///
    /// <para>The rule is intended to standardize the summary text for a constructor based on the access level of the
    /// constructor. The summary for a non-private instance constructor must begin with "Initializes a new instance of
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
    /// <para>Private instance constructors must use the summary text "Prevents a default instance of the {class name}
    /// class from being created."</para>
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
        private const string Title = "Constructor summary documentation must begin with standard text";
        private const string MessageFormat = "Constructor summary documentation must begin with standard text";
        private const string Description = "The XML documentation header for a C# constructor does not contain the appropriate summary text.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1642.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> ConstructorDeclarationAction = HandleConstructorDeclaration;

        /// <summary>
        /// Gets the standard text which is expected to appear at the beginning of the <c>&lt;summary&gt;</c>
        /// documentation for a non-private constructor.
        /// </summary>
        /// <value>
        /// The standard text which is expected to appear at the beginning of the <c>&lt;summary&gt;</c> documentation
        /// for a non-private constructor. This text appears before the name of the containing class, followed by a
        /// <c>&lt;see&gt;</c> element targeting the containing type, and finally followed by <c>class</c> or
        /// <c>struct</c> as appropriate for the containing type.
        /// </value>
        public static string NonPrivateConstructorStandardText { get; } = "Initializes a new instance of the ";

        /// <summary>
        /// Gets the standard text which is expected to appear at the beginning of the <c>&lt;summary&gt;</c>
        /// documentation for a private constructor.
        /// </summary>
        /// <remarks>
        /// <para>In addition to the format given in <see cref="PrivateConstructorStandardText"/>, a private constructor
        /// may choose to use <see cref="NonPrivateConstructorStandardText"/> instead. The code fix provided for this
        /// diagnostic uses <see cref="NonPrivateConstructorStandardText"/> by default, since this is generally a more
        /// accurate representation of a user's intent. In new code, <see langword="static"/> classes provide a
        /// superior alternative to private constructors for the purpose of declaring utility types that cannot be
        /// instantiated.</para>
        /// </remarks>
        /// <value>
        /// The standard text which is expected to appear at the beginning of the <c>&lt;summary&gt;</c> documentation
        /// for a private constructor. The first element appears before the name of the containing class, followed by a
        /// <c>&lt;see&gt;</c> element targeting the containing type, then by <c>class</c> or <c>struct</c> as
        /// appropriate for the containing type, and finally followed by the second element of this array.
        /// </value>
        public static ImmutableArray<string> PrivateConstructorStandardText { get; } = ImmutableArray.Create("Prevents a default instance of the ", " from being created");

        /// <summary>
        /// Gets the standard text which is expected to appear at the beginning of the <c>&lt;summary&gt;</c>
        /// documentation for a static constructor.
        /// </summary>
        /// <value>
        /// The standard text which is expected to appear at the beginning of the <c>&lt;summary&gt;</c> documentation
        /// for a static constructor. The first element appears before the name of the containing class, followed by a
        /// <c>&lt;see&gt;</c> element targeting the containing type, and finally followed by <c>class</c> or
        /// <c>struct</c> as appropriate for the containing type.
        /// </value>
        public static string StaticConstructorStandardText { get; } = "Initializes static members of the ";

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
            context.RegisterSyntaxNodeActionHonorExclusions(ConstructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;

            bool isStruct = constructorDeclarationSyntax.Parent?.IsKind(SyntaxKind.StructDeclaration) ?? false;

            if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                string secondPartText = isStruct ? " struct." : " class.";
                HandleDeclaration(context, StaticConstructorStandardText, secondPartText, Descriptor);
            }
            else if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                string typeKindText = isStruct ? " struct" : " class";

                if (HandleDeclaration(context, PrivateConstructorStandardText[0], typeKindText + PrivateConstructorStandardText[1], null) == MatchResult.FoundMatch)
                {
                    return;
                }

                // also allow the non-private wording for private constructors
                HandleDeclaration(context, NonPrivateConstructorStandardText, typeKindText, Descriptor);
            }
            else
            {
                string typeKindText = isStruct ? " struct" : " class";
                HandleDeclaration(context, NonPrivateConstructorStandardText, typeKindText, Descriptor);
            }
        }
    }
}
