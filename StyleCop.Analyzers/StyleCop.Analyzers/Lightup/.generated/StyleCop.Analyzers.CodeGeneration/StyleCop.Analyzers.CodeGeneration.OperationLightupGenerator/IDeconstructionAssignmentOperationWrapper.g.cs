// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IDeconstructionAssignmentOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IDeconstructionAssignmentOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static IDeconstructionAssignmentOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IDeconstructionAssignmentOperationWrapper));
        }

        private IDeconstructionAssignmentOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;

        public static explicit operator IDeconstructionAssignmentOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IDeconstructionAssignmentOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public IOperation Target => ((IAssignmentOperationWrapper)this).Target;
        public IOperation Value => ((IAssignmentOperationWrapper)this).Value;

        public static explicit operator IDeconstructionAssignmentOperationWrapper(IAssignmentOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IAssignmentOperationWrapper(IDeconstructionAssignmentOperationWrapper wrapper) => IAssignmentOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IDeconstructionAssignmentOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IDeconstructionAssignmentOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
