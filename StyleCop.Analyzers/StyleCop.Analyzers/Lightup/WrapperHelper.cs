// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class WrapperHelper
    {
        private static readonly ImmutableDictionary<Type, Type> WrappedTypes;

        static WrapperHelper()
        {
            var codeAnalysisAssembly = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly;
            var builder = ImmutableDictionary.CreateBuilder<Type, Type>();

            builder.Add(typeof(CasePatternSwitchLabelSyntaxWrapper), codeAnalysisAssembly.GetType(CasePatternSwitchLabelSyntaxWrapper.WrappedTypeName));

            // Prior to C# 7, ForEachStatementSyntax was the base type for all foreach statements. If
            // the CommonForEachStatementSyntax type isn't found at runtime, we fall back to using this type instead.
            var forEachStatementSyntaxType = codeAnalysisAssembly.GetType(CommonForEachStatementSyntaxWrapper.WrappedTypeName)
                ?? codeAnalysisAssembly.GetType(CommonForEachStatementSyntaxWrapper.FallbackWrappedTypeName);
            builder.Add(typeof(CommonForEachStatementSyntaxWrapper), forEachStatementSyntaxType);

            builder.Add(typeof(ConstantPatternSyntaxWrapper), codeAnalysisAssembly.GetType(ConstantPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DeclarationExpressionSyntaxWrapper), codeAnalysisAssembly.GetType(DeclarationExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DeclarationPatternSyntaxWrapper), codeAnalysisAssembly.GetType(DeclarationPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DiscardDesignationSyntaxWrapper), codeAnalysisAssembly.GetType(DiscardDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ForEachVariableStatementSyntaxWrapper), codeAnalysisAssembly.GetType(ForEachVariableStatementSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(IsPatternExpressionSyntaxWrapper), codeAnalysisAssembly.GetType(IsPatternExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(LocalFunctionStatementSyntaxWrapper), codeAnalysisAssembly.GetType(LocalFunctionStatementSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ParenthesizedVariableDesignationSyntaxWrapper), codeAnalysisAssembly.GetType(ParenthesizedVariableDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(PatternSyntaxWrapper), codeAnalysisAssembly.GetType(PatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RefExpressionSyntaxWrapper), codeAnalysisAssembly.GetType(RefExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RefTypeSyntaxWrapper), codeAnalysisAssembly.GetType(RefTypeSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(SingleVariableDesignationSyntaxWrapper), codeAnalysisAssembly.GetType(SingleVariableDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ThrowExpressionSyntaxWrapper), codeAnalysisAssembly.GetType(ThrowExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TupleElementSyntaxWrapper), codeAnalysisAssembly.GetType(TupleElementSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TupleExpressionSyntaxWrapper), codeAnalysisAssembly.GetType(TupleExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TupleTypeSyntaxWrapper), codeAnalysisAssembly.GetType(TupleTypeSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(VariableDesignationSyntaxWrapper), codeAnalysisAssembly.GetType(VariableDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(WhenClauseSyntaxWrapper), codeAnalysisAssembly.GetType(WhenClauseSyntaxWrapper.WrappedTypeName));

            WrappedTypes = builder.ToImmutable();
        }

        /// <summary>
        /// Gets the type that is wrapped by the given wrapper.
        /// </summary>
        /// <param name="wrapperType">Type of the wrapper for which the wrapped type should be retrieved.</param>
        /// <returns>The wrapped type, or null if there is no info.</returns>
        internal static Type GetWrappedType(Type wrapperType)
        {
            Type wrappedType;
            if (WrappedTypes.TryGetValue(wrapperType, out wrappedType))
            {
                return wrappedType;
            }

            return null;
        }
    }
}
