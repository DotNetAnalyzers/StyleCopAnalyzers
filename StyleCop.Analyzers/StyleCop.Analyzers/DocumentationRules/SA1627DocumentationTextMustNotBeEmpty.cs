// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The XML header documentation for a C# code element contains an empty tag.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when the documentation header for an element contains an empty tag. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Joins a first name and a last name together into a single string.
    /// /// &lt;/summary&gt;
    /// /// &lt;remarks&gt;&lt;/remarks&gt;
    /// /// &lt;param name="firstName"&gt;Other part of name.&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;Part of the name.&lt;/param&gt;
    /// /// &lt;returns&gt;The joined names.&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("Cannot generate documentation")]
    internal class SA1627DocumentationTextMustNotBeEmpty : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1627DocumentationTextMustNotBeEmpty"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1627";
        private const string Title = "Documentation text must not be empty";
        private const string MessageFormat = "The documentation text within the {0} tag must not be empty.";
        private const string Description = "The XML header documentation for a C# code element contains an empty tag.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1627.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        private static string[] elementsToCheck =
            {
                XmlCommentHelper.RemarksXmlTag,
                XmlCommentHelper.PermissionXmlTag,
                XmlCommentHelper.ExceptionXmlTag,
                XmlCommentHelper.ExampleXmlTag
            };

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
            context.RegisterSyntaxNodeActionHonorExclusions(HandleXmlElement, SyntaxKind.XmlElement);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleXmlEmptyElement, SyntaxKind.XmlEmptyElement);
        }

        private static void HandleXmlElement(SyntaxNodeAnalysisContext context)
        {
            XmlElementSyntax emptyElement = context.Node as XmlElementSyntax;

            var name = emptyElement?.StartTag?.Name;

            if (elementsToCheck.Contains(name.ToString()) && XmlCommentHelper.IsConsideredEmpty(emptyElement))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, emptyElement.GetLocation(), name.ToString()));
            }
        }

        private static void HandleXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            XmlEmptyElementSyntax emptyElement = context.Node as XmlEmptyElementSyntax;

            if (elementsToCheck.Contains(emptyElement?.Name.ToString()))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, emptyElement.GetLocation(), emptyElement?.Name.ToString()));
            }
        }
    }
}
