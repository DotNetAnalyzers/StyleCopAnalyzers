// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IMethodBodyOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IMethodBodyOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static IMethodBodyOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IMethodBodyOperationWrapper));
        }

        private IMethodBodyOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;

        public static explicit operator IMethodBodyOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IMethodBodyOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public IBlockOperationWrapper BlockBody => ((IMethodBodyBaseOperationWrapper)this).BlockBody;
        public IBlockOperationWrapper ExpressionBody => ((IMethodBodyBaseOperationWrapper)this).ExpressionBody;

        public static explicit operator IMethodBodyOperationWrapper(IMethodBodyBaseOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IMethodBodyBaseOperationWrapper(IMethodBodyOperationWrapper wrapper) => IMethodBodyBaseOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IMethodBodyOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IMethodBodyOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
