// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Helpers;
    using Helpers.ObjectPools;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The XML documentation for a C# element contains two or more identical entries, indicating that the documentation
    /// has been copied and pasted. This can sometimes indicate invalid or poorly written documentation.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when an element contains two or more identical documentation texts. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Joins a first name and a last name together into a single string.
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="firstName"&gt;Part of the name.&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;Part of the name.&lt;/param&gt;
    /// /// &lt;returns&gt;The joined names.&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     return firstName + " " + lastName;
    /// }
    /// </code>
    ///
    /// <para>In some cases, a method may contain one or more parameters which are not used within the body of the
    /// method. In this case, the documentation for the parameter can be set to "The parameter is not used." StyleCop
    /// will allow multiple parameters to contain identical documentation as long as the documentation string is "The
    /// parameter is not used."</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Joins a first name and a last name together into a single string.
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="firstName"&gt;The first name to join.&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;The last name to join.&lt;/param&gt;
    /// /// &lt;returns&gt;The joined names.&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     return firstName + " " + lastName;
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1625ElementDocumentationMustNotBeCopiedAndPasted : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1625ElementDocumentationMustNotBeCopiedAndPasted"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1625";
        private const string ParameterNotUsed = "The parameter is not used.";
        private const string Title = "Element documentation must not be copied and pasted";
        private const string MessageFormat = "Element documentation must not be copied and pasted";
        private const string Description = "The Xml documentation for a C# element contains two or more identical entries, indicating that the documentation has been copied and pasted. This can sometimes indicate invalid or poorly written documentation.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1625.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> DocumentationTriviaAction = HandleDocumentationTrivia;

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
            context.RegisterSyntaxNodeActionHonorExclusions(DocumentationTriviaAction, SyntaxKinds.DocumentationComment);
        }

        private static void HandleDocumentationTrivia(SyntaxNodeAnalysisContext context)
        {
            DocumentationCommentTriviaSyntax syntax = context.Node as DocumentationCommentTriviaSyntax;

            var objectPool = SharedPools.Default<HashSet<string>>();
            HashSet<string> documentationTexts = objectPool.Allocate();

            foreach (var content in syntax.Content)
            {
                string text = XmlCommentHelper.GetText(content, true)?.Trim();

                if (string.IsNullOrWhiteSpace(text) || string.Equals(text, ParameterNotUsed, StringComparison.Ordinal))
                {
                    continue;
                }

                if (documentationTexts.Contains(text))
                {
                    // Add violation
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, content.GetLocation()));
                }
                else
                {
                    documentationTexts.Add(text);
                }
            }

            objectPool.ClearAndFree(documentationTexts);
        }
    }
}
