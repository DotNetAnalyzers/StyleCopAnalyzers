// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class SyntaxWrapperHelper
    {
        private static readonly ImmutableDictionary<Type, Type> WrappedTypes;

        static SyntaxWrapperHelper()
        {
            var csharpCodeAnalysisAssembly = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly;
            var builder = ImmutableDictionary.CreateBuilder<Type, Type>();

            var objectCreationExpressionSyntaxType = csharpCodeAnalysisAssembly.GetType(BaseObjectCreationExpressionSyntaxWrapper.WrappedTypeName) ?? csharpCodeAnalysisAssembly.GetType(BaseObjectCreationExpressionSyntaxWrapper.FallbackWrappedTypeName);
            builder.Add(typeof(BaseObjectCreationExpressionSyntaxWrapper), objectCreationExpressionSyntaxType);
            builder.Add(typeof(BaseParameterSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(BaseParameterSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(BinaryPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(BinaryPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(CasePatternSwitchLabelSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(CasePatternSwitchLabelSyntaxWrapper.WrappedTypeName));
            var forEachStatementSyntaxType = csharpCodeAnalysisAssembly.GetType(CommonForEachStatementSyntaxWrapper.WrappedTypeName) ?? csharpCodeAnalysisAssembly.GetType(CommonForEachStatementSyntaxWrapper.FallbackWrappedTypeName);
            builder.Add(typeof(CommonForEachStatementSyntaxWrapper), forEachStatementSyntaxType);
            builder.Add(typeof(ConstantPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ConstantPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DeclarationExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(DeclarationExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DeclarationPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(DeclarationPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DefaultConstraintSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(DefaultConstraintSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DiscardDesignationSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(DiscardDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(DiscardPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(DiscardPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ExpressionOrPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ExpressionOrPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ForEachVariableStatementSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ForEachVariableStatementSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(FunctionPointerCallingConventionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(FunctionPointerCallingConventionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(FunctionPointerParameterListSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(FunctionPointerParameterListSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(FunctionPointerParameterSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(FunctionPointerParameterSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(FunctionPointerTypeSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(FunctionPointerTypeSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(FunctionPointerUnmanagedCallingConventionListSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(FunctionPointerUnmanagedCallingConventionListSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(FunctionPointerUnmanagedCallingConventionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(FunctionPointerUnmanagedCallingConventionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ImplicitObjectCreationExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ImplicitObjectCreationExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(IsPatternExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(IsPatternExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(LocalFunctionStatementSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(LocalFunctionStatementSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(NullableDirectiveTriviaSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(NullableDirectiveTriviaSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ParenthesizedPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ParenthesizedPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ParenthesizedVariableDesignationSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ParenthesizedVariableDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(PatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(PatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(PositionalPatternClauseSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(PositionalPatternClauseSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(PrimaryConstructorBaseTypeSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(PrimaryConstructorBaseTypeSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(PropertyPatternClauseSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(PropertyPatternClauseSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RangeExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RangeExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RecordDeclarationSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RecordDeclarationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RecursivePatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RecursivePatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RefExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RefExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RefTypeSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RefTypeSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(RelationalPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(RelationalPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(SingleVariableDesignationSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(SingleVariableDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(SubpatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(SubpatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(SwitchExpressionArmSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(SwitchExpressionArmSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(SwitchExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(SwitchExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(ThrowExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ThrowExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TupleElementSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(TupleElementSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TupleExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(TupleExpressionSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TupleTypeSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(TupleTypeSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(TypePatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(TypePatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(UnaryPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(UnaryPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(VariableDesignationSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(VariableDesignationSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(VarPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(VarPatternSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(WhenClauseSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(WhenClauseSyntaxWrapper.WrappedTypeName));
            builder.Add(typeof(WithExpressionSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(WithExpressionSyntaxWrapper.WrappedTypeName));

            WrappedTypes = builder.ToImmutable();
        }

        /// <summary>
        /// Gets the type that is wrapped by the given wrapper.
        /// </summary>
        /// <param name = "wrapperType">Type of the wrapper for which the wrapped type should be retrieved.</param>
        /// <returns>The wrapped type, or null if there is no info.</returns>
        internal static Type GetWrappedType(Type wrapperType)
        {
            if (WrappedTypes.TryGetValue(wrapperType, out Type wrappedType))
            {
                return wrappedType;
            }

            return null;
        }
    }
}
