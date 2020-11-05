// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class WrapperHelper
    {
        private static readonly ImmutableDictionary<Type, Type> WrappedTypes;

        static WrapperHelper()
        {
            var csharpCodeAnalysisAssembly = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly;
            var codeAnalysisAssembly = typeof(SyntaxNode).GetTypeInfo().Assembly;
            var builder = ImmutableDictionary.CreateBuilder<Type, Type>();

            builder.Add(typeof(CasePatternSwitchLabelSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(CasePatternSwitchLabelSyntaxWrapper.WrappedTypeName));

            // Prior to C# 7, ForEachStatementSyntax was the base type for all foreach statements. If
            // the CommonForEachStatementSyntax type isn't found at runtime, we fall back to using this type instead.
            var forEachStatementSyntaxType = csharpCodeAnalysisAssembly.GetType(CommonForEachStatementSyntaxWrapper.WrappedTypeName)
                ?? csharpCodeAnalysisAssembly.GetType(CommonForEachStatementSyntaxWrapper.FallbackWrappedTypeName);
            builder.Add(typeof(CommonForEachStatementSyntaxWrapper), forEachStatementSyntaxType);

            builder.Add(typeof(ConstantPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ConstantPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DeclarationExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(DeclarationExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DeclarationPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(DeclarationPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DiscardDesignationSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(DiscardDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DiscardPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(DiscardPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ForEachVariableStatementSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ForEachVariableStatementSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(IsPatternExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(IsPatternExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(LocalFunctionStatementSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(LocalFunctionStatementSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ParenthesizedVariableDesignationSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ParenthesizedVariableDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(PatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(PatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(PositionalPatternClauseSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(PositionalPatternClauseSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(PropertyPatternClauseSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(PropertyPatternClauseSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RangeExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RangeExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RecursivePatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RecursivePatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RefExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RefExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RefTypeSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RefTypeSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(SingleVariableDesignationSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(SingleVariableDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(SubpatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(SubpatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(SwitchExpressionArmSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(SwitchExpressionArmSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(SwitchExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(SwitchExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ThrowExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ThrowExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TupleElementSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(TupleElementSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TupleExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(TupleExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TupleTypeSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(TupleTypeSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(VarPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(VarPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(VariableDesignationSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(VariableDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(WhenClauseSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(WhenClauseSyntaxWrapper.WrappedTypeName));

            builder.Add(typeof(IArgumentOperationWrapper), codeAnalysisAssembly.GetType(IArgumentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IFieldReferenceOperationWrapper), codeAnalysisAssembly.GetType(IFieldReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IMemberReferenceOperationWrapper), codeAnalysisAssembly.GetType(IMemberReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IObjectCreationOperationWrapper), codeAnalysisAssembly.GetType(IObjectCreationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IObjectOrCollectionInitializerOperationWrapper), codeAnalysisAssembly.GetType(IObjectOrCollectionInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ITypeParameterObjectCreationOperationWrapper), codeAnalysisAssembly.GetType(ITypeParameterObjectCreationOperationWrapper.WrappedTypeName));

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
