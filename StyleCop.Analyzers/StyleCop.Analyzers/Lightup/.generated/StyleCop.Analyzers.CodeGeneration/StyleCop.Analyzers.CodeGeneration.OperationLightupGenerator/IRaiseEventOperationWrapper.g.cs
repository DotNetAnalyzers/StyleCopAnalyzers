// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IRaiseEventOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IRaiseEventOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> EventReferenceAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> ArgumentsAccessor;
        private readonly IOperation operation;
        static IRaiseEventOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IRaiseEventOperationWrapper));
            EventReferenceAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(EventReference));
            ArgumentsAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(Arguments));
        }

        private IRaiseEventOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IEventReferenceOperationWrapper EventReference => IEventReferenceOperationWrapper.FromOperation(EventReferenceAccessor(this.WrappedOperation));
        public ImmutableArray<IOperation> Arguments => ArgumentsAccessor(this.WrappedOperation);
        public static explicit operator IRaiseEventOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IRaiseEventOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IRaiseEventOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IRaiseEventOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
