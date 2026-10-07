// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IEmptyOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IEmptyOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static IEmptyOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IEmptyOperationWrapper));
        }

        private IEmptyOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;

        public static explicit operator IEmptyOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IEmptyOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IEmptyOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IEmptyOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
