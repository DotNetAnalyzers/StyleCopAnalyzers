// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
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
        [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1012:Start action has no registered actions", Justification = "This is not a start action method.")]
        internal static bool SupportsTuples(this CompilationStartAnalysisContext context)
        {
            return context.Compilation is CSharpCompilation { LanguageVersion: >= LanguageVersionEx.CSharp7 };
        }

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

        internal static bool SupportsNativeSizedIntegers(this Compilation compilation)
        {
            if (compilation is not CSharpCompilation { LanguageVersion: >= LanguageVersionEx.CSharp11 } csharpCompilation)
            {
                return false;
            }

            var runtimeFeatureType = csharpCompilation.GetTypeByMetadataName("System.Runtime.CompilerServices.RuntimeFeature");
            if (runtimeFeatureType is null)
            {
                return false;
            }

            foreach (var member in runtimeFeatureType.GetMembers("NumericIntPtr"))
            {
                if (member is IFieldSymbol { IsConst: true, Type.SpecialType: SpecialType.System_String })
                {
                    return true;
                }
            }

            return false;
        }
    }
}
