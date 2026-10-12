// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    internal static class OperationWrapperHelper
    {
        private static readonly ImmutableDictionary<Type, Type> WrappedTypes;
        static OperationWrapperHelper()
        {
            var codeAnalysisAssembly = typeof(SyntaxNode).GetTypeInfo().Assembly;
            var builder = ImmutableDictionary.CreateBuilder<Type, Type>();
            builder.Add(typeof(IOperationWrapper), typeof(IOperation));
            builder.Add(typeof(ILoopOperationWrapper), codeAnalysisAssembly.GetType(ILoopOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IMemberReferenceOperationWrapper), codeAnalysisAssembly.GetType(IMemberReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IAssignmentOperationWrapper), codeAnalysisAssembly.GetType(IAssignmentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ISymbolInitializerOperationWrapper), codeAnalysisAssembly.GetType(ISymbolInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ICaseClauseOperationWrapper), codeAnalysisAssembly.GetType(ICaseClauseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IInterpolatedStringContentOperationWrapper), codeAnalysisAssembly.GetType(IInterpolatedStringContentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IPatternOperationWrapper), codeAnalysisAssembly.GetType(IPatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IMethodBodyBaseOperationWrapper), codeAnalysisAssembly.GetType(IMethodBodyBaseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IInvalidOperationWrapper), codeAnalysisAssembly.GetType(IInvalidOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IBlockOperationWrapper), codeAnalysisAssembly.GetType(IBlockOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IVariableDeclarationGroupOperationWrapper), codeAnalysisAssembly.GetType(IVariableDeclarationGroupOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ISwitchOperationWrapper), codeAnalysisAssembly.GetType(ISwitchOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IForEachLoopOperationWrapper), codeAnalysisAssembly.GetType(IForEachLoopOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IForLoopOperationWrapper), codeAnalysisAssembly.GetType(IForLoopOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IForToLoopOperationWrapper), codeAnalysisAssembly.GetType(IForToLoopOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IWhileLoopOperationWrapper), codeAnalysisAssembly.GetType(IWhileLoopOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ILabeledOperationWrapper), codeAnalysisAssembly.GetType(ILabeledOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IBranchOperationWrapper), codeAnalysisAssembly.GetType(IBranchOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IEmptyOperationWrapper), codeAnalysisAssembly.GetType(IEmptyOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IReturnOperationWrapper), codeAnalysisAssembly.GetType(IReturnOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ILockOperationWrapper), codeAnalysisAssembly.GetType(ILockOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ITryOperationWrapper), codeAnalysisAssembly.GetType(ITryOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IUsingOperationWrapper), codeAnalysisAssembly.GetType(IUsingOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IExpressionStatementOperationWrapper), codeAnalysisAssembly.GetType(IExpressionStatementOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ILocalFunctionOperationWrapper), codeAnalysisAssembly.GetType(ILocalFunctionOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IStopOperationWrapper), codeAnalysisAssembly.GetType(IStopOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IEndOperationWrapper), codeAnalysisAssembly.GetType(IEndOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IRaiseEventOperationWrapper), codeAnalysisAssembly.GetType(IRaiseEventOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ILiteralOperationWrapper), codeAnalysisAssembly.GetType(ILiteralOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IConversionOperationWrapper), codeAnalysisAssembly.GetType(IConversionOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IInvocationOperationWrapper), codeAnalysisAssembly.GetType(IInvocationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IArrayElementReferenceOperationWrapper), codeAnalysisAssembly.GetType(IArrayElementReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ILocalReferenceOperationWrapper), codeAnalysisAssembly.GetType(ILocalReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IParameterReferenceOperationWrapper), codeAnalysisAssembly.GetType(IParameterReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IFieldReferenceOperationWrapper), codeAnalysisAssembly.GetType(IFieldReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IMethodReferenceOperationWrapper), codeAnalysisAssembly.GetType(IMethodReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IPropertyReferenceOperationWrapper), codeAnalysisAssembly.GetType(IPropertyReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IEventReferenceOperationWrapper), codeAnalysisAssembly.GetType(IEventReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IUnaryOperationWrapper), codeAnalysisAssembly.GetType(IUnaryOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IBinaryOperationWrapper), codeAnalysisAssembly.GetType(IBinaryOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IConditionalOperationWrapper), codeAnalysisAssembly.GetType(IConditionalOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ICoalesceOperationWrapper), codeAnalysisAssembly.GetType(ICoalesceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IAnonymousFunctionOperationWrapper), codeAnalysisAssembly.GetType(IAnonymousFunctionOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IObjectCreationOperationWrapper), codeAnalysisAssembly.GetType(IObjectCreationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ITypeParameterObjectCreationOperationWrapper), codeAnalysisAssembly.GetType(ITypeParameterObjectCreationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IArrayCreationOperationWrapper), codeAnalysisAssembly.GetType(IArrayCreationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IInstanceReferenceOperationWrapper), codeAnalysisAssembly.GetType(IInstanceReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IIsTypeOperationWrapper), codeAnalysisAssembly.GetType(IIsTypeOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IAwaitOperationWrapper), codeAnalysisAssembly.GetType(IAwaitOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ISimpleAssignmentOperationWrapper), codeAnalysisAssembly.GetType(ISimpleAssignmentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ICompoundAssignmentOperationWrapper), codeAnalysisAssembly.GetType(ICompoundAssignmentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IParenthesizedOperationWrapper), codeAnalysisAssembly.GetType(IParenthesizedOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IEventAssignmentOperationWrapper), codeAnalysisAssembly.GetType(IEventAssignmentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IConditionalAccessOperationWrapper), codeAnalysisAssembly.GetType(IConditionalAccessOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IConditionalAccessInstanceOperationWrapper), codeAnalysisAssembly.GetType(IConditionalAccessInstanceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IInterpolatedStringOperationWrapper), codeAnalysisAssembly.GetType(IInterpolatedStringOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IAnonymousObjectCreationOperationWrapper), codeAnalysisAssembly.GetType(IAnonymousObjectCreationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IObjectOrCollectionInitializerOperationWrapper), codeAnalysisAssembly.GetType(IObjectOrCollectionInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IMemberInitializerOperationWrapper), codeAnalysisAssembly.GetType(IMemberInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ICollectionElementInitializerOperationWrapper), codeAnalysisAssembly.GetType(ICollectionElementInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(INameOfOperationWrapper), codeAnalysisAssembly.GetType(INameOfOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ITupleOperationWrapper), codeAnalysisAssembly.GetType(ITupleOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDynamicObjectCreationOperationWrapper), codeAnalysisAssembly.GetType(IDynamicObjectCreationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDynamicMemberReferenceOperationWrapper), codeAnalysisAssembly.GetType(IDynamicMemberReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDynamicInvocationOperationWrapper), codeAnalysisAssembly.GetType(IDynamicInvocationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDynamicIndexerAccessOperationWrapper), codeAnalysisAssembly.GetType(IDynamicIndexerAccessOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ITranslatedQueryOperationWrapper), codeAnalysisAssembly.GetType(ITranslatedQueryOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDelegateCreationOperationWrapper), codeAnalysisAssembly.GetType(IDelegateCreationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDefaultValueOperationWrapper), codeAnalysisAssembly.GetType(IDefaultValueOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ITypeOfOperationWrapper), codeAnalysisAssembly.GetType(ITypeOfOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ISizeOfOperationWrapper), codeAnalysisAssembly.GetType(ISizeOfOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IAddressOfOperationWrapper), codeAnalysisAssembly.GetType(IAddressOfOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IIsPatternOperationWrapper), codeAnalysisAssembly.GetType(IIsPatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IIncrementOrDecrementOperationWrapper), codeAnalysisAssembly.GetType(IIncrementOrDecrementOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IThrowOperationWrapper), codeAnalysisAssembly.GetType(IThrowOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDeconstructionAssignmentOperationWrapper), codeAnalysisAssembly.GetType(IDeconstructionAssignmentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDeclarationExpressionOperationWrapper), codeAnalysisAssembly.GetType(IDeclarationExpressionOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IOmittedArgumentOperationWrapper), codeAnalysisAssembly.GetType(IOmittedArgumentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IFieldInitializerOperationWrapper), codeAnalysisAssembly.GetType(IFieldInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IVariableInitializerOperationWrapper), codeAnalysisAssembly.GetType(IVariableInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IPropertyInitializerOperationWrapper), codeAnalysisAssembly.GetType(IPropertyInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IParameterInitializerOperationWrapper), codeAnalysisAssembly.GetType(IParameterInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IArrayInitializerOperationWrapper), codeAnalysisAssembly.GetType(IArrayInitializerOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IVariableDeclaratorOperationWrapper), codeAnalysisAssembly.GetType(IVariableDeclaratorOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IVariableDeclarationOperationWrapper), codeAnalysisAssembly.GetType(IVariableDeclarationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IArgumentOperationWrapper), codeAnalysisAssembly.GetType(IArgumentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ICatchClauseOperationWrapper), codeAnalysisAssembly.GetType(ICatchClauseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ISwitchCaseOperationWrapper), codeAnalysisAssembly.GetType(ISwitchCaseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDefaultCaseClauseOperationWrapper), codeAnalysisAssembly.GetType(IDefaultCaseClauseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IPatternCaseClauseOperationWrapper), codeAnalysisAssembly.GetType(IPatternCaseClauseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IRangeCaseClauseOperationWrapper), codeAnalysisAssembly.GetType(IRangeCaseClauseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IRelationalCaseClauseOperationWrapper), codeAnalysisAssembly.GetType(IRelationalCaseClauseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ISingleValueCaseClauseOperationWrapper), codeAnalysisAssembly.GetType(ISingleValueCaseClauseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IInterpolatedStringTextOperationWrapper), codeAnalysisAssembly.GetType(IInterpolatedStringTextOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IInterpolationOperationWrapper), codeAnalysisAssembly.GetType(IInterpolationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IConstantPatternOperationWrapper), codeAnalysisAssembly.GetType(IConstantPatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDeclarationPatternOperationWrapper), codeAnalysisAssembly.GetType(IDeclarationPatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ITupleBinaryOperationWrapper), codeAnalysisAssembly.GetType(ITupleBinaryOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IMethodBodyOperationWrapper), codeAnalysisAssembly.GetType(IMethodBodyOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IConstructorBodyOperationWrapper), codeAnalysisAssembly.GetType(IConstructorBodyOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDiscardOperationWrapper), codeAnalysisAssembly.GetType(IDiscardOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IFlowCaptureOperationWrapper), codeAnalysisAssembly.GetType(IFlowCaptureOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IFlowCaptureReferenceOperationWrapper), codeAnalysisAssembly.GetType(IFlowCaptureReferenceOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IIsNullOperationWrapper), codeAnalysisAssembly.GetType(IIsNullOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ICaughtExceptionOperationWrapper), codeAnalysisAssembly.GetType(ICaughtExceptionOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IStaticLocalInitializationSemaphoreOperationWrapper), codeAnalysisAssembly.GetType(IStaticLocalInitializationSemaphoreOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IFlowAnonymousFunctionOperationWrapper), codeAnalysisAssembly.GetType(IFlowAnonymousFunctionOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ICoalesceAssignmentOperationWrapper), codeAnalysisAssembly.GetType(ICoalesceAssignmentOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IRangeOperationWrapper), codeAnalysisAssembly.GetType(IRangeOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IReDimOperationWrapper), codeAnalysisAssembly.GetType(IReDimOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IReDimClauseOperationWrapper), codeAnalysisAssembly.GetType(IReDimClauseOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IRecursivePatternOperationWrapper), codeAnalysisAssembly.GetType(IRecursivePatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IDiscardPatternOperationWrapper), codeAnalysisAssembly.GetType(IDiscardPatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ISwitchExpressionOperationWrapper), codeAnalysisAssembly.GetType(ISwitchExpressionOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ISwitchExpressionArmOperationWrapper), codeAnalysisAssembly.GetType(ISwitchExpressionArmOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IPropertySubpatternOperationWrapper), codeAnalysisAssembly.GetType(IPropertySubpatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IUsingDeclarationOperationWrapper), codeAnalysisAssembly.GetType(IUsingDeclarationOperationWrapper.WrappedTypeName));
            builder.Add(typeof(INegatedPatternOperationWrapper), codeAnalysisAssembly.GetType(INegatedPatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IBinaryPatternOperationWrapper), codeAnalysisAssembly.GetType(IBinaryPatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(ITypePatternOperationWrapper), codeAnalysisAssembly.GetType(ITypePatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IRelationalPatternOperationWrapper), codeAnalysisAssembly.GetType(IRelationalPatternOperationWrapper.WrappedTypeName));
            builder.Add(typeof(IWithOperationWrapper), codeAnalysisAssembly.GetType(IWithOperationWrapper.WrappedTypeName));
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
