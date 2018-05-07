// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Xml.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A base class for diagnostics <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/> and <see cref="SA1643DestructorSummaryDocumentationMustBeginWithStandardText"/> to share common code.
    /// </summary>
    internal abstract class StandardTextDiagnosticBase : DiagnosticAnalyzer
    {
        /// <summary>
        /// The key used for signalling that no codefix should be offered.
        /// </summary>
        internal const string NoCodeFixKey = "NoCodeFix";

        private static readonly ImmutableDictionary<string, string> NoCodeFixProperties = ImmutableDictionary.Create<string, string>().Add(NoCodeFixKey, string.Empty);

        /// <summary>
        /// Describes the result of matching a summary element to a specific desired wording.
        /// </summary>
        public enum MatchResult
        {
            /// <summary>
            /// The analysis could not be completed due to errors in the syntax tree or a comment structure which was
            /// not accounted for.
            /// </summary>
            Unknown = -1,

            /// <summary>
            /// No complete or partial match was found.
            /// </summary>
            None,

            /// <summary>
            /// A standard text was found but the see element was incorrect.
            /// </summary>
            InvalidSeeTag,

            /// <summary>
            /// A match to the expected text was found.
            /// </summary>
            FoundMatch,
        }

        /// <summary>
        /// Analyzes a <see cref="BaseMethodDeclarationSyntax"/> node. If it has a summary it is checked if the text starts with &quot;[firstTextPart]&lt;see cref=&quot;[className]&quot;/&gt;[secondTextPart]&quot;.
        /// </summary>
        /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> of this analysis.</param>
        /// <param name="firstTextPart">The first part of the standard text.</param>
        /// <param name="secondTextPart">The second part of the standard text.</param>
        /// <param name="diagnosticDescriptor">The diagnostic to report for violations, or <see langword="null"/> to not report violations.</param>
        /// <returns>A <see cref="MatchResult"/> describing the result of the analysis.</returns>
        protected static MatchResult HandleDeclaration(SyntaxNodeAnalysisContext context, string firstTextPart, string secondTextPart, DiagnosticDescriptor diagnosticDescriptor)
        {
            var declarationSyntax = (BaseMethodDeclarationSyntax)context.Node;
            var documentationStructure = declarationSyntax.GetDocumentationCommentTriviaSyntax();
            if (documentationStructure == null)
            {
                return MatchResult.Unknown;
            }

            Location diagnosticLocation;
            ImmutableDictionary<string, string> diagnosticProperties;

            if (documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag) is XmlEmptyElementSyntax includeElement)
            {
                diagnosticLocation = includeElement.GetLocation();
                diagnosticProperties = NoCodeFixProperties;

                var declaration = context.SemanticModel.GetDeclaredSymbol(declarationSyntax, context.CancellationToken);
                if (declaration == null)
                {
                    return MatchResult.Unknown;
                }

                var rawDocumentation = declaration.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                var completeDocumentation = XElement.Parse(rawDocumentation, LoadOptions.None);

                var summaryElement = completeDocumentation.Nodes().OfType<XElement>().FirstOrDefault(element => element.Name == XmlCommentHelper.SummaryXmlTag);
                if (summaryElement == null)
                {
                    return MatchResult.Unknown;
                }

                var summaryNodes = summaryElement.Nodes().ToList();
                if (summaryNodes.Count >= 3)
                {
                    if (summaryNodes[0] is XText firstTextPartNode && summaryNodes[1] is XElement classReferencePart && summaryNodes[2] is XText secondTextPartNode)
                    {
                        if (TextPartsMatch(firstTextPart, secondTextPart, firstTextPartNode, secondTextPartNode))
                        {
                            if (SeeTagIsCorrect(context, classReferencePart, declarationSyntax))
                            {
                                // We found a correct standard text
                                return MatchResult.FoundMatch;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!(documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag) is XmlElementSyntax summaryElement))
                {
                    return MatchResult.Unknown;
                }

                diagnosticLocation = summaryElement.GetLocation();
                diagnosticProperties = ImmutableDictionary.Create<string, string>();

                // Check if the summary content could be a correct standard text
                if (summaryElement.Content.Count >= 3)
                {
                    // Standard text has the form <part1><see><part2>
                    if (summaryElement.Content[0] is XmlTextSyntax firstTextPartSyntax && summaryElement.Content[1] is XmlEmptyElementSyntax classReferencePart && summaryElement.Content[2] is XmlTextSyntax secondTextPartSyntax)
                    {
                        if (TextPartsMatch(firstTextPart, secondTextPart, firstTextPartSyntax, secondTextPartSyntax))
                        {
                            if (SeeTagIsCorrect(context, classReferencePart, declarationSyntax))
                            {
                                // We found a correct standard text
                                return MatchResult.FoundMatch;
                            }

                            diagnosticLocation = classReferencePart.GetLocation();
                        }
                    }
                }
            }

            if (diagnosticDescriptor != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, diagnosticLocation, diagnosticProperties));
            }

            // TODO: be more specific about the type of error when possible
            return MatchResult.None;
        }

        private static bool SeeTagIsCorrect(SyntaxNodeAnalysisContext context, XmlEmptyElementSyntax classReferencePart, BaseMethodDeclarationSyntax constructorDeclarationSyntax)
        {
            XmlCrefAttributeSyntax crefAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlCrefAttributeSyntax>(classReferencePart);
            CrefSyntax crefSyntax = crefAttribute?.Cref;
            if (crefAttribute == null)
            {
                return false;
            }

            SemanticModel semanticModel = context.SemanticModel;
            if (!(semanticModel.GetSymbolInfo(crefSyntax, context.CancellationToken).Symbol is INamedTypeSymbol actualSymbol))
            {
                return false;
            }

            INamedTypeSymbol expectedSymbol = semanticModel.GetDeclaredSymbol(constructorDeclarationSyntax.Parent, context.CancellationToken) as INamedTypeSymbol;
            return actualSymbol.OriginalDefinition == expectedSymbol;
        }

        private static bool SeeTagIsCorrect(SyntaxNodeAnalysisContext context, XElement classReferencePart, BaseMethodDeclarationSyntax constructorDeclarationSyntax)
        {
            var crefAttribute = classReferencePart.Attribute(XmlCommentHelper.CrefArgumentName);
            if (crefAttribute == null)
            {
                return false;
            }

            var typeName = crefAttribute.Value.Split(':').Last();

            SemanticModel semanticModel = context.SemanticModel;
            var foundSymbols = semanticModel.LookupNamespacesAndTypes(constructorDeclarationSyntax.SpanStart, name: typeName);
            if (foundSymbols.Length != 1)
            {
                return false;
            }

            if (!(foundSymbols[0] is INamedTypeSymbol actualSymbol))
            {
                return false;
            }

            INamedTypeSymbol expectedSymbol = semanticModel.GetDeclaredSymbol(constructorDeclarationSyntax.Parent, context.CancellationToken) as INamedTypeSymbol;
            return actualSymbol.OriginalDefinition == expectedSymbol;
        }

        private static bool TextPartsMatch(string firstText, string secondText, XmlTextSyntax firstTextPart, XmlTextSyntax secondTextPart)
        {
            string firstTextPartText = XmlCommentHelper.GetText(firstTextPart, normalizeWhitespace: true);
            if (firstText != firstTextPartText.TrimStart())
            {
                return false;
            }

            string secondTextPartText = XmlCommentHelper.GetText(secondTextPart, normalizeWhitespace: true);
            return secondTextPartText.StartsWith(secondText, StringComparison.Ordinal);
        }

        private static bool TextPartsMatch(string firstText, string secondText, XText firstTextPart, XText secondTextPart)
        {
            string firstTextPartText = firstTextPart.Value.TrimStart();
            if (!string.Equals(firstText, firstTextPartText, StringComparison.Ordinal))
            {
                return false;
            }

            string secondTextPartText = secondTextPart.Value;
            return secondTextPartText.StartsWith(secondText, StringComparison.Ordinal);
        }
    }
}
