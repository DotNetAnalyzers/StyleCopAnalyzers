// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
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
            var declarationSyntax = context.Node as BaseMethodDeclarationSyntax;
            if (declarationSyntax == null)
            {
                return MatchResult.Unknown;
            }

            var documentationStructure = declarationSyntax.GetDocumentationCommentTriviaSyntax();
            if (documentationStructure == null)
            {
                return MatchResult.Unknown;
            }

            var summaryElement = documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag) as XmlElementSyntax;
            if (summaryElement == null)
            {
                return MatchResult.Unknown;
            }

            // Check if the summary content could be a correct standard text
            if (summaryElement.Content.Count >= 3)
            {
                // Standard text has the form <part1><see><part2>
                var firstTextPartSyntax = summaryElement.Content[0] as XmlTextSyntax;
                var classReferencePart = summaryElement.Content[1] as XmlEmptyElementSyntax;
                var secondTextParSyntaxt = summaryElement.Content[2] as XmlTextSyntax;

                if (firstTextPartSyntax != null && classReferencePart != null && secondTextParSyntaxt != null)
                {
                    if (TextPartsMatch(firstTextPart, secondTextPart, firstTextPartSyntax, secondTextParSyntaxt))
                    {
                        if (SeeTagIsCorrect(context, classReferencePart, declarationSyntax))
                        {
                            // We found a correct standard text
                            return MatchResult.FoundMatch;
                        }
                        else
                        {
                            if (diagnosticDescriptor != null)
                            {
                                context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, classReferencePart.GetLocation()));
                            }

                            return MatchResult.None;
                        }
                    }
                }
            }

            if (diagnosticDescriptor != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, summaryElement.GetLocation()));
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
            INamedTypeSymbol actualSymbol = semanticModel.GetSymbolInfo(crefSyntax, context.CancellationToken).Symbol as INamedTypeSymbol;
            if (actualSymbol == null)
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
            if (!secondTextPartText.StartsWith(secondText, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }
    }
}
