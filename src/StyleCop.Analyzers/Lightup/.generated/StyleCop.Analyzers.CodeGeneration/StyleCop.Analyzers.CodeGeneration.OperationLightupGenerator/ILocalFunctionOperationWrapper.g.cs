// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ILocalFunctionOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ILocalFunctionOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IMethodSymbol> SymbolAccessor;
        private static readonly Func<IOperation, IOperation> BodyAccessor;
        private static readonly Func<IOperation, IOperation> IgnoredBodyAccessor;
        private readonly IOperation operation;
        static ILocalFunctionOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ILocalFunctionOperationWrapper));
            SymbolAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(Symbol));
            BodyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Body));
            IgnoredBodyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(IgnoredBody));
        }

        private ILocalFunctionOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IMethodSymbol Symbol => SymbolAccessor(this.WrappedOperation);
        public IBlockOperationWrapper Body => IBlockOperationWrapper.FromOperation(BodyAccessor(this.WrappedOperation));
        public IBlockOperationWrapper IgnoredBody => IBlockOperationWrapper.FromOperation(IgnoredBodyAccessor(this.WrappedOperation));

        public static explicit operator ILocalFunctionOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(ILocalFunctionOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ILocalFunctionOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ILocalFunctionOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
