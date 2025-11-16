// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ISimpleAssignmentOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ISimpleAssignmentOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, bool> IsRefAccessor;
        private readonly IOperation operation;
        static ISimpleAssignmentOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ISimpleAssignmentOperationWrapper));
            IsRefAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsRef));
        }

        private ISimpleAssignmentOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public bool IsRef => IsRefAccessor(this.WrappedOperation);

        public static explicit operator ISimpleAssignmentOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(ISimpleAssignmentOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public IOperation Target => ((IAssignmentOperationWrapper)this).Target;
        public IOperation Value => ((IAssignmentOperationWrapper)this).Value;

        public static explicit operator ISimpleAssignmentOperationWrapper(IAssignmentOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IAssignmentOperationWrapper(ISimpleAssignmentOperationWrapper wrapper) => IAssignmentOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ISimpleAssignmentOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ISimpleAssignmentOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
