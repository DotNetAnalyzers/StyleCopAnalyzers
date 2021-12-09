// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IFlowAnonymousFunctionOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.FlowAnalysis.IFlowAnonymousFunctionOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IMethodSymbol> SymbolAccessor;
        private readonly IOperation operation;
        static IFlowAnonymousFunctionOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IFlowAnonymousFunctionOperationWrapper));
            SymbolAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(Symbol));
        }

        private IFlowAnonymousFunctionOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IMethodSymbol Symbol => SymbolAccessor(this.WrappedOperation);
        public static IFlowAnonymousFunctionOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IFlowAnonymousFunctionOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
