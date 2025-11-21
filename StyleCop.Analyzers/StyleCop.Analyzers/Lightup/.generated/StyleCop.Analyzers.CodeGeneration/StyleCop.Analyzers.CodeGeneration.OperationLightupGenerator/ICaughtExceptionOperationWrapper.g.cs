// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ICaughtExceptionOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.FlowAnalysis.ICaughtExceptionOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static ICaughtExceptionOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ICaughtExceptionOperationWrapper));
        }

        private ICaughtExceptionOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public static explicit operator ICaughtExceptionOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(ICaughtExceptionOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ICaughtExceptionOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ICaughtExceptionOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
