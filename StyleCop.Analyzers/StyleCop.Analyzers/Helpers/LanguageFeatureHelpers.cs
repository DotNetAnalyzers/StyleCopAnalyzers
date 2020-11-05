// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// Helper methods for checking specific language versions.
    /// </summary>
    internal static class LanguageFeatureHelpers
    {
        /// <summary>
        /// Checks if the tuple language feature is supported.
        /// </summary>
        /// <param name="context">The analysis context that will be checked.</param>
        /// <returns>True if tuples are supported by the compiler.</returns>
        internal static bool SupportsTuples(this SyntaxNodeAnalysisContext context)
        {
            var csharpParseOptions = context.Node.SyntaxTree.Options as CSharpParseOptions;
            return (csharpParseOptions != null) && (csharpParseOptions.LanguageVersion >= LanguageVersionEx.CSharp7);
        }

        /// <summary>
        /// Checks if the tuple language feature is supported.
        /// </summary>
        /// <param name="context">The analysis context that will be checked.</param>
        /// <returns>True if tuples are supported by the compiler.</returns>
        internal static bool SupportsTuples(this OperationAnalysisContext context)
        {
            var csharpParseOptions = context.Operation.Syntax.SyntaxTree.Options as CSharpParseOptions;
            return (csharpParseOptions != null) && (csharpParseOptions.LanguageVersion >= LanguageVersionEx.CSharp7);
        }

        /// <summary>
        /// Checks if the inferred tuple element names language feature is supported.
        /// </summary>
        /// <param name="context">The analysis context that will be checked.</param>
        /// <returns>True if inferred tuple names are supported by the compiler.</returns>
        internal static bool SupportsInferredTupleElementNames(this SyntaxNodeAnalysisContext context)
        {
            var csharpParseOptions = context.Node.SyntaxTree.Options as CSharpParseOptions;
            return (csharpParseOptions != null) && (csharpParseOptions.LanguageVersion >= LanguageVersionEx.CSharp7_1);
        }
    }
}
