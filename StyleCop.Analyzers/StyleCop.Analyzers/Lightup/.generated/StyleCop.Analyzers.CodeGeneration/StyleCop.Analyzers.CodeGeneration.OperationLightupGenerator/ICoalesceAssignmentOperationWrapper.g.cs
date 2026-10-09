// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ICoalesceAssignmentOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ICoalesceAssignmentOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static ICoalesceAssignmentOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ICoalesceAssignmentOperationWrapper));
        }

        private ICoalesceAssignmentOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;

        public static explicit operator ICoalesceAssignmentOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(ICoalesceAssignmentOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public IOperation Target => ((IAssignmentOperationWrapper)this).Target;
        public IOperation Value => ((IAssignmentOperationWrapper)this).Value;

        public static explicit operator ICoalesceAssignmentOperationWrapper(IAssignmentOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IAssignmentOperationWrapper(ICoalesceAssignmentOperationWrapper wrapper) => IAssignmentOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ICoalesceAssignmentOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ICoalesceAssignmentOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
