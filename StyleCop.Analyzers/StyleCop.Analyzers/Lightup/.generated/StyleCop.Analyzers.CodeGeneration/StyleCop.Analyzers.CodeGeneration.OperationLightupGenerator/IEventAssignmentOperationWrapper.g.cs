// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IEventAssignmentOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IEventAssignmentOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> EventReferenceAccessor;
        private static readonly Func<IOperation, IOperation> HandlerValueAccessor;
        private static readonly Func<IOperation, bool> AddsAccessor;
        private readonly IOperation operation;
        static IEventAssignmentOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IEventAssignmentOperationWrapper));
            EventReferenceAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(EventReference));
            HandlerValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(HandlerValue));
            AddsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(Adds));
        }

        private IEventAssignmentOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation EventReference => EventReferenceAccessor(this.WrappedOperation);
        public IOperation HandlerValue => HandlerValueAccessor(this.WrappedOperation);
        public bool Adds => AddsAccessor(this.WrappedOperation);

        public static explicit operator IEventAssignmentOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IEventAssignmentOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IEventAssignmentOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IEventAssignmentOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
