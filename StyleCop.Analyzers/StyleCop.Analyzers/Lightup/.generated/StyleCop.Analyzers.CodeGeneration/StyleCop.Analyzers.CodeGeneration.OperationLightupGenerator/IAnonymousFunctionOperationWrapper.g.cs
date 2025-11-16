// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IAnonymousFunctionOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IAnonymousFunctionOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IMethodSymbol> SymbolAccessor;
        private static readonly Func<IOperation, IOperation> BodyAccessor;
        private readonly IOperation operation;
        static IAnonymousFunctionOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IAnonymousFunctionOperationWrapper));
            SymbolAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(Symbol));
            BodyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Body));
        }

        private IAnonymousFunctionOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IMethodSymbol Symbol => SymbolAccessor(this.WrappedOperation);
        public IBlockOperationWrapper Body => IBlockOperationWrapper.FromOperation(BodyAccessor(this.WrappedOperation));

        public static explicit operator IAnonymousFunctionOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IAnonymousFunctionOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IAnonymousFunctionOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IAnonymousFunctionOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
